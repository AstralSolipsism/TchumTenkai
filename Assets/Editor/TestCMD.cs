using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestCMD
{
    [MenuItem("TestCMD/��ȡ���")]
    public static void ReadTable()
    {
        PackageTable packageTable = Resources.Load<PackageTable>("Datas/PackageTableData/PackageTable");
        foreach (Item item in packageTable.dataList)
        {
            Debug.Log(string.Format("��id��:{0}����name��:{1}", item.itemID, item.name));
        }
    }

    [MenuItem("TestCMD/�򿪱�������")]
    public static void OpenPackagePanel()
    {
        UIManager.Instance.OpenPanel(UIConst.PackagePanel);
    }
}
