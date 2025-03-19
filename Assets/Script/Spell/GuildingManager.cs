using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 引导控制器 : MonoBehaviour
{
    private 法术基础类 当前引导法术;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            当前引导法术?.中断引导();
        }
    }

    public void 开始引导法术(法术基础类 法术)
    {
        if (法术.当前阶段 == 生命周期阶段.已释放)
        {
            当前引导法术 = 法术;
            法术.开始引导();
        }
    }
}
