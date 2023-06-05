using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Inventory;

namespace MFarm.Inventory
{
    public class ItemPickUp : MonoBehaviour
    {
        private void OnTriggerStay2D(Collider2D other)
        {
            Item item = other.GetComponent<Item>();

            if (item != null)
            {
                if (item.itemDetails.canPickedup)
                {
                    if (InventoryManager.Instance.CanAddItem(item.itemID))
                    {
                        //拾取物品添加到背包
                        InventoryManager.Instance.AddItem(item, true);
                        //播放音效
                        EventHandler.CallPlaySoundEvent(SoundName.Pickup);
                    }
                    else
                    {
                        UIManager.Instance.GoTo(12);
                    }
                }
                
            }
        }
    }
}