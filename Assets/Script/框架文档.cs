/*
// 基础组件层
using System.Collections.Generic;
using UnityEngine;

public abstract class 法术基础类 : MonoBehaviour
{
    // 核心属性
    protected 法术形态 形态;
    protected SpellNeeds[] 需求成分;
    protected 法术效果[] 效果列表;
    protected 生命周期阶段 当前阶段;

    // 所有权相关属性
    public 阵营类型 来源阵营 { get; private set; }
    public I拥有者 当前拥有者 { get; protected set; }

    // 状态管理
    public void 进入施法阶段(I成分提供者 提供者) { ...}
    public void 提交成分(SpellNeeds 成分) { ...}
    protected virtual void 释放处理() { ...}
    protected virtual void 引导更新() { ...}

    // 初始化时设置来源
    public virtual void 初始化(I拥有者 初始拥有者)
    {
        当前拥有者 = 初始拥有者;
        来源阵营 = 初始拥有者.获取阵营();
    }

    // 所有权转移方法
    public virtual void 转移所有权(I拥有者 新拥有者)
    {
        // 验证是否可以转移（如不在引导阶段等）
        if(当前阶段 != 生命周期阶段.引导中){
            当前拥有者 = 新拥有者;
            触发所有权变更事件();
        }
    }
}

// 数据模型层
public struct SpellNeeds
{
    public enum 类型 { 言语, 姿势, 材料, 专注, 特殊 }
    public 类型 成分类型;
    public string 具体需求; // 如"火元素材料"
    public bool 需要持续提供;
}

public abstract class 法术形态
{
    public enum 大类 { 能量, 物质, 精神 }
    public 影响范围 作用范围;
    public 行动模式 移动方式;

    public abstract void 形态实例化(Vector3 施放位置);
    public abstract void 形态更新();
}

// 效果系统
public class 法术效果系统
{
    private static 效果通用方法 通用方法;

    public static void 结算效果(效果类型 类型, 效果参数 参数)
    {
        var 效果 = 通用方法.获取效果(类型);
        效果.效果发动(参数);
    }
}

// 在效果参数中携带所有权信息
public struct 效果参数
{
    public 法术基础类 来源法术;
    public Vector3 施放位置;
    public 阵营类型 当前阵营 => 来源法术.当前拥有者.获取阵营();
}

// 领域机关系统
public class 领域机关 : MonoBehaviour, I成分提供者
{
    private 机关单元[] 可用单元;
    private Dictionary<法术实例, 成分分配> 当前分配;

    public bool 尝试满足成分(法术实例 法术)
    {
        // 检查空闲单元能否满足需求成分
    }
}

// 施法者系统
public class 施法者 : MonoBehaviour, I成分提供者
{
    [SerializeField] 阵营类型 阵营;
    public 阵营类型 获取阵营() => 阵营;

    public 物品栏 装备栏;
    public 法术书 当前法术书;
    private 当前处理法术 正在处理的法术;

    // 在装备法术时自动设置来源
    public 法术基础类 生成法术实例(法术配置 配置)
    {
        var 实例 = Instantiate(配置.预制体);
        实例.初始化(this);
        return 实例;
    }

    public void 开始施法(法术基础类 法术)
    {
        if (正在处理的法术 == null)
        {
            法术.进入施法阶段(this);
        }
    }
}
// 法术书管理系统
public class 法术书
{
    // 卡组结构：键为法术模板，值为可用次数
    public Dictionary<法术模板, int> 可用法术 = new();

    // 实例化方法
    public 法术基础类 生成实例(法术模板 模板, I拥有者 拥有者)
    {
        if(可用法术[模板] > 0){
            可用法术[模板]--;
            var 实例 = Instantiate(模板.预制体);
            实例.初始化(拥有者);
            return 实例;
        }
        return null;
    }
    // 回收机制
    public void 回收法术(法术模板 模板)
    {
        if(可用法术.ContainsKey(模板)){
            可用法术[模板]++;
        }
    }
}

// 核心管理系统
public class 法术管理局 : MonoBehaviour
{
    public readonly 区域字典<法术基础类> 手牌区 = new();
    public readonly 区域字典<法术基础类> 场上区 = new();
    public readonly 区域字典<法术基础类> 墓地区 = new();
    public readonly 区域字典<法术基础类> 除外区 = new();
    
    // 自定义区域字典类
    public class 区域字典<T> where T : 法术基础类
    {
        private Dictionary<I拥有者, List<T>> 数据 = new();

        public void 添加(I拥有者 拥有者, T 实例)
        {
            if(!数据.ContainsKey(拥有者)){
                数据[拥有者] = new List<T>();
            }
            数据[拥有者].Add(实例);
        }

        public List<T> 获取所有(I拥有者 拥有者)
        {
            return 数据.TryGetValue(拥有者, out var list) ? list : new List<T>();
        }

        public void 移除(T 实例)
        {
            foreach(var kv in 数据){
                if(kv.Value.Remove(实例)){
                    return;
                }
            }
        }


    //连锁处理
    public Stack<法术效果> 连锁堆栈 = new();

    private IEnumerator 处理连锁()
    {
        yield return new 窗口期等待();
        while (连锁堆栈.Count > 0)
        {
            var 效果 = 连锁堆栈.Pop();
            效果.立即结算();
        }
    }

    // 根据阵营筛选法术
    public List<法术基础类> 获取敌对法术(阵营类型 请求方阵营)
    {
        return 场上区.Where(f => 
            f.当前拥有者.获取阵营() != 请求方阵营
        ).ToList();
    }

    // 处理法术碰撞时的阵营判断
    private void On法术碰撞(法术基础类 法术A, 法术基础类 法术B)
    {
        bool 是敌对关系 = 阵营系统.是否敌对(
            法术A.当前拥有者.获取阵营(),
            法术B.当前拥有者.获取阵营()
        );

        if(是敌对关系){
            触发敌对碰撞事件(法术A, 法术B);
        }
    }
}

// 接口定义
public interface I成分提供者
{
    bool 尝试满足成分(法术实例 法术);
    void 终止提供(法术实例 法术);
}

public interface I拥有者
{
    阵营类型 获取阵营();
    Transform 获取施法锚点(); // 用于视觉表现
}




// UI系统实现
public class 法术区域UI : MonoBehaviour
{
    [SerializeField] 新区域 目标区域;
    [SerializeField] RectTransform 内容根节点;
    [SerializeField] 法术UI元素 元素预制体;

    private ObjectPool<法术UI元素> 元素池;
    private Dictionary<法术基础类, 法术UI元素> 当前显示 = new();

    private void Start()
    {
        元素池 = new ObjectPool<法术UI元素>(
            createFunc: () => Instantiate(元素预制体, 内容根节点),
            actionOnGet: (ui) => ui.gameObject.SetActive(true),
            actionOnRelease: (ui) => ui.gameObject.SetActive(false)
        );

        法术管理局.实例.注册区域监听器(目标区域, 更新显示);
    }

    private void 更新显示(I拥有者 玩家, List<法术基础类> 法术列表)
    {
        // 移除不再存在的元素
        var 需移除 = 当前显示.Keys.Except(法术列表).ToList();
        foreach(var 法术 in 需移除){
            元素池.Release(当前显示[法术]);
            当前显示.Remove(法术);
        }

        // 添加新元素
        foreach(var 法术 in 法术列表){
            if(!当前显示.ContainsKey(法术)){
                var ui = 元素池.Get();
                ui.绑定数据(法术);
                当前显示[法术] = ui;
            }
        }

        // 更新布局
        StartCoroutine(延迟布局更新());
    }

    IEnumerator 延迟布局更新()
    {
        yield return null; // 等待一帧让UI元素尺寸稳定
        LayoutRebuilder.ForceRebuildLayoutImmediate(内容根节点);
    }
}

// 阵营关系系统（可扩展）
public static class 阵营系统
{
    // 简单版本：直接比较是否不同
    public static bool 是否敌对(阵营类型 a, 阵营类型 b)
    {
        return a != b;
        
        // 复杂版本示例：
        // return 阵营关系矩阵[a][b] == 关系类型.敌对;
    }
}

// 增强型模板配置
[CreateAssetMenu(menuName = "法术/模板")]
public class 法术模板 : ScriptableObject
{
    public 法术基础类 预制体;
    public SpellNeeds[] 必须成分;
    public 形态配置 形态参数;
    public 效果配置[] 效果链;

    [System.Serializable]
    public struct 形态配置
    {
        public 形态类型 类型;
        public float 作用半径;
        public float 移动速度;
    }

    [System.Serializable]
    public struct 效果配置
    {
        public 效果类型 类型;
        public int 伤害值;
        public 元素属性 元素;
    }
}
*/


/*示例合集
 // 典型生命周期实现示例
public class 火球术 : 法术基础类
{
    protected override void 释放处理()
    {
        base.释放处理();
        形态 = new 投射物形态(速度: 10f, 爆炸半径: 3f);
        法术效果系统.结算效果(效果类型.范围伤害, new 伤害参数
        {
            伤害值 = 50,
            元素类型 = 火元素
        });
    }
}
 
    // 区域转移方法示例
    public void 移动到场上(法术基础类 法术)
    {
        手牌区.移除(法术);
        场上区.添加(法术.当前拥有者, 法术);
        触发区域变更事件(法术, 新区域.手牌区, 新区域.场上区);
    }

// 动态成分绑定示例：
public class 动态成分系统 : MonoBehaviour
{
    void Update()
    {
        foreach(var 法术 in 法术管理局.实例.手牌区.获取所有(玩家)){
            if(法术.需求成分.Any(c => c.类型 == 成分类型.材料)){
                更新材料显示(法术);
            }
        }
    }

    void 更新材料显示(法术基础类 法术)
    {
        var 缺失材料 = 法术.需求成分
            .Where(c => !法术.已满足成分.Contains(c))
            .OfType<材料成分>();

        foreach(var 材料 in 缺失材料){
            UI系统.显示材料需求(材料.所需物品);
        }
    }
}

// 使用UnityEvent的监听机制
[System.Serializable]
public class 区域变更事件 : UnityEvent<I拥有者, List<法术基础类>> {}

public class 法术管理局 : MonoBehaviour
{
    public 区域变更事件 On手牌区变更 = new();
    
    private void 触发区域变更事件(法术基础类 法术, 新区域 原区域, 新区域 新区域)
    {
        // 获取受影响的所有者
        var 拥有者 = 法术.当前拥有者;
        
        // 触发对应区域事件
        switch(新区域){
            case 新区域.手牌区:
                On手牌区变更.Invoke(拥有者, 手牌区.获取所有(拥有者));
                break;
            // 其他区域处理...
        }
    }
}
 */