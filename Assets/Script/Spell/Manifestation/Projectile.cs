using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static 法术效果;

public class Projectile : 法术形态
{
    private int 当前发射数量;
    private float 生成间隔 = 0.3f;
    private Coroutine 生成协程;
    protected override void 生成初始实体()
    {
        生成协程 = StartCoroutine(间隔生成实体());
    }

    private IEnumerator 间隔生成实体()
    {
        for (int i = 0; i < 配置参数.同时生成数量; i++)
        {
            var 实体 = Instantiate(配置参数.基础预制体, transform);
            实体.transform.localPosition = 计算散布位置(i);
            形态实体列表.Add(实体);
            配置实体(实体);

            // 触发飞弹生成事件
            事件系统.触发事件("飞弹生成", new 效果参数
            {
                来源法术 = this.宿主法术.gameObject,
                数值参数 = new Dictionary<string, float> {
                    { "当前数量", i+1 },
                    { "最大数量", 配置参数.同时生成数量 }
                }
            });

            yield return new WaitForSeconds(生成间隔);
        }
    }

    private Vector3 计算散布位置(int 索引)
    {
        float 角度间隔 = 配置参数.散布角度 / 配置参数.同时生成数量;
        return Quaternion.Euler(0, 角度间隔 * 索引, 0) * Vector3.forward * 0.5f;
    }

    /*protected override void 更新实体状态(GameObject 实体)
    {
        base.更新实体状态(实体);

        // 自动追踪逻辑
        if (配置参数.启用自动追踪)
        {
            var 追踪组件 = 实体.GetComponent<自动追踪组件>();
            追踪组件?.更新目标(获取最近敌人());
        }
    }*/

    public override void 销毁形态()
    {
        if (生成协程 != null) StopCoroutine(生成协程);
        base.销毁形态();
    }
}