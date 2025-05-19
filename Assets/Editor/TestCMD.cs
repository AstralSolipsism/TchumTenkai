using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestCMD
{
    [MenuItem("TestCMD/读取表格")]
    public static void ReadTable()
    {
        PackageTable packageTable = Resources.Load<PackageTable>("Datas/PackageTableData/PackageTable");
        foreach (Item item in packageTable.dataList)
        {
            Debug.Log(string.Format("【id】:{0}，【name】:{1}", item.itemID, item.name));
        }
    }

    [MenuItem("TestCMD/打开背包界面")]
    public static void OpenPackagePanel()
    {
        UIManager.Instance.OpenPanel(UIConst.PackagePanel);
    }
}
