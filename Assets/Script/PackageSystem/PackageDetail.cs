using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PackageDetail : MonoBehaviour
{
    private Transform UITitle;
    private Transform UIIcon;
    private Transform UIName;
    private Transform UIDescription;

    private PackageLocalItem packageLocalData;
    private Item item;
    private PackagePanel uiParent;

    private void Awake()
    {
        InitUIName();
    }

    private void InitUIName()
    {
        UITitle = transform.Find("Top/Title");
        UIIcon = transform.Find("Center/Icon");
        UIName = transform.Find("Center/Name");
        UIDescription = transform.Find("Bottom/Description");
    }

    public void Refresh(PackageLocalItem packageLocalData,PackagePanel uiParent)
    {
        this.packageLocalData = packageLocalData;
        item = PackageManager.Instance.GetItemByID(packageLocalData.itemID);
        this.uiParent = uiParent;
        //���
        switch (item.type)
        {
            case ItemType.Equipment:
                UITitle.GetComponent<Text>().text = "����";
                break;
            case ItemType.Spell:
                UITitle.GetComponent<Text>().text = "����";
                break;
            case ItemType.Material:
                UITitle.GetComponent<Text>().text = "����";
                break;
        }
        //����
        UIName.GetComponent<Text>().text = this.item.name;
        //��Ʒ��ͼƬ
        Texture2D t = (Texture2D)Resources.Load(this.item.imagePath);
        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0, 0));
        UIIcon.GetComponent<Image>().sprite = temp;
        //��ϸ����
        UIDescription.GetComponent<Text>().text = this.item.description;
    }
}
