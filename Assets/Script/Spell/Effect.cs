using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static 法术管理局;

#region 基础枚举定义
public enum 效果类型
{
    即时,       // 立即生效
    持续,       // 持续生效
    触发选发,   // 满足条件触发
    触发必发,
    装备,       // 常驻生效
    连锁        // 可加入连锁
}

public enum 生效区域
{
    无限制,
    手牌区,
    场上区,
    墓地区,
    除外区
}

[Flags]
public enum 效果性质
{
    无 = 0,
    敌我通用 = 1 << 0,
    需选择对象 = 1 << 1,
    无视抗性 = 1 << 2,
    可连锁 = 1 << 3
}

#endregion

#region 效果数据类
[Serializable]
public class 效果配置
{
    public string 效果ID;
    public string 显示名称;
    public 效果类型 类型;
    public 生效区域 有效区域;
    public 效果性质 性质;
    public string 触发条件ID; // 对应条件系统的注册ID
    public string[] 监听事件类型; // 例如：["法术释放", "受到伤害"]
    public float 持续时间;
    public float 冷却时间;
    public int 优先级;
}
#endregion

#region   核心效果类
public abstract class 法术效果
{
    public 效果配置 配置 { get; set; }
    protected internal 法术基础类 宿主法术;
    private float 冷却剩余时间;

    // 条件检查委托
    public Func<bool> 发动条件 = () => true;
    public Action 支付代价 = () => { };
    public Func<IEnumerable<GameObject>> 对象判定 = () => new List<GameObject>();

    // 初始化方法
    public virtual void 初始化(法术基础类 宿主)
    {
        宿主法术 = 宿主;
    }

    // 主执行方法
    public 效果执行结果 效果发动(效果参数 参数)
    {
        支付代价.Invoke();
        处理冷却();
        if (!可发动()) return 效果执行结果.效果无效;
        if (!条件系统.检查条件(配置.触发条件ID, 参数))
            return 效果执行结果.效果无效;

        var 结果 = 结算效果();
        return 结果;
    }

    protected abstract 效果执行结果 结算效果();


    // 条件检查
    public bool 可发动()
    {
        return 冷却剩余时间 <= 0 &&
               区域条件满足() &&
               发动条件.Invoke() &&
               对象存在();
    }

    private bool 区域条件满足()
    {
        return 宿主法术.当前区域 == 配置.有效区域 || 配置.有效区域 == 生效区域.无限制;
    }

    private bool 对象存在()
    {
        return !配置.性质.HasFlag(效果性质.需选择对象) || 对象判定.Invoke().Any();
    }

    // 冷却处理
    private void 处理冷却()
    {
        if (配置.冷却时间 > 0)
        {
            冷却剩余时间 = 配置.冷却时间;
            效果冷却系统.注册冷却(this);
        }
    }

    public void 更新冷却(float deltaTime)
    {
        冷却剩余时间 = Mathf.Max(0, 冷却剩余时间 - deltaTime);
    }

    private void 注册事件监听()
    {
        foreach (var 事件类型 in 配置.监听事件类型)
        {
            事件系统.On法术事件 += 处理事件;
        }
    }

    private void 处理事件(效果参数 参数)
    {
        if (条件系统.检查条件(配置.触发条件ID, 参数))
        {
            效果发动(参数);
        }
    }
    #endregion

    #region 通用效果类
    public class 即时效果 : 法术效果
    {
        protected override 效果执行结果 结算效果()
        {
            // 执行即时处理逻辑
            return 效果执行结果.成功结算;
        }
    }

    public class 持续效果 : 法术效果
    {
        private float 持续时间;

        public override void 初始化(法术基础类 宿主)
        {
            base.初始化(宿主);
            持续时间 = 配置.持续时间;
        }

        protected override 效果执行结果 结算效果()
        {
            宿主法术.StartCoroutine(持续生效流程());
            return 效果执行结果.持续中;
        }

        private IEnumerator 持续生效流程()
        {
            while (持续时间 > 0)
            {
                执行持续逻辑();
                持续时间 -= Time.deltaTime;
                yield return null;
            }
        }
        protected internal virtual void 执行持续逻辑()
        {
            // 默认实现（兼容旧代码）
            执行持续逻辑();
        }
        protected internal virtual void 执行持续逻辑(效果参数 参数)
        {
            // 默认实现（兼容旧代码）
            执行持续逻辑();
        }
    }

    public class 抗破坏效果 : 法术效果.持续效果
    {
        protected override 效果执行结果 结算效果()
        {
            // 激活时提供抗性
            return base.结算效果();
        }

        protected internal override void 执行持续逻辑(效果参数 参数)
        {
            // 持续期间保持抗性
        }
    }

    #endregion

    #region 触发条件系统

    public class 条件系统
    {
        // 注册全局条件检查器
        private static Dictionary<string, Func<效果参数, bool>> 条件表 = new();

        public static void 注册条件(string 条件ID, Func<效果参数, bool> 检查器)
        {
            条件表[条件ID] = 检查器;
        }

        public static bool 检查条件(string 条件ID, 效果参数 参数)
        {
            return 条件表.TryGetValue(条件ID, out var checker) && checker(参数);
        }
    }



    #endregion

    #region 触发事件监听系统
    public static class 事件系统
    {
        public static event Action<效果参数> On法术事件;
        public static event Action<效果参数> On破坏成功;
        public static event Action<效果参数> On破坏抵抗;

        public static void 触发事件(string 事件类型, 效果参数 参数)
        {
            参数.事件类型 = 事件类型;
            On法术事件?.Invoke(参数);
        }

        public static void 触发破坏成功事件(效果参数 参数)
        {
            参数.事件类型 = "破坏成功";
            On破坏成功?.Invoke(参数);
        }

        public static void 触发破坏抵抗事件(效果参数 参数)
        {
            参数.事件类型 = "破坏抵抗";
            On破坏抵抗?.Invoke(参数);
        }
    }

    #endregion

    #region 效果管理系统

    public static class 效果优先级系统
    {
        public static void 处理效果冲突(List<法术效果> 生效效果, 效果参数 参数)
        {
            // 按优先级排序并执行
            var 排序效果 = 生效效果
                .OrderByDescending(e => e.配置.优先级)
                .ThenBy(e => e.宿主法术.spellLevel);

            foreach (var 效果 in 排序效果)
            {
                if (效果.可发动())
                {
                    效果.效果发动(参数);
                    if (参数.已修改强度) break; // 高优先级效果生效后终止
                }
            }
        }
    }
    public static class 效果查询系统
    {
        public static List<法术效果> 获取可用效果(生效区域 区域, 阵营类型 当前阵营)
        {
            var 结果 = new List<法术效果>();
            var 区域法术 = 法术管理局.实例.获取区域所有法术(区域);

            foreach (var 法术 in 区域法术)
            {
                foreach (var 效果 in 法术.效果列表)
                {
                    if (效果是否可用(效果, 当前阵营))
                    {
                        结果.Add(效果);
                    }
                }
            }

            return 结果.OrderByDescending(e => e.配置.优先级).ToList();
        }

        private static bool 效果是否可用(法术效果 效果, 阵营类型 当前阵营)
        {
            return 效果.可发动() &&
                  (效果属于当前阵营(效果, 当前阵营) ||
                   效果.配置.性质.HasFlag(效果性质.敌我通用));
        }

        private static bool 效果属于当前阵营(法术效果 效果, 阵营类型 当前阵营)
        {
            return 效果.宿主法术.是否属于阵营(当前阵营);
        }

        public static List<法术基础类> 获取场上通常法术(阵营类型 阵营)
        {
            return 法术管理局.实例.获取区域所有法术(生效区域.场上区)
                .Where(f => f.是否通常法术 && f.是否属于阵营(阵营))
                .ToList();
        }
    }
    #endregion

    #region 辅助类和接口
    public enum 效果执行结果
    {
        成功结算,
        效果无效,
        持续中
    }

    public interface I效果目标
    {
        bool 是否符合条件(法术效果 效果);
    }

    public class 效果参数
    {
        // 基础信息
        public string 事件类型; // 必填
        public GameObject 来源法术; // 必填

        // 时空信息
        public string 触发生命周期;
        public Vector3 触发位置;
        public float 触发时间;

        // 对象信息
        public GameObject 主要目标;
        public List<GameObject> 次要目标;
        public Dictionary<string, float> 数值参数 = new();
        

        // 状态信息
        public 法术效果 来源效果;
        public float 触发时魔力值;

        // 破坏相关参数
        public bool 破坏已生效 = false;
        public GameObject 破坏来源;

        //区域信息
        public 区域类型 新区域;
        public 区域类型 影响区域;
        internal 区域类型 原区域;

        // 自定义扩展
        public Dictionary<string, object> 扩展数据;

    }
}
#endregion

