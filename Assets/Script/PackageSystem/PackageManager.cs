using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageManager : MonoBehaviour
{
    private static PackageManager instance;
    public static PackageManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<PackageManager>();
                if(instance == null)
                {
                    GameObject obj = new GameObject(typeof(PackageManager).Name);
                    instance = obj.AddComponent<PackageManager>();
                }
            }
            return instance;
        }
    }

    private PackageTable packageTable;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        UIManager.Instance.OpenPanel(UIConst.PackagePanel);
    }
    //��þ�̬�������ݱ�
    public PackageTable GetPackageTable()
    {
        if(packageTable == null)
        {
            packageTable = Resources.Load<PackageTable>("Datas/PackageTableData/PackageTable");
        }

        return packageTable;
    }
    //��ȡ��̬��������
    public List<PackageLocalItem> GetPackageLocalData()
    {
        return PackageLocalData.Instance.LoadPackage();
    }
    //����ID�Ӿ�̬���߱��ȡ��̬��������
    public Item GetItemByID(int id)
    {
        List<Item> packageDataList = GetPackageTable().dataList;
        foreach(Item item in packageDataList)
        {
            if (item.itemID == id) return item;
        }
        return null;
    }
    //����UID�Ӷ�̬���߱��л�ȡ����
    public PackageLocalItem GetPackLocalItemByUID(string uid)
    {
        List<PackageLocalItem> packageDataList = GetPackageLocalData();
        foreach (PackageLocalItem item in packageDataList)
        {
            if (item.uid == uid) return item;
        }
        return null;
    }
    //��������Ķ�̬��������
    public List<PackageLocalItem> GetSortPackageLocalData()
    {
        List<PackageLocalItem> localItems = PackageLocalData.Instance.LoadPackage();
        localItems.Sort(new PackItemComparer());
        return localItems;
    }
    //����itemID��ӵ���
    public void AddItemToPackageLocalData(int id, int nums)
    {
        Item item = GetItemByID(id);
        PackageLocalItem packageLocalItem = new()
        {
            uid = System.Guid.NewGuid().ToString(),
            itemID = item.itemID,
            num = nums,
            isNew = false
        };
        PackageLocalData.Instance.items.Add(packageLocalItem);
        PackageLocalData.Instance.SavePackage();
    }
    public void AddItemToPackageLocalData(int id)
    {
        AddItemToPackageLocalData(id, 1);
    }
    //����UIDɾ������
    public void DeletePackageItems(string uid,bool needSave = true)
    {
        PackageLocalItem packageLocalItem = GetPackLocalItemByUID(uid);
        if (packageLocalItem == null) 
            return;
        PackageLocalData.Instance.items.Remove(packageLocalItem);
        if (needSave)
        {
            PackageLocalData.Instance.SavePackage();
        }
    }
    public void DeletePackageItems(List<string> uids)
    {
        foreach(string uid in uids)
        {
            DeletePackageItems(uid, false);
        }
        PackageLocalData.Instance.SavePackage();
    }
}

public  class PackItemComparer : IComparer<PackageLocalItem>
{
    public int Compare(PackageLocalItem a,PackageLocalItem b)
    {
        Item x = PackageManager.Instance.GetItemByID(a.itemID);
        Item y = PackageManager.Instance.GetItemByID(b.itemID);

        return y.itemID.CompareTo(x.itemID);
    }
}