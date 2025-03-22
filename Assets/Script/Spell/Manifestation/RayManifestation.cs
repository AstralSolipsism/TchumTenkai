using UnityEngine;

public class RayManifestation : 法术形态
{
    private float 能效损失计时;
    private 法术基础类 对抗目标;

    public override int 获取对抗加成(法术形态 其他形态)
    {
        return 0; // 射线不参与结构强度加成
    }

    protected override void 更新实体状态(GameObject 实体)
    {
        // 持续前进逻辑
        实体.transform.Translate(Vector3.forward * 配置参数.移动速度 * Time.deltaTime);
    }

    public override bool 是否开始处理对抗(int 强度差)
    {
        // 射线不会被结构强度破坏
        能效损失计时 += Time.deltaTime;
        if (能效损失计时 >= 1f)
        {
            宿主法术.currentMana -= Mathf.Abs(强度差);
            能效损失计时 = 0;
        }
        return true; // 始终存活
    }

    public override void 形态更新()
    {
        base.形态更新();

        if (对抗目标 != null)
        {
            能效损失计时 += Time.deltaTime;
            if (能效损失计时 >= 1f)
            {
                float 强度差 = Mathf.Abs(宿主法术.cohesion - 对抗目标.cohesion);
                宿主法术.currentMana = Mathf.Max(0, 宿主法术.currentMana - 强度差);
                能效损失计时 = 0;
            }
        }
    }
}