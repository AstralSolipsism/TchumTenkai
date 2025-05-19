using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageLocalData
{
    private static PackageLocalData instance;

    public static PackageLocalData Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new PackageLocalData();
            }
            return instance;
        }
    }

    public List<PackageLocalItem> items;

    public void SavePackage()
    {
        string inventoryJson = JsonUtility.ToJson(this);
        PlayerPrefs.SetString("PackageLocalData", inventoryJson);
        PlayerPrefs.Save();
    }

    public List<PackageLocalItem> LoadPackage()
    {
        if (items != null)
        {
            return items;
        }
        else if (PlayerPrefs.HasKey("PackageLocalData"))
        {
            string inventoryJson = PlayerPrefs.GetString("PackageLocalData");
            PackageLocalData packageLocalData = JsonUtility.FromJson<PackageLocalData>(inventoryJson);
            items = packageLocalData.items;
            return items;
        }
        else
        {
            items = new List<PackageLocalItem>();
            return items;
        }
    }
}

[System.Serializable]
public class PackageLocalItem
{
    public string uid;
    public int itemID;
    public int num;
    public bool isNew;
    public override string ToString()
    {
        return string.Format("[id]:{0},[num]:{1}", itemID, num);
    }
}