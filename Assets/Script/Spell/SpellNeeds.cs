// 成分类型基础架构
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum 成分类型
{
    言语,
    姿势,
    材料,
    目标选择,
    自定义条件
}

// 成分基类
public abstract class 法术成分
{
    public 成分类型 类型 { get; protected set; }
    public int 需求数量 { get; protected set; }
    public int 当前进度 { get; protected set; }
    public bool 已完成 => 当前进度 >= 需求数量;

    public virtual void 提交进度(int 增量 = 1)
    {
        当前进度 = Mathf.Min(当前进度 + 增量, 需求数量);
    }

    public abstract bool 验证条件(object 输入参数);
}

// 具体成分实现
public class 言语成分 : 法术成分
{
    public enum 言语类型 { 咒文1, 咒文2, 咒文3,咒文4,咒文5, 咒文6, 咒文7,咒文8,咒文9, 咒文10 }
    public 言语类型 具体类型 { get; private set; }

    public 言语成分(言语类型 类型, int 需求节数)
    {
        this.类型 = 成分类型.言语;
        具体类型 = 类型;
        需求数量 = 需求节数;
    }

    public override bool 验证条件(object 输入参数)
    {
        // 验证输入是否为匹配的言语类型
        return 输入参数 is 言语类型 type && type == 具体类型;
    }
}

public class 姿势成分 : 法术成分
{
    public enum 姿势类型 { 姿势1, 姿势2, 姿势3,姿势4,姿势5,姿势6,姿势7,姿势8,姿势9,姿势10 }
    public 姿势类型 具体类型 { get; private set; }

    public 姿势成分(姿势类型 类型, int 需求次数)
    {
        this.类型 = 成分类型.姿势;
        具体类型 = 类型;
        需求数量 = 需求次数;
    }

    public override bool 验证条件(object 输入参数)
    {
        return 输入参数 is 姿势类型 type && type == 具体类型;
    }
}

public class 目标成分 : 法术成分
{
    public enum 目标类型 { 单位, 位置, 法术实例 }
    public 目标类型 需求类型 { get; private set; }

    public object 选定目标 { get; private set; }

    public 目标成分(目标类型 类型)
    {
        this.类型 = 成分类型.目标选择;
        需求类型 = 类型;
        需求数量 = 1; // 目标选择只需一次
    }

    public override bool 验证条件(object 输入参数)
    {
        switch (需求类型)
        {
            case 目标类型.单位:
                return 输入参数 is GameObject obj && obj.GetComponent<Character>();
            case 目标类型.位置:
                return 输入参数 is Vector3;
            case 目标类型.法术实例:
                return 输入参数 is 法术基础类;
            default:
                return false;
        }
    }

    public override void 提交进度(int 增量 = 1)
    {
        if (验证条件(增量)) // 此处增量参数实际是目标信息
        {
            选定目标 = 增量;
            base.提交进度();
        }
    }
}

public class 自定义成分 : 法术成分
{
    public System.Func<bool> 动态条件 { get; private set; }

    public 自定义成分(System.Func<bool> 条件检查器)
    {
        类型 = 成分类型.自定义条件;
        需求数量 = 1;
        动态条件 = 条件检查器;
    }

    public override bool 验证条件(object 输入参数)
    {
        return 动态条件?.Invoke() ?? false;
    }
}



// 领域单元的成分提供接口
public interface I成分提供者
{
    bool 尝试提供成分(成分类型 类型, out object 参数);
    void 开始提供(法术基础类 法术);
    void 终止提供(法术基础类 法术);
}

