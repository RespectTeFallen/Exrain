using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public int itemID;
    public string itemDesc;
    public int itemCount;
    public bool itemStackable;
    public string itemData;
    public Sprite icon;
    public ItemType itemType;

    public enum ItemType
    {
        Item,
        Attachment,
        Equipment,
        Null
    }

    public Item(string name, int id, string desc, int count, bool stackable, string data, ItemType type)
    {
        itemName = name;
        itemID = id;
        itemDesc = desc;
        itemCount = count;
        itemStackable = stackable;
        itemData = data;
        icon = Resources.Load<Sprite>("ItemIcons/" + itemName);
        itemType = type;
    }
}
