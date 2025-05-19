using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PackagePanel : BasePanel
{
    private Transform UIMenu;

    private Transform UIMenuEquipment;

    private Transform UIMenuSpell;

    private Transform UIMenuMaterial;

    private Transform UITabName;

    private Transform UICloseBtn;

    private Transform UICenter;

    private Transform UIScrolView;

    private Transform UIDetailPanel;

    private Transform UIDeletePanel;

    private Transform UIDeleteBackBtn;

    //private Transform UIDeleteInfoText;

    private Transform UIDeleteConfirmBtn;

    private Transform UIBottomMenus;

    private Transform UIDeleteBtn;

    //private Transform UIDetailBtn;
    private ItemType packagePanelType;

    public GameObject PackageUIItemPrefab;

    //记录鼠标选中哪个物品
    private string _chooseUID;

    public string chooseUID
    {
        get
        {
            return _chooseUID;
        }
        set
        {
            _chooseUID = value;
            RefreshDetail();
        }
    }

    public List<string> deleteChooseUID;

    public PackageMode curMode;

    public void AddDeleteChooseUID(string uid)
    {
        this.deleteChooseUID ??= new List<string>();
        if (!this.deleteChooseUID.Contains(uid))
        {
            this.deleteChooseUID.Add(uid);
        }
        else
        {
            this.deleteChooseUID.Remove(uid);
        }
        RefreshDeletePanel();
    }

    private void RefreshDeletePanel()
    {
        RectTransform scrollContent = UIScrolView.GetComponent<ScrollRect>().content;
        foreach(Transform cell in scrollContent)
        {
            PackageCell packageCell = cell.GetComponent<PackageCell>();
            //物品刷新选中状态
            packageCell.RefreshDeleteState();
        }
    }

    private void Awake()
    {
        InitUI();
    }
    private void Start()
    {
        packagePanelType = ItemType.Equipment;
        RefreshUI();
    }

    private void RefreshUI()
    {
        RefreshScroll();
    }
    private void RefreshDetail()
    {
        PackageLocalItem localItem = PackageManager.Instance.GetPackLocalItemByUID(chooseUID);
        UIDetailPanel.GetComponent<PackageDetail>().Refresh(localItem, this);
    }

    private void RefreshScroll()
    {
        RectTransform scrollContent = UIScrolView.GetComponent<ScrollRect>().content;
        for(int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }
        foreach(PackageLocalItem localData in PackageManager.Instance.GetSortPackageLocalData())
        {
            if(PackageManager.Instance.GetItemByID(localData.itemID).type == packagePanelType)
            {
            Transform PackageUIItem = Instantiate(PackageUIItemPrefab.transform, scrollContent) as Transform;
            PackageCell packageCell = PackageUIItem.GetComponent<PackageCell>();
            packageCell.Refresh(localData, this);
            }
        }
    }

    private void InitUI()
    {
        InitUIName();
        InitClick();

    }

    private void InitUIName()
    {
        UIMenu = transform.Find("TopCenter/Menus");
        UIMenuEquipment = transform.Find("TopCenter/Menus/Equipment");
        UIMenuSpell = transform.Find("TopCenter/Menus/Spell");
        UIMenuMaterial = transform.Find("TopCenter/Menus/Material");
        UITabName = transform.Find("LeftTop/TabName");
        UICloseBtn = transform.Find("RightTop/CloseBtn");
        UICenter = transform.Find("Center");
        UIScrolView = transform.Find("Center/ScrollView");
        UIDetailPanel = transform.Find("Center/DetailPanel");
        UIDeletePanel = transform.Find("Bottom/DeletePanel");
        UIDeleteBackBtn = transform.Find("Bottom/DeletePanel/DeleteBackBtn");
        UIDeleteConfirmBtn = transform.Find("Bottom/DeletePanel/DeleteConfirmBtn");
        UIBottomMenus = transform.Find("Bottom/BottomMenus");
        UIDeleteBtn = transform.Find("Bottom/BottomMenus/DeleteBtn");

        UIDeletePanel.gameObject.SetActive(false);
        UIBottomMenus.gameObject.SetActive(true);
    }
    private void InitClick()
    {
        UIMenuEquipment.GetComponent<Button>().onClick.AddListener(OnclickEquipment);
        UIMenuSpell.GetComponent<Button>().onClick.AddListener(OnclickSpell);
        UIMenuMaterial.GetComponent<Button>().onClick.AddListener(OnclickMaterial);

        UIDeleteBackBtn.GetComponent<Button>().onClick.AddListener(OnclickDeleteBackBtn);
        UIDeleteConfirmBtn.GetComponent<Button>().onClick.AddListener(OnclickDeleteConfirmBtn);
        UICloseBtn.GetComponent<Button>().onClick.AddListener(OnclickCloseBtn);
        UIDeleteBtn.GetComponent<Button>().onClick.AddListener(OnclickDeleteBtn);

    }

    private void OnclickDeleteBtn()
    {
        curMode = PackageMode.delete;
        UIDeletePanel.gameObject.SetActive(true);
    }

    private void OnclickCloseBtn()
    {
        UIManager.Instance.ClosePanel(UIConst.PackagePanel);
    }

    private void OnclickDeleteConfirmBtn()
    {
        if (this.deleteChooseUID == null || this.deleteChooseUID.Count == 0) return;
        PackageManager.Instance.DeletePackageItems(deleteChooseUID);
        RefreshUI();
    }

    private void OnclickDeleteBackBtn()
    {
        curMode = PackageMode.normal;
        UIDeletePanel.gameObject.SetActive(false);
        deleteChooseUID = new List<string>();
        RefreshDeletePanel();
    }

    private void OnclickMaterial()
    {
        packagePanelType = ItemType.Material;
        RefreshUI();
    }

    private void OnclickSpell()
    {
        packagePanelType = ItemType.Spell;
        RefreshUI();
    }

    private void OnclickEquipment()
    {
        packagePanelType = ItemType.Equipment; 
        RefreshUI();
    }
}

public enum PackageMode
{
    normal,
    delete
}