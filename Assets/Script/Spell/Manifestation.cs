// 基础形态框架
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ManifestationCategory
{
    Ray,        // 射线型
    Projectile, // 投射物
    Cloud,      // 云雾
    Shield,     // 护盾
    Beam,       // 持续光束
    AreaPulse,  // 区域脉冲
    Summon,     // 召唤物
    Custom      // 其他待开发类型
}

public abstract class 法术形态 : MonoBehaviour
{
    protected  internal 法术基础类 宿主法术;
    protected internal 形态配置 配置参数;
    protected List<GameObject> 形态实体列表 = new(); //一个法术的形态是固定的，但实体数量是不一定的，例如一个魔法飞弹法术可能射出多颗魔法飞弹

    public virtual void 初始化(法术基础类 宿主, 形态配置 配置)
    {
        宿主法术 = 宿主;
        配置参数 = 配置;
        生成初始实体();
    }

    protected virtual void 生成初始实体()
    {
        // 默认生成单个实体
        var 实体 = Instantiate(配置参数.基础预制体, transform);
        形态实体列表.Add(实体);
        配置实体(实体);
    }

    protected virtual void 配置实体(GameObject 实体)
    {
        // 设置基本属性
        var 控制器 = 实体.GetComponent<形态实体控制器>();
        if (控制器 != null)
        {
            控制器.初始化(this);
        }
    }

    public virtual void 形态更新()
    {
        // 更新所有实体状态
        foreach (var 实体 in 形态实体列表)
        {
            if (实体.activeSelf)
            {
                更新实体状态(实体);
            }
        }
    }

    protected virtual void 更新实体状态(GameObject 实体)
    {
        /* 例如基础移动逻辑
        var 移动组件 = 实体.GetComponent<形态移动组件>();
        移动组件?.执行移动(Time.deltaTime);*/
    }

    public virtual void 销毁形态()
    {
        foreach (var 实体 in 形态实体列表)
        {
            StartCoroutine(播放销毁特效(实体));
        }
        形态实体列表.Clear();
    }

    IEnumerator 播放销毁特效(GameObject 实体)
    {
        var 粒子系统 = 实体.GetComponentInChildren<ParticleSystem>();
        if (粒子系统 != null)
        {
            实体.GetComponent<Renderer>().enabled = false;
            粒子系统.Play();
            yield return new WaitUntil(() => !粒子系统.isPlaying);
        }
        Destroy(实体);
    }

    public virtual int 获取对抗加成(法术形态 其他形态)
    {
        return 0; // 默认无加成
    }
     public virtual int 获取结构强度加成(法术形态 对抗形态)
    {
        return 0;
    }

    public virtual bool 是否开始处理对抗(int 强度差)
    {
        return 强度差 >= 0; // 默认对抗逻辑
    }

    // 能效系数影响方法
    public virtual float 影响能效损失系数(float 原始系数, int 强度差)
    {
        return 原始系数;
    }
}



/* 使用示例：创建火球术
[CreateAssetMenu(menuName = "法术/火球术")]
public class 火球术配置 : 法术模板
{
    [Header("形态参数")]
    public 形态配置 火球形态;

    public override void 初始化法术(法术基础类 实例)
    {
        base.初始化法术(实例);
        实例.初始化形态(火球形态);
    }
}
*/