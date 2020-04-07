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

    public void AddItems()
    {
        for (int i = 1; i <= 2; i++)
        {
            items.Add(new Item("test" + i, 1, "test item", 1, Item.ItemType.Item));
        }
    }

}
