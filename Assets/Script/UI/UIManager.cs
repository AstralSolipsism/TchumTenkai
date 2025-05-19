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

    //·�������ֵ�
    private Dictionary<string, string> pathDict;
    //Ԥ���建���ֵ�
    private Dictionary<string, GameObject> prefabDict;
    //�Ѵ򿪽��滺���ֵ�
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
        //����Ƿ��Ѿ���
        BasePanel panel = null;
        if (panelDict.TryGetValue(name, out panel))
        {
            Debug.Log("�����Ѵ�");
            return null;
        }
        //���·���Ƿ�Ϸ�
        string path = "";
        if(!pathDict.TryGetValue(name,out path))
        {
            Debug.LogError("·������:" + name);
            return null;
        }
        //����Ԥ����
        GameObject panelPrefab = null;
        if(!prefabDict.TryGetValue(name,out panelPrefab))
        {
            string realPath = "Prefab/Panel/" + path;
            panelPrefab = Resources.Load<GameObject>(realPath) as GameObject;
            prefabDict.Add(name,panelPrefab);
        }
        //�򿪽���
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
            Debug.LogError("����δ��");
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