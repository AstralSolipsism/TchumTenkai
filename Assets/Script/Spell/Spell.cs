using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;




/*
#region Spell基类

public abstract class Spell : MonoBehaviour //Spell基类
{
    // 基础属性
    public Guid InstanceID;
    public int spellID;//每个法术唯一的编号
    public EffectType effectType; //效果类型
    public int spellLevel; //法术等级
    public School school; //法术学派
    public int cohesion; //结构强度 
    public float manaCost; //施法魔耗
    public float currentMana; //当前能效    
    public SpellNeeds requirements; // 成分系统    
    public SpellManifestation manifestation ;// 形态系统

    #region 生命周期管理
    public SpellState CurrentState { get; set; } = SpellState.Initialized;
    public bool IsManifestationInitialized { get; protected set; }
    public GameObject PrefabReference { get; private set; }
    
    public event Action<Spell> OnSpellExpired;
    #endregion

    // 获取成分中数据的方法
    public T GetComponentData<T>(ComponentType type) where T : class
    {
        foreach (var component in requirements.components)
        {
            if (component.type == type)
            {
                return component.componentData as T;
            }
        }
        return null;
    }

    protected virtual void Awake()
    {
        InstanceID = Guid.NewGuid();

    }


    // 开始施法
    public virtual void Initialize()
    {
        // 重置所有状态
        CurrentState = SpellState.Initialized;
        IsManifestationInitialized = false;
        if (requirements != null)
        {
            foreach (var component in requirements.components)
            {
                component.isFulfilled = false;
                component.fulfillProgress = 0;
            }
        }
    }

    public virtual void BeginCast()
    {
        Casting().Forget(); 
    }

    // 成分被满足时自动开始释放
    private async UniTaskVoid Casting()
    {
        // 执行成分满足的等待逻辑
        foreach (var component in requirements.components)
        {
            if (component.isRequired && !component.isFulfilled)
            {
                // 使用 UniTask 的等待方式
                await UniTask.WaitUntil(() => requirements.AllComponentsMet());
            }
        }

        ReleaseSpell();
    }

    // 释放法术
    protected virtual void ReleaseSpell()
    {
        CurrentState = SpellState.Released;
    }

    // 清理逻辑
    public virtual void CleanUp()
    {
        // 重置所有组件状态
        if (manifestation != null)
        {
            //manifestation.Reset();
        }
        StopAllCoroutines();
        OnSpellExpired = null;
    }

    // 法术结束处理
    public void TriggerExpiration()
    {
        CurrentState = SpellState.Expired;
        OnSpellExpired?.Invoke(this);
    }


}

#endregion



#region 成分要素
// 法术成分体系


[System.Serializable]
public class SpellComponent
{
    public ComponentType type;
    public bool isRequired = true;
    public float fulfillNeed;
    public float fulfillProgress;
    public bool isFulfilled;

    // 新增数据容器（使用Unity可序列化的泛型）
    [SerializeReference]
    public IComponentData componentData;
    // 条件验证接口
    public interface IComponentData { bool IsValid(); }
    public bool CheckFulfillment()
    {
        // 基础条件验证isFulfilled
        bool baseFulfilled = isFulfilled;

        // 自定义数据验证-componentData为空直接true，不为空则调用IsValid
        bool dataValid = componentData?.IsValid() ?? true;

        return baseFulfilled && dataValid;
    }
}
#region 特殊成分数据定义
// 目标成分数据
[System.Serializable]
public class TargetData : SpellComponent.IComponentData
{
    public GameObject targetObject;
    public Vector3 targetPosition;
    public bool IsValid()
    {
        bool objectValid = targetObject != null && targetObject;
        return objectValid;
    } 
}


/*示例：在释放法术时获取目标成分示例
     protected virtual void ReleaseSpell()
{
    // 获取目标信息
    var targetData = GetComponentData<TargetData>(ComponentType.Target);
    if (targetData != null)
    {
        GameObject target = targetData.targetObject;
        // 使用目标对象进行逻辑处理...
    }

    manifestation?.ApplyEffect(this);
    StartCoroutine(FinishSpell());
} 

#endregion
public class SpellNeeds
{
    public List<SpellComponent> components = new List<SpellComponent>();//声明并初始化一个 SpellComponent 类型的列表，用于存储法术施放所需的所有成分。
    public bool AllComponentsMet()
    {
        return components.TrueForAll(c => !c.isRequired || c.isFulfilled);//确保所有标记为 isRequired = true 的 SpellComponent 必须满足 isFulfilled = true，而非必须成分不影响结果。
    }
}



#endregion

#region 形态要素

public abstract class SpellManifestation
{
 

    public ManifestationCategory category;
    public float range;
    public Vector3 movementPattern;

    // 抽象方法
    public abstract void InitializeManifestation(Spell spell);
    public abstract void UpdateManifestation(Spell spell);
    public abstract void ApplyEffect(Spell spell);
}


#endregion
*/