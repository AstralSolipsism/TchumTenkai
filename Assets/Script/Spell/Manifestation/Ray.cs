using UnityEngine;

public class 射线形态 : 法术形态
{
    private LineRenderer 射线渲染器;
    private float 累计持续时间;

    protected override void 配置实体(GameObject 实体)
    {
        base.配置实体(实体);
        射线渲染器 = 实体.GetComponent<LineRenderer>();
    }

    protected override void 更新实体状态(GameObject 实体)
    {
        base.更新实体状态(实体);
        /*更新射线效果();
        处理持续伤害();*/
    }

    /*private void 更新射线效果()
    {
        // 设置射线起点终点
        射线渲染器.SetPosition(0, transform.position);
        Vector3 终点 = transform.position + transform.forward * 配置参数.最大射程;

        // 碰撞检测
        if (Physics.Raycast(transform.position, transform.forward, out var hit, 配置参数.最大射程))
        {
            终点 = hit.point;
        }
        射线渲染er.SetPosition(1, 终点);
    }*/

    /*private void 处理持续伤害()
    {
        累计持续时间 += Time.deltaTime;
        if (累计Duration >= 配置参数.伤害间隔)
        {
            执行范围伤害();
            累计持续时间 = 0;
        }
    }*/
}