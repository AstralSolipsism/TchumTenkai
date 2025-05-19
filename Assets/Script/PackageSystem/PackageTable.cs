using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="PackageSystem/PackageTable",fileName ="PackageTable")]
public class PackageTable : ScriptableObject
{
    public List<Item> dataList = new List<Item>();
}
public enum ItemType
{
    Equipment,
    Spell,
    Material
}
[System.Serializable]
public class Item
{
    public string uID;
    public int itemID;
    public ItemType type;
    public string name;
    public string description;
    public string imagePath;
}
