using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static 法术形态;
using static 法术效果;
using static 法术效果.效果参数;
using static 法术管理局;

// 在法术基础类中的成分管理
public enum 生命周期阶段
{
    实例化,
    施法中,
    已释放,
    引导中,
    效果处理中,
    已离场
}
public abstract class 法术基础类 : MonoBehaviour
{
    #region 核心属性
    public string 法术ID;
    public EffectType effectType; //伤害类型
    public int spellLevel; //额定法术等级
    public int currentLevel;//生效法术等级
    public School school; //法术学派
    public int cohesion; //结构强度 
    public float manaCost; //施法魔耗
    public float currentMana; //当前魔力
    public int power;//威力
    public ManifestationTag Tags;



    [SerializeField] protected internal 生命周期阶段 当前阶段 = 生命周期阶段.实例化;
    [SerializeField] protected internal 法术效果 基础效果;
    [SerializeField] protected internal List<法术效果> 效果列表 = new List<法术效果>();

    //效果处理相关变量
    public bool 是否通常法术 => 效果列表.Count == 0;
    private bool _可被破坏 = true;
    private bool _可被取对象 = true;

    protected I成分提供者 当前提供者;
    protected Coroutine 引导协程;

    [Header("事件配置")]
    public UnityEvent 进入施法阶段事件;
    public UnityEvent 成分满足事件;
    public UnityEvent 释放成功事件;
    public UnityEvent 引导开始事件;
    public UnityEvent 引导结束事件;
    public UnityEvent 所有权变更事件;
    #endregion

    #region 法术成分管理
    private List<法术成分> 需求成分 = new List<法术成分>();
    private Dictionary<法术成分, bool> 完成状态 = new();

    public void 添加成分需求(法术成分 成分)
    {
        需求成分.Add(成分);
        完成状态[成分] = false; // 初始状态为未完成
    }


    public bool 尝试提交成分(成分类型 类型, object 参数)
    {
        bool 有进展 = false;
        foreach (var 成分 in 需求成分.Where(c => c.类型 == 类型 && !c.已完成))
        {
            if (成分.验证条件(参数))
            {
                成分.提交进度();
                有进展 = true;
                if (成分.已完成)
                    完成状态[成分] = true;
            }
        }
        return 有进展;
    }

    public IEnumerable<成分类型> 获取需求成分类型()
    {        
        return 需求成分.Select(c => c.类型).Distinct();
    }
    public bool 是否完成所有成分 => 完成状态.Values.All(v => v);
    #endregion

    #region 法术形态管理

    private 法术形态 当前形态;
    public 形态配置 形态配置;

    protected  internal void 初始化形态(形态配置 配置)
    {
        var 形态组件 = gameObject.AddComponent(获取形态类型(配置.类型)) as 法术形态;
        形态组件.初始化(this, 配置);
        当前形态 = 形态组件;
    }

    private System.Type 获取形态类型(ManifestationCategory 类型)
    {
        // 配置的映射表更合适，这里简化为switch
        switch (类型)
        {
            case ManifestationCategory.Projectile: return typeof(Projectile);
            case ManifestationCategory.Ray: return typeof(RayManifestation);
            // 其他类型映射...
            default: return typeof(法术形态);
        }
    }

    public void 形态更新()
    {
        当前形态?.形态更新();
    }

    public void 处理碰撞事件(Collider other)
    {
        var 碰撞参数 = new 效果参数
        {
            事件类型 = "碰撞",
            来源法术 = this.gameObject,
            主要目标 = other.gameObject,            
        };

        // 触发碰撞事件
        事件系统.触发事件("碰撞", 碰撞参数);
    }



    #endregion

    #region 标签管理
    public bool HasTag(ManifestationTag tag)  //标签校验
    {
        return (Tags & tag) == tag;
    }

    public void AddTag(ManifestationTag tag) //标签添加
    {
        Tags |= tag;
    }

    public void RemoveTag(ManifestationTag tag) //标签移除
    {
        Tags &= ~tag;
    }
    #endregion

    #region 状态管理
    public void 进入施法阶段(I成分提供者 提供者)
    {
        if (当前阶段 != 生命周期阶段.实例化) return;

        当前提供者 = 提供者;
        当前阶段 = 生命周期阶段.施法中;
        进入施法阶段事件.Invoke();
        StartCoroutine(施法超时检测());
    }

    private IEnumerator 施法超时检测()
    {
        float 超时时间 = 30f; // 可从配置读取
        float 计时 = 0;

        while (当前阶段 == 生命周期阶段.施法中)
        {
            计时 += Time.deltaTime;
            if (计时 >= 超时时间)
            {
                终止施法();
                yield break;
            }
            yield return null;
        }
    }

    private void 终止施法()
    {
        Debug.Log($"{name} 施法超时终止");
        进入离场阶段();
    }

    private void 检查成分完成()
    {
        if (是否完成所有成分)
        {
            成分满足事件.Invoke();
            执行释放();
        }
    }

    private void 执行释放()
    {
        当前阶段 = 生命周期阶段.已释放;
        释放处理();
        释放成功事件.Invoke();

    }

    protected virtual void 释放处理()
    {
        // 由具体法术实现形态初始化等逻辑
        // 触发释放事件
        事件系统.触发事件("法术释放", new 效果参数
        {
            来源法术 = this.gameObject
        });



    }

    public void 开始引导()
    {
        if (当前阶段 != 生命周期阶段.已释放) return;

        当前阶段 = 生命周期阶段.引导中;
        引导协程 = StartCoroutine(引导更新循环());
        引导开始事件.Invoke();
    }

    private IEnumerator 引导更新循环()
    {
        float 上次时间 = Time.time;
        while (当前阶段 == 生命周期阶段.引导中)
        {
            float deltaTime = Time.time - 上次时间;
            引导更新(deltaTime);
            上次时间 = Time.time;
            yield return null;
        }
    }
    protected virtual void 引导更新(float deltaTime)
    {
        var 参数 = new 效果参数
        {
            事件类型 = "引导更新",
            来源法术 = this.gameObject,
            数值参数 = new Dictionary<string, float> {
            { "时间增量", deltaTime },
            { "当前魔力", currentMana }
        }
        };

        foreach (var 效果 in 效果列表.Where(e => e.配置.触发条件ID == "引导阶段"))
        {
            法术效果系统.执行持续效果(效果, 参数); // 传递参数
        }
    }
    public void 中断引导()
    {
        if (引导协程 != null)
        {
            StopCoroutine(引导协程);
            引导结束事件.Invoke();
        }
        进入离场阶段();
    }

    public void 进入离场阶段()
    {
        var 原区域 = 法术管理局.实例.获取当前区域(this);
        法术管理局.实例.注册区域转移事件(this, 原区域);

        StartCoroutine(离场处理(原区域));
    }

    private IEnumerator 离场处理(区域类型 原区域)
    {
        var 参数 = new 效果参数
        {
            事件类型 = "离场触发",
            来源法术 = this.gameObject,        
            原区域 = 原区域,
            新区域 = 区域类型.墓地区
            
        };

        foreach (var 效果 in 效果列表.Where(e => e.配置.触发条件ID == "离场时"))
        {
            法术效果系统.执行效果(效果, 参数); // 传递离场参数
            yield return null; // 允许分帧处理
        }

        // 视觉表现处理（示例）
        //yield return StartCoroutine(播放离场动画());

        //法术管理局.实例.注销法术(this);
        Destroy(gameObject);
    }
    #endregion

    #region 所有权管理
    [SerializeField] private 阵营类型 来源阵营;
    [SerializeField] protected internal I拥有者 当前拥有者;

    public void 初始化(I拥有者 初始拥有者)
    {
        当前拥有者 = 初始拥有者;
        来源阵营 = 初始拥有者.获取阵营();
        gameObject.layer = 初始拥有者.获取法术层();
    }

    public void 转移所有权(I拥有者 新拥有者)
    {
        if (当前阶段 == 生命周期阶段.引导中) return;

        // 移交成分提供者
        if (当前提供者 is I成分提供者 旧提供者)
        {
            旧提供者.终止提供(this);
        }

        当前拥有者 = 新拥有者;

        if (新拥有者 is I成分提供者 新提供者)
        {
            当前提供者 = 新提供者;
            新提供者.开始提供(this);
        }

        更新视觉表现();
        所有权变更事件.Invoke();
    }

    private void 更新视觉表现()
    {
        /* 根据新拥有者阵营更新材质、粒子颜色等
        var 外观组件 = GetComponent<法术外观>();
        if (外观组件 != null)
        {
            外观组件.更新阵营外观(当前拥有者.获取阵营());
        }*/
    }
    #endregion

    #region 结构强度对抗
    public bool 结构强度对抗(法术基础类 其他法术)
    {
        int 形态加成A = 当前形态.获取对抗加成(其他法术.当前形态);
        int 形态加成B = 其他法术.当前形态.获取对抗加成(当前形态);
        int 强度差 = (cohesion + 形态加成A) - (其他法术.cohesion + 形态加成B);

        // 优先处理特殊形态逻辑
        if (当前形态 != null && !当前形态.是否开始处理对抗(强度差))
        {
            return false;
        }

        // 常规对抗逻辑
        if (强度差 < 0)
        {
            破坏();
            return false;
        }
        else
        {
            float 能效损失系数 = 获取能效损失系数(强度差);

            // 应用形态影响
            if (当前形态 != null)
            {
                能效损失系数 = 当前形态.影响能效损失系数(能效损失系数, 强度差);
            }

            float 能效损失值 = 其他法术.currentMana * 能效损失系数;

            currentMana = Mathf.Max(0, currentMana - 能效损失值);

            // 能效耗尽检查
            if (currentMana <= 0)
            {
                破坏();
                return false;
            }

            // 强度差>=2时破坏对方
            if (强度差 >= 2)
            {
                其他法术.破坏();
            }
            return true;
        }
    }


    protected virtual float 获取能效损失系数(int 强度差)
    {
        return 强度差 switch
        {
            0 => 1.0f,
            1 => 0.6f,
            _ => 0.3f // 默认值（强度差>=2时）
        };
    }
    private int 获取形态加成(法术基础类 当前法术, 法术基础类 对方法术)
    {
        if (当前法术.当前形态 != null &&
            当前法术.当前形态.配置参数.类型 == ManifestationCategory.Shield &&
            (对方法术.当前形态.配置参数.类型 == ManifestationCategory.Projectile ||
             对方法术.当前形态.配置参数.类型 == ManifestationCategory.Ray))
        {
            return 2;
        }
        return 0;
    }

    #endregion

    #region 破坏相关

    public void 破坏()
    {
        if (!可被破坏检查()) return;

        // 触发破坏前事件
        事件系统.触发事件("破坏尝试", new 效果参数
        {
            来源法术 = this.gameObject,
            事件类型 = "破坏尝试"
        });

        if (可被破坏检查())
        {
            进入离场阶段();
        }
        else
        {
            // 触发破坏抵抗事件
            事件系统.触发事件("破坏抵抗", new 效果参数
            {
                来源法术 = this.gameObject,
                事件类型 = "破坏抵抗"
            });
        }
    }

    private bool 可被破坏检查()
    {
        // 基础可破坏性 + 效果影响
        return _可被破坏 && !效果列表.Any(e => e is 抗破坏效果);
    }


    #endregion

    #region 效果管理
    public 生效区域 当前区域;//持续法术离开指定区域将不生效

    public void 添加效果(法术效果 效果)
    {
        效果.初始化(this);
        效果列表.Add(效果);
    }

    public void 触发效果(string 效果ID, 效果参数 参数 = null)
    {
        var 效果 = 效果列表.FirstOrDefault(e => e.配置.效果ID == 效果ID);
        if (效果 == null) return;

        // 自动填充默认参数
        if (参数 == null)
        {
            参数 = new 效果参数
            {
                来源法术 = this.gameObject,
                事件类型 = 效果ID
            };
        }

        效果.效果发动(参数);
    }
    
    public IEnumerable<法术效果> 可用效果 => //效果访问器，用于查询
    效果列表.Where(e => e.可发动());
    #endregion

    #region 辅助方法
    public T 获取形态组件<T>() where T : 法术形态
    {
        return 当前形态 as T;
    }

    public bool 是否属于阵营(阵营类型 阵营)
    {
        return 当前拥有者.获取阵营() == 阵营;
    }
    #endregion

}