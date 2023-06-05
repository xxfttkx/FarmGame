using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Inventory;
public class ActionBarUI : Singleton<ActionBarUI>
{
    public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();
    public int currIndex = -1;
    public SlotUI[] slotUIs;
    public bool canInput = false;

    protected override void Awake()
    {
        base.Awake();
        slotUIs = GetComponentsInChildren<SlotUI>();
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0) return;
        SwitchSlot(scroll);
    }
    private void OnEnable()
    {
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
    }



    private void OnDisable()
    {
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Pause:
                canInput = false;
                break;
            case GameState.Gameplay:
                canInput = true;
                break;
            case GameState.GameEnd:
                canInput = false;
                currIndex = -1;
                break;
            default: break;
        }
    }

    private void SwitchSlot(float scroll)
    {
        if (scroll < 0)//向下滚动
        {
            int count = 10;
            while (count > 0)
            {
                --count;
                ++currIndex;
                if (currIndex >= 10) currIndex = 0;
                var tempItemDetails = slotUIs[currIndex].itemDetails;
                if (tempItemDetails != null && tempItemDetails.itemID != 0)
                    break;
            }
            var itemDetails = slotUIs[currIndex].itemDetails;
            if (itemDetails != null && itemDetails.itemID != 0)
            {
                slotUIs[currIndex].isSelected = true;
                inventoryUI.UpdateSlotHighlight(currIndex);
                EventHandler.CallItemSelectedEvent(itemDetails, true);
            }
        }
        else
        {
            int count = 10;
            while (count > 0)
            {
                --count;
                --currIndex;
                if (currIndex < 0) currIndex = 9;
                var tempItemDetails = slotUIs[currIndex].itemDetails;
                if (tempItemDetails != null && tempItemDetails.itemID != 0)
                    break;
            }
            var itemDetails = slotUIs[currIndex].itemDetails;
            if (itemDetails != null && itemDetails.itemID != 0)
            {
                slotUIs[currIndex].isSelected = true;
                inventoryUI.UpdateSlotHighlight(currIndex);
                EventHandler.CallItemSelectedEvent(itemDetails, true);
            }
        }
    }
    void OnAfterSceneLoadedEvent()
    {
        currIndex = -1;
    }
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            currIndex = -1;
        }
    }
}
