using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PackageCell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Transform UIIcon;
    private Transform UIName;
    private Transform UINew;
    private Transform UISelect;
    private Transform UIDeleteSelect;
    private Transform UISelectAnimation;
    private Transform UIMouseOverAnimation;

    private PackageLocalItem packageLocalData;
    private Item item;
    private PackagePanel uiParent;

    private void Awake()
    {
        InitUIName();
        UIDeleteSelect.gameObject.SetActive(false);
        UISelectAnimation.gameObject.SetActive(false);
        UIMouseOverAnimation.gameObject.SetActive(false);
    }

    private void InitUIName()
    {
        UIIcon = transform.Find("Top/Icon");
        UINew = transform.Find("Top/New");
        UIName = transform.Find("Bottom/Name");
        UISelect = transform.Find("Select");
        UIDeleteSelect = transform.Find("DeleteSelect");
        UISelectAnimation = transform.Find("SelectAnimation");
        UIMouseOverAnimation = transform.Find("MouseOverAnimation");
    }

    public void Refresh(PackageLocalItem packageLocalData,PackagePanel uiParent)
    {
        //数据初始化
        this.packageLocalData = packageLocalData;
        this.item = PackageManager.Instance.GetItemByID(packageLocalData.itemID);
        this.uiParent = uiParent;
        //物品名字
        UIName.GetComponent<Text>().text = item.name;
        //是否新获得
        UINew.gameObject.SetActive(this.packageLocalData.isNew);
        //物品的图片
        Texture2D t = (Texture2D)Resources.Load(this.item.imagePath);
        Sprite temp = Sprite.Create(t, new Rect(0, 0, t.width, t.height),new Vector2(0,0));
        UIIcon.GetComponent<Image>().sprite = temp;
    }

    public void RefreshDeleteState()
    {
        if (this.uiParent.deleteChooseUID.Contains(this.packageLocalData.uid))
        {
            this.UIDeleteSelect.gameObject.SetActive(true);
        }
        else
        {
            this.UIDeleteSelect.gameObject.SetActive(false);
        }
    }
    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if(this.uiParent.curMode == PackageMode.delete)
        {
            this.uiParent.AddDeleteChooseUID(this.packageLocalData.uid);
        }
        if (this.uiParent.chooseUID == this.packageLocalData.uid)
            return;
        else
            this.uiParent.chooseUID = this.packageLocalData.uid;
        UISelectAnimation.gameObject.SetActive(true);
        UISelectAnimation.GetComponent<Animator>().SetTrigger("Selected");
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        UIMouseOverAnimation.gameObject.SetActive(true);
        UIMouseOverAnimation.GetComponent<Animator>().SetTrigger("MouseIn");
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        UIMouseOverAnimation.GetComponent<Animator>().SetTrigger("MouseOut");
    }

}
