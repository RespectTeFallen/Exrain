using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{

    public Item item;
    public Button Slot;
    public Button Delete;
    public TextMeshProUGUI itemCount;
    public Image Icon;

    
    public void SetSlot(Item it)
    {
        item = it;
        Icon.sprite = item.icon;
        if (item.itemID != 0)
        {
            Delete.gameObject.SetActive(true);
        }
        else
        {
            Delete.gameObject.SetActive(false);
        }
        if (item.itemCount > 1)
        {
            itemCount.text = item.itemCount.ToString();
            itemCount.enabled = true;
        }
        else if (item.itemCount <= 1)
        {
            itemCount.text = "";
            itemCount.enabled = false;
        }
    }

    public void PointerEnter()
    {
        if (item.itemID != 0)
        {
            Inventory.instance.dataField.transform.position = new Vector2(transform.position.x + 20, transform.position.y - 20);
            Inventory.instance.dataField.GetComponentInChildren<TextMeshProUGUI>().text = item.itemName;
            Inventory.instance.dataField.SetActive(true);
        }
    }

    public void PointerExit()
    {
        Inventory.instance.dataField.SetActive(false);
    }

    public void Selected()
    {
        if (item.itemID != 0)
        {
            if (Inventory.instance.selected == null)
            {
                Inventory.instance.SelectItem(item);
            }
            else
            {
                Inventory.instance.PlaceItem(item);
            }
        }
        else if(Inventory.instance.selected != null)
        {
            Inventory.instance.PlaceItem(item);
        }
    }

    public void Remove()
    {
        if (item.itemID != 0)
        {
            Inventory.instance.RemoveItem(item);
        }
    }

}
