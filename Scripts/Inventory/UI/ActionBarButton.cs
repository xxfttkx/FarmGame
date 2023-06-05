using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;
        private SlotUI slotUI;
        private bool canUse = true;


        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }

        private void OnEnable()
        {
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        }

        private void OnDisable()
        {
            EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        }

        private void OnUpdateGameStateEvent(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Gameplay:
                    canUse = true;
                    break;
                case GameState.Pause:
                    canUse = false;
                    break;
                case GameState.GameEnd:
                    canUse = false;
                    break;
                default: break;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(key) && canUse)
            {
                if (slotUI.itemDetails != null)
                {
                    slotUI.isSelected = !slotUI.isSelected;
                    if (slotUI.isSelected)
                        slotUI.inventoryUI.UpdateSlotHighlight(slotUI.slotIndex);
                    else
                        slotUI.inventoryUI.UpdateSlotHighlight(-1);

                    EventHandler.CallItemSelectedEvent(slotUI.itemDetails, slotUI.isSelected);
                }
            }
        }
    }
}