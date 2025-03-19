using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectType { Cutting, Piercing, Blunt, Fire, Cold, Corrosion, Lightning, Sanctity, Devour, Psyche, Life, None }//属性：切割、穿刺、钝击、火焰、寒冷、腐蚀、雷电、神圣、黯噬、心灵、生命、无属性
public enum School { Protection, Alteration, Mentalism, Energy, Destiny, Arcane, Desecration }//学派：防护、变化、精神、能量、命运、奥秘、亵渎
public enum DurationType { Instant, Timed, Permanent }//持续时间：即时、定时、永久


public enum ComponentType //成分种类
{
    Verbal,        //言语
    Somatic,       //姿势
    Material,      //材料
    Mana,          // 魔力
    Target,        // 目标
    Concentration, // 专注
    Custom         // 特殊条件
}
public enum SpellState //法术生命周期管理
{
    Initialized,    // 已实例化
    Casting,        // 成分满足中，对应抽卡阶段
    Released,       // 释放法术，对应卡牌的发动、召唤
    Guilding,       // 引导中，对应场上卡牌的效果发动
    Active,         // 效果处理中
    Expired,        // 已过期，处理离场
}

public enum 阵营类型
{
    我方,
    敌方,
    中立
}