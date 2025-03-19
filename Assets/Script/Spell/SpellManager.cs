using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using static 法术效果;

public interface I拥有者
{
    阵营类型 获取阵营();
    Transform 获取施法锚点(); // 用于视觉表现
    int 获取法术层();
}

public class 法术管理局 : MonoBehaviour
{
    public static 法术管理局 实例;

    void Awake()
    {
        if (实例 != null && 实例 != this)
        {
            Destroy(gameObject);
            return;
        }
        实例 = this;
        DontDestroyOnLoad(gameObject);

        条件系统.注册条件("碰撞时", param =>
        param.事件类型 == "碰撞"
);

        条件系统.注册条件("法术释放", param =>
            param.事件类型== "法术释放"
        );

        条件系统.注册条件("进入墓地", param =>
            param.原区域 == 区域类型.墓地区
        );
    }

    // 互动处理方法
    public void 报告形态碰撞(法术形态 来源形态, Collision 碰撞信息)
    {
        var 目标形态 = 碰撞信息.gameObject.GetComponent<法术形态>();
        if (目标形态 != null)
        {
            //处理形态互动(来源形态, 目标形态, 碰撞Info);
        }
    }

    public void 报告形态碰撞(法术形态 来源形态, Collider 碰撞信息)
    {
        var 目标形态 = 碰撞信息.gameObject.GetComponent<法术形态>();
        if (目标形态 != null)
        {
            //处理形态互动(来源形态, 目标形态, 碰撞Info);
        }
    }

    #region 区域管理
    public enum 区域类型 { 手牌区, 场上区, 墓地区, 除外区 }

    public class 区域变更事件 : UnityEvent<I拥有者, 区域类型, 区域类型, 法术基础类> { }
    public 区域变更事件 On区域变更 = new 区域变更事件();

    public class 区域字典<T> where T : 法术基础类
    {
        protected internal Dictionary<I拥有者, List<T>> 数据 = new Dictionary<I拥有者, List<T>>();

        public void 添加(I拥有者 拥有者, T 实例)
        {
            if (!数据.ContainsKey(拥有者)) 数据[拥有者] = new List<T>();
            数据[拥有者].Add(实例);
        }

        public List<T> 获取区域所有()
        {
            return 数据.Values.SelectMany(x => x).ToList();
        }

        public List<T> 获取拥有者法术(I拥有者 拥有者)
        {
            return 数据.TryGetValue(拥有者, out var list) ? list : new List<T>();
        }

        public bool 移除(T 实例)
        {
            foreach (var kv in 数据)
            {
                if (kv.Value.Remove(实例))
                {
                    if (kv.Value.Count == 0) 数据.Remove(kv.Key);
                    return true;
                }
            }
            return false;
        }

        public void 清空() => 数据.Clear();
    }

    public 区域字典<法术基础类> 手牌区 = new 区域字典<法术基础类>();
    public 区域字典<法术基础类> 场上区 = new 区域字典<法术基础类>();
    public 区域字典<法术基础类> 墓地区 = new 区域字典<法术基础类>();
    public 区域字典<法术基础类> 除外区 = new 区域字典<法术基础类>();

    public void 转移法术(法术基础类 法术, 区域类型 目标区域)
    {
        var 原区域 = 获取当前区域(法术);
        if (原区域 == 目标区域) return;

        // 从原区域移除
        Get区域字典(原区域).移除(法术);

        // 加入新区域
        var 新区域字典 = Get区域字典(目标区域);
        新区域字典.添加(法术.当前拥有者, 法术);

        // 触发事件
        On区域变更.Invoke(法术.当前拥有者, 原区域, 目标区域, 法术);

        // 处理区域进入逻辑
        处理区域进入(法术, 目标区域);
    }

    private void 处理区域进入(法术基础类 法术, 区域类型 新区域)
    {
        // 仅更新区域状态
        法术.当前区域 = 转换为生效区域(新区域);

        // 触发效果执行（由效果系统处理具体逻辑）
        法术.触发效果("区域进入");
    }

    private 生效区域 转换为生效区域(区域类型 类型)
    {
        return 类型 switch
        {
            区域类型.场上区 => 生效区域.场上区,
            区域类型.手牌区 => 生效区域.手牌区,
            区域类型.墓地区 => 生效区域.墓地区,
            区域类型.除外区 => 生效区域.除外区,
            _ => 生效区域.无限制
        };
    }

    protected internal 区域类型 获取当前区域(法术基础类 法术)
    {
        if (手牌区.获取拥有者法术(法术.当前拥有者).Contains(法术)) return 区域类型.手牌区;
        if (场上区.获取拥有者法术(法术.当前拥有者).Contains(法术)) return 区域类型.场上区;
        if (墓地区.获取拥有者法术(法术.当前拥有者).Contains(法术)) return 区域类型.墓地区;
        return 区域类型.除外区;
    }

    public List<法术基础类> 获取敌方法术(阵营类型 我方阵营)
    {
        return 场上区.数据
            .Where(kv => kv.Key.获取阵营() != 我方阵营)
            .SelectMany(kv => kv.Value)
            .ToList();
    }
    private 区域字典<法术基础类> Get区域字典(区域类型 类型)
    {
        switch (类型)
        {
            case 区域类型.手牌区: return 手牌区;
            case 区域类型.场上区: return 场上区;
            case 区域类型.墓地区: return 墓地区;
            default: return 除外区;
        }
    }

    public void 注册区域转移事件(法术基础类 法术, 区域类型 原区域)
    {
        事件系统.触发事件("法术区域转移", new 效果参数
        {
            事件类型 = "区域变更",
            来源法术 = 法术.gameObject,
            原区域 = 原区域,
            新区域 = 法术管理局.实例.获取当前区域(法术)
        });
    }
    #endregion

    #region 连锁系统
    public Stack<(法术效果 效果, 效果参数 参数)> 连锁堆栈 = new Stack<(法术效果, 效果参数)>();
    private Coroutine 当前连锁处理;

    public void 添加连锁(法术效果 效果, 效果参数 触发参数)
    {
        连锁堆栈.Push((效果, 触发参数));
        if (当前连锁处理 == null)
        {
            当前连锁处理 = StartCoroutine(处理连锁());
        }
    }

    private IEnumerator 处理连锁()
    {
        yield return new WaitForSeconds(0.8f);

        while (连锁堆栈.Count > 0)
        {
            var (效果, 参数) = 连锁堆栈.Pop();

            // 自动补充必要参数
            if (参数.来源法术 == null)
            {
                参数.来源法术 = 效果.宿主法术.gameObject;
            }

            效果.效果发动(参数);
            yield return new WaitForSeconds(0.2f);
        }

        当前连锁处理 = null;
    }

    #endregion

    #region 阵营系统
    public enum 阵营关系 { 友好, 敌对, 中立 }

    public List<法术基础类> 获取敌对法术(阵营类型 请求方阵营)
    {
        List<法术基础类> 结果 = new List<法术基础类>();
        foreach (var kv in 场上区.数据)
        {
            if (kv.Key.获取阵营() != 请求方阵营)
            {
                结果.AddRange(kv.Value);
            }
        }
        return 结果;
    }

    public 阵营关系 获取阵营关系(阵营类型 a, 阵营类型 b)
    {
        if (a == b) return 阵营关系.友好;
        if (a == 阵营类型.中立 || b == 阵营类型.中立) return 阵营关系.中立;
        return 阵营关系.敌对;
    }
    #endregion

    #region 生命周期管理

    void Update()
    {
        更新法术状态();
    }

    void 更新法术状态()
    {
        foreach (var 法术 in 获取所有法术())
        {
            if (法术.当前阶段 == 生命周期阶段.已释放)
            {
                法术.形态更新();
            }
        }
    }

    public IEnumerable<法术基础类> 获取全局法术()
    {
        return 手牌区.获取区域所有()
              .Concat(场上区.获取区域所有())
              .Concat(墓地区.获取区域所有())
              .Concat(除外区.获取区域所有());
    }
    public IEnumerable<法术基础类> 获取区域所有法术(生效区域 区域)
    {
        return Get区域字典(转换区域类型(区域)).获取区域所有()
               .Where(f => f.当前区域 == 区域);
    }

    public IEnumerable<法术基础类> 获取可操作法术(生效区域 区域, 阵营类型 当前玩家阵营)
    {
        return 获取区域所有法术(区域)
              .Where(f => f.是否属于阵营(当前玩家阵营) ||
                         f.效果列表.Any(e => e.配置.性质.HasFlag(效果性质.敌我通用)))
              .OrderByDescending(f => f.效果列表.Max(e => e.配置.优先级));
    }
    #endregion

    #region 碰撞处理
    public void 报告形态碰撞(法术基础类 来源法术, GameObject 目标对象)
    {
        var 目标法术 = 目标对象.GetComponent<法术基础类>();
        if (目标法术 == null) { 触发基础效果(来源法术, 目标对象); return; }

        bool 是敌对 = 获取阵营关系(
            来源法术.当前拥有者.获取阵营(),
            目标法术.当前拥有者.获取阵营()
        ) == 阵营关系.敌对;

        if (是敌对)
        {
            // 执行法术对抗逻辑
            bool 来源存活 = 来源法术.结构强度对抗(目标法术);           
            触发敌对碰撞(来源法术, 目标法术);
        }
    }

    public UnityEvent<法术基础类, 法术基础类> On敌对碰撞 = new UnityEvent<法术基础类, 法术基础类>();

    private void 触发基础效果(法术基础类 法术, GameObject 目标) //等同于游戏王战斗阶段
    {
        if (法术.基础效果 == null) return;

        var 参数 = new 效果参数
        {
            事件类型 = "基础碰撞",
            来源法术 = 法术.gameObject,
            主要目标 = 目标,
            数值参数 = new Dictionary<string, float> {
            { "基础威力", 法术.power },
            { "能效系数", 法术.currentMana / 法术.manaCost }
        }
        };

        法术效果系统.执行效果(法术.基础效果, 参数);
    }

    void 触发敌对碰撞(法术基础类 a, 法术基础类 b)
    {
        /*On敌对碰撞.Invoke(a, b);
        a.处理敌对碰撞(b);
        b.处理敌对碰撞(a);*/
    }
    #endregion

    #region 效果相关

    public IEnumerable<法术基础类> 获取区域法术(生效区域 区域)
    {
        switch (区域)
        {
            case 生效区域.手牌区: return 手牌区.获取区域所有();
            case 生效区域.场上区: return 场上区.获取区域所有();
            case 生效区域.墓地区: return 墓地区.获取区域所有();
            case 生效区域.除外区: return 除外区.获取区域所有();
            default: return 获取全局法术();
        }
    }

    private IEnumerable<法术基础类>获取所有法术()
    {
        return 手牌区.获取区域所有()
            .Concat(场上区.获取区域所有())
            .Concat(墓地区.获取区域所有())
            .Concat(除外区.获取区域所有());
    }

    #endregion

    #region 区域类型转换
    private 区域类型 转换区域类型(生效区域 区域)
    {
        return 区域 switch
        {
            生效区域.手牌区 => 区域类型.手牌区,
            生效区域.场上区 => 区域类型.场上区,
            生效区域.墓地区 => 区域类型.墓地区,
            生效区域.除外区 => 区域类型.除外区,
            _ => 区域类型.手牌区
        };
    }
    #endregion
}
