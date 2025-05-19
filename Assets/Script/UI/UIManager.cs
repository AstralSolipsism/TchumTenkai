using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                instance = new UIManager();
            return instance;
        }
    }
    private UIManager()
    {
        InitDicts();
    }
    
    private Transform uiRoot;

    //路径配置字典
    private Dictionary<string, string> pathDict;
    //预制体缓存字典
    private Dictionary<string, GameObject> prefabDict;
    //已打开界面缓存字典
    private Dictionary<string, BasePanel> panelDict;

    public Transform UIRoot
    {
        get
        {
            if(uiRoot == null)
            {
                if (GameObject.Find("Canvas"))
                {
                    uiRoot = GameObject.Find("Canvas").transform;
                }
                else
                {
                    uiRoot = new GameObject("Canvas").transform;
                }
            }
            return uiRoot;
        }
    }

    private void InitDicts()
    {
        prefabDict = new Dictionary<string, GameObject>();
        panelDict = new Dictionary<string, BasePanel>();
        pathDict = new Dictionary<string, string>() 
        {
            { UIConst.PackagePanel,"Package/PackagePanel"},
        };
    }

    public BasePanel GetPanel(string name)
    {
        BasePanel panel = null;
        if (panelDict.TryGetValue(name, out panel))
            return panel;
        return null;
    }

    public BasePanel OpenPanel(string name)
    {
        //检测是否已经打开
        BasePanel panel = null;
        if (panelDict.TryGetValue(name, out panel))
        {
            Debug.Log("界面已打开");
            return null;
        }
        //检测路径是否合法
        string path = "";
        if(!pathDict.TryGetValue(name,out path))
        {
            Debug.LogError("路径错误:" + name);
            return null;
        }
        //缓存预制体
        GameObject panelPrefab = null;
        if(!prefabDict.TryGetValue(name,out panelPrefab))
        {
            string realPath = "Prefab/Panel/" + path;
            panelPrefab = Resources.Load<GameObject>(realPath) as GameObject;
            prefabDict.Add(name,panelPrefab);
        }
        //打开界面
        GameObject panelObj = GameObject.Instantiate(panelPrefab, UIRoot, false);
        panel = panelObj.GetComponent<BasePanel>();
        panelDict.Add(name, panel);
        return panel;
    }

    public bool ClosePanel(string name)
    {
        BasePanel panel = null;
        if(!panelDict.TryGetValue(name,out panel))
        {
            Debug.LogError("界面未打开");
            return false;
        }
        panel.ClosePanel();
        if (panelDict.ContainsKey(name))
        {
            panelDict.Remove(name);
        }
        return true;
    }
}

public class UIConst
{
    public const string PackagePanel = "PackagePanel";
}