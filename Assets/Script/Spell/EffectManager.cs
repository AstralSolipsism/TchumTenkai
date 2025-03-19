using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static 法术效果;

public static class 法术效果系统
{
    public static void 执行效果(法术效果 效果, 效果参数 参数)
    {
        // 添加事件触发
        事件系统.触发事件("效果发动", new 效果参数
        {
            来源效果 = 效果,
            事件类型 = "效果前置"
        });

        效果.效果发动(参数);

        事件系统.触发事件("效果发动", new 效果参数
        {
            来源效果 = 效果,
            事件类型 = "效果后置"
        });
    }

    public static void 执行持续效果(法术效果 效果, 效果参数 参数)
    {
        if (效果 is 持续效果 持续)
        {
            float deltaTime = 参数.数值参数.TryGetValue("时间增量", out var dt) ? dt : Time.deltaTime;
            持续.执行持续逻辑(参数);
        }
    }
}

public static class 效果冷却系统
{
    private static Dictionary<string, List<法术效果>> 冷却效果表 = new Dictionary<string, List<法术效果>>();

    public static void 注册冷却(法术效果 效果)
    {
        string 冷却键 = 生成冷却键(效果);
        if (!冷却效果表.ContainsKey(冷却键))
        {
            冷却效果表[冷却键] = new List<法术效果>();
        }
        冷却效果表[冷却键].Add(效果);
    }

    public static bool 处于冷却(法术效果 效果)
    {
        string 冷却键 = 生成冷却键(效果);
        return 冷却效果表.ContainsKey(冷却键) &&
              冷却效果表[冷却键].Any(e => e.可发动());
    }

    private static string 生成冷却键(法术效果 效果)
    {
        return $"{效果.宿主法术.法术ID}_{效果.配置.效果ID}";
    }

    public static void 更新所有冷却(float deltaTime)
    {
        foreach (var list in 冷却效果表.Values)
        {
            foreach (var effect in list)
            {
                effect.更新冷却(deltaTime);
            }
        }
    }
}