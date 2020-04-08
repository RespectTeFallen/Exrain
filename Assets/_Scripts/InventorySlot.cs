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
    
    private Vector3 lastMousePos;
    
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
            Inventory.instance.drawDataField(item.itemName, item.itemDesc, item.itemData);
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
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Inventory.instance.SwapItem(item);
                }
                else
                {
                    Inventory.instance.SelectItem(item);
                }
            }
            else
            {
                Inventory.instance.PlaceItem(item);
                Invoke("PointerEnter", 0.1f);
            }
        }
        else if(Inventory.instance.selected != null)
        {
            Inventory.instance.PlaceItem(item);
            Invoke("PointerEnter", 0.1f);
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
