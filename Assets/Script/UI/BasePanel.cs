using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    protected bool isRemove = false;
    protected new string name;

    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    //����panel
    public virtual void OpenPanel(string name)
    {
        this.name = name;
        SetActive(true);
    }
    //�ر�panel
    public virtual void ClosePanel()
    {
        isRemove = true;
        SetActive(false);
        Destroy(gameObject);
    }
}
