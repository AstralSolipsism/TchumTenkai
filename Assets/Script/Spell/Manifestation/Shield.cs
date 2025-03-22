using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : 法术形态
{
    public override int 获取对抗加成(法术形态 其他形态)
    {
        if (其他形态 != null &&
           (其他形态.配置参数.类型 == ManifestationCategory.Ray ||
            其他形态.配置参数.类型 == ManifestationCategory.Projectile))
        {
            return 3; // 对抗射线和飞弹时+3结构强度
        }
        return 0;
    }

    public override float 影响能效损失系数(float 原始系数, int 强度差)
    {
        // 盾形态在强度差为0时获得10%减伤
        return 强度差 == 0 ? 原始系数 * 0.9f : 原始系数;
    }

    protected override void 更新实体状态(GameObject 实体)
    {
        
        
    }
}
