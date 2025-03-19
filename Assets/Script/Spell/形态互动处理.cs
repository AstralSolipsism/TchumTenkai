using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class 形态互动系统
{
    public static void 处理形态互动(法术形态 形态A, 法术形态 形态B, Collision 碰撞信息)
    {
        // 示例：火球与水箭相遇产生蒸汽
        if (形态A.配置参数.类型 == ManifestationCategory.Ray &&
           形态B.配置参数.类型 == ManifestationCategory.Ray)
        {
            /*生成互动效果(特效库.蒸汽爆炸, 碰撞信息.contacts[0].point);
            形态A.宿主法术.触发提前终止();
            形态B.宿主法术.触发提前终止();*/
        }
    }
}
