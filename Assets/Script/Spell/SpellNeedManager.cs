using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 施法者成分处理器 : MonoBehaviour
{
    public float 施法速度 = 1.0f; // 影响成分完成速度

    private 法术基础类 当前处理法术;
    private Dictionary<成分类型, object> 当前输入缓存 = new();

    void Update()
    {
        if (当前处理法术 != null)
        {
            // 自动处理持续型成分
            处理持续成分(成分类型.言语);
            处理持续成分(成分类型.姿势);

            // 处理需要手动输入的成分
            if (Input.GetKeyDown(KeyCode.T))
            {
                var 目标 = 选择目标();
                当前处理法术.尝试提交成分(成分类型.目标选择, 目标);
            }
        }
    }

    private GameObject 选择目标()//简易临时实现
    {        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private void 处理持续成分(成分类型 类型)
    {
        if (当前输入缓存.TryGetValue(类型, out var 参数))
        {
            float 增量 = 施法速度 * Time.deltaTime;
            当前处理法术.尝试提交成分(类型, (float)参数 * 增量);
        }
    }

    public void 开始处理法术(法术基础类 法术)
    {
        当前处理法术 = 法术;
        // 初始化输入缓存
        foreach (var 类型 in 法术.获取需求成分类型())
        {
            当前输入缓存[类型] = 获取默认输入值(类型);
        }
    }

    private object 获取默认输入值(成分类型 类型)
    {
        switch (类型)
        {
            case 成分类型.言语: return 1.0f; // 默认语速
            case 成分类型.姿势: return 1.0f; // 默认姿势速度
            default: return null;
        }
    }
}
