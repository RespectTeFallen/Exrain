using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("More than one instance of Inventory found!");
            return;
        }
        instance = this;
    }
    #endregion

    //Inventory.instance.AddItem("nearby", new Item("ammo", 3, "gun ammo", 10, Item.ItemType.Equipment));

    public List<Item> inventory = new List<Item>();
    public List<Item> nearby = new List<Item>();
    public static bool updateInventory = false;

    public Image selectedImage;
    public GameObject dataField;
    public Item selected;
    private InventorySlot lastIndex;
    private InventorySlot lastInv;

    private ItemDatabase database;

    public Transform inventorySlotsParent;
    InventorySlot[] inventorySlots;
    private int invLength;
    public Transform nearbySlotsParent;
    InventorySlot[] nearbySlots;
    public int nearLength;

    void Start()
    {
        instance = this;
        ItemDatabase.instance.AddItems();
        selected = null;

        inventorySlots = inventorySlotsParent.GetComponentsInChildren<InventorySlot>();
        invLength = inventorySlots.Length;
        nearbySlots = nearbySlotsParent.GetComponentsInChildren<InventorySlot>();
        nearLength = nearbySlots.Length;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventory.Add(new Item("empty", 0, "", 0, "", Item.ItemType.Null));
        }
        for (int i = 0; i < nearbySlots.Length; i++)
        {
            nearby.Add(new Item("empty", 0, "", 0, "", Item.ItemType.Null));
        }
        database = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>();
        for (int i = 0; i < database.items.Count; i++)
        {
            inventory[i] = database.items[i];
        }
    }

    public void AddItem(string name, Item item)
    {
        if (name == "inventory")
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].itemID == item.itemID)
                {
                    inventory[i] = new Item(item.itemName, item.itemID, item.itemDesc, inventory[i].itemCount + item.itemCount, item.itemData, item.itemType);
                    return;
                }
            }
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].itemID == 0)
                {
                    inventory[i] = item;
                    return;
                }
            }
        }
        if (name == "nearby")
        {
            for (int i = 0; i < nearby.Count; i++)
            {
                if (nearby[i].itemID == item.itemID)
                {
                    nearby[i] = new Item(item.itemName, item.itemID, item.itemDesc, nearby[i].itemCount + item.itemCount, item.itemData, item.itemType);
                    return;
                }
            }
            for (int i = 0; i < nearby.Count; i++)
            {
                if (nearby[i].itemID == 0)
                {
                    nearby[i] = item;
                    return;
                }
            }
        }
    }

    void Update()
    {
        if (updateInventory)
        {
            DrawInventory();
        }
    }

    void DrawInventory()
    {
        for (int i = 0; i < invLength; i++)
        {
            inventorySlots[i].SetSlot(inventory[i]);
        }
        for (int i = 0; i < nearLength; i++)
        {
            nearbySlots[i].SetSlot(nearby[i]);
        }

        if (selected != null)
        {
            selectedImage.sprite = selected.icon;
            if (selected.itemCount > 1)
            {
                selectedImage.GetComponentInChildren<TextMeshProUGUI>().text = selected.itemCount.ToString();
            }
            else
            {
                selectedImage.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
            selectedImage.gameObject.SetActive(true);
            Vector2 pos = new Vector2(
                Input.mousePosition.x + 25,
                Input.mousePosition.y - 25
                );
            selectedImage.transform.position = pos;
        }
        else
        {
            selectedImage.gameObject.SetActive(false);
        }
    }

    public void RemoveItem(Item item)
    {
        if (inventory.Contains(item))
        {
            int index = inventory.IndexOf(item);
            inventory[index] = new Item("empty", 0, "", 0, "", Item.ItemType.Null);
        }
        else if (nearby.Contains(item))
        {
            int index = nearby.IndexOf(item);
            nearby[index] = new Item("empty", 0, "", 0, "", Item.ItemType.Null);
        }
    }

    public void SelectItem(Item item)
    {
        if (inventory.Contains(item))
        {
            lastIndex = inventorySlots[inventory.IndexOf(item)];
            selected = lastIndex.item;
            inventory[inventory.IndexOf(selected)] = new Item("empty", 0, "", 0, "", Item.ItemType.Null);
        }
        else if (nearby.Contains(item))
        {
            lastIndex = nearbySlots[nearby.IndexOf(item)];
            selected = lastIndex.item;
            nearby[nearby.IndexOf(selected)] = new Item("empty", 0, "", 0, "", Item.ItemType.Null);
        }
    }

    public void SwapItem(Item item)
    {
        if (inventory.Contains(item))
        {
            for (int i = 0; i < nearby.Count; i++)
            {
                if (nearby[i].itemID == 0 || nearby[i].itemID == item.itemID)
                {
                    if (nearby[i].itemID == item.itemID)
                    {
                        nearby[i] = new Item(item.itemName, item.itemID, item.itemDesc, nearby[i].itemCount + item.itemCount, item.itemData, item.itemType);
                        break;
                    }
                    else
                    {
                        nearby[i] = item;
                        break;
                    }
                }
            }
            inventory[inventory.IndexOf(item)] = new Item("empty", 0, "", 0, "", Item.ItemType.Null);
        }
        else if (nearby.Contains(item))
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].itemID == 0 || inventory[i].itemID == item.itemID)
                {
                    if (inventory[i].itemID == item.itemID)
                    {
                        inventory[i] = new Item(item.itemName, item.itemID, item.itemDesc, inventory[i].itemCount + item.itemCount, item.itemData, item.itemType);
                        break;
                    }
                    else
                    {
                        inventory[i] = item;
                        break;
                    }
                }
            }
            nearby[nearby.IndexOf(item)] = new Item("empty", 0, "", 0, "", Item.ItemType.Null);
        }
    }

    public void PlaceItem(Item item)
    {
        if (inventory.Contains(item))
        {
            int index = inventory.IndexOf(item);
            if (item.itemID == selected.itemID)
            {
                inventory[index] = new Item(item.itemName, item.itemID, item.itemDesc, item.itemCount + selected.itemCount, item.itemData, item.itemType);
                selected = null;
                lastIndex = null;
                return;
            }
            if (inventory[index].itemID != 0)
            {
                if (inventory.Contains(lastIndex.item))
                {
                    inventory[inventory.IndexOf(lastIndex.item)] = inventory[index];
                }
                if (nearby.Contains(lastIndex.item))
                {
                    nearby[nearby.IndexOf(lastIndex.item)] = inventory[index];
                }
                inventory[index] = selected;
                selected = null;
            }
            else
            {
                inventory[index] = selected;
                selected = null;
            }
        }
        else if (nearby.Contains(item))
        {
            int index = nearby.IndexOf(item);
            if (item.itemID == selected.itemID)
            {
                nearby[index] = new Item(item.itemName, item.itemID, item.itemDesc, item.itemCount + selected.itemCount, item.itemData, item.itemType);
                selected = null;
                lastIndex = null;
                return;
            }
            if (nearby[index].itemID != 0)
            {
                if (nearby.Contains(lastIndex.item))
                {
                    nearby[nearby.IndexOf(lastIndex.item)] = nearby[index];
                }
                if (inventory.Contains(lastIndex.item))
                {
                    inventory[inventory.IndexOf(lastIndex.item)] = nearby[index];
                }
                nearby[index] = selected;
                selected = null;
            }
            else
            {
                nearby[index] = selected;
                selected = null;
            }
        }
    }

    public void drawDataField(string ItemName, string Header, string Data)
    {
        dataField.GetComponentInChildren<TextMeshProUGUI>().text = 
            "" + ItemName +
            "\n" +
            "\n" + Header +
            "\n" + Data
            ;
    }
}
