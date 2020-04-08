using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lootable : MonoBehaviour
{

    public List<Item> loot = new List<Item>();

    public string lootName;
    public int lootID;

    private GameObject LootObject;

    void Start()
    {
        LootObject = GetComponent<GameObject>();
        
    }

}
