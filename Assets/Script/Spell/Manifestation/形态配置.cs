using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 形态配置数据
[CreateAssetMenu(menuName = "法术/形态配置")]
public class 形态配置 : ScriptableObject
{
    public GameObject 基础预制体;
    public ManifestationCategory 类型;

    [Header("通用参数")]
    public float 移动速度 = 5f;
    public float 持续时间 = 3f;
    public int 同时生成数量 = 1;
    public float 散布角度 = 30f;

    [Header("特殊参数")]
    public bool 启用自动追踪;
    public float 追踪角度限制 = 45f;
    public float 伤害间隔 = 0.5f;
}
