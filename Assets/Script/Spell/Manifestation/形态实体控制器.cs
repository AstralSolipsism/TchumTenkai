using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 形态实体控制器 : MonoBehaviour //附加在形态实体预制体上，例如单个魔法飞弹。用于连接Spell与形态
{
    private 法术形态 所属形态;

    public void 初始化(法术形态 形态)
    {
        所属形态 = 形态;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (所属形态 != null)
        {
            所属形态.宿主法术.处理碰撞事件(other);
            // 向全局管理器报告碰撞
            法术管理局.实例.报告形态碰撞(所属形态, other);
            所属形态.宿主法术.处理碰撞事件(other);
        }
    }
}
