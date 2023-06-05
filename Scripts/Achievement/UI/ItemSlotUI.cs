using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MFarm.Inventory;

public class ItemSlotUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Image image;
    public Text text;

    public void SetContent(int itemId, int num)
    {
        var itemDetails = InventoryManager.Instance.GetItemDetails(itemId);
        image.sprite = itemDetails.itemIcon;
        text.text = num.ToString();
    }
}
