using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLogUI : MonoBehaviour
{
    public GameObject itemSlotPrefab;
    public GameObject itemContent;
    public void ShowItemLogUI()
    {
        this.gameObject.SetActive(true);
        var itemDict = AchievementManager.Instance.getItemNumDict;
        var currItemSlots = GetComponentsInChildren<ItemSlotUI>(true);
        int showNum = itemDict.Count;
        int needAddNum = showNum - currItemSlots.Length;
        for (int i = 0; i < needAddNum; ++i)
        {
            Instantiate(itemSlotPrefab, Vector3.zero, Quaternion.identity, itemContent.transform);
        }
        var newItemSlots = GetComponentsInChildren<ItemSlotUI>(true);
        int index = 0 ;
        foreach(var item in itemDict)
        {
            newItemSlots[index].gameObject.SetActive(true);
            var slot = newItemSlots[index];
            ++index;
            slot.SetContent(item.Key, item.Value);
        }
    }

    public void CloseItemLogUI()
    {
        var newItemSlots = GetComponentsInChildren<ItemSlotUI>(true);
        foreach(var itemSlot in newItemSlots)
        {
            itemSlot.gameObject.SetActive(false);
        }
        this.gameObject.SetActive(false);
    }
}
