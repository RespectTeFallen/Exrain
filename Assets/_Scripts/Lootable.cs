using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour
{

    public List<Item> loot = new List<Item>();

    public string lootName;
    public int lootID;
    public int lootCount;

    private GameObject LootObject;

    void Start()
    {
        LootObject = GetComponent<GameObject>();
        for (int i = 0; i < Random.Range(1, lootCount); i++)
        {
            loot.Add(ItemDatabase.instance.items[Random.Range(0, ItemDatabase.instance.items.Count)]);
        }
    }

}
