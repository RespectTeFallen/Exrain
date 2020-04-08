using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{

    #region Singleton
    public static ItemDatabase instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one instance of ItemDatabase found!");
            return;
        }
        instance = this;
    }

    #endregion

    public List<Item> items = new List<Item>();

    public Dictionary<string, Item> itemList = new Dictionary<string, Item>();

    public void AddItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            itemList.Add(items[i].itemName, items[i]);
        }
    }

}
