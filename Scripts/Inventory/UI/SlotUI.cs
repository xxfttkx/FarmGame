using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        [Header("组件获取")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        public Image slotHightlight;
        [SerializeField] private Button button;

        [Header("格子类型")]
        public SlotType slotType;
        public bool isSelected;
        public int slotIndex;

        //物品信息
        public ItemDetails itemDetails;
        public int itemAmount;

        public bool isMoving = false;

        public InventoryLocation Location
        {
            get
            {
                return slotType switch
                {
                    SlotType.Bag => InventoryLocation.Player,
                    SlotType.Box => InventoryLocation.Box,
                    _ => InventoryLocation.Player
                };
            }
        }

        public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        private void Start()
        {
            isSelected = false;
            if (itemDetails == null)
            {
                UpdateEmptySlot();
            }
        }

        private void Update()
        {
            
        }

        /// <summary>
        /// 更新格子UI和信息
        /// </summary>
        /// <param name="item">ItemDetails</param>
        /// <param name="amount">持有数量</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            itemAmount = amount;
            amountText.text = amount.ToString();
            slotImage.enabled = true;
            button.interactable = true;
        }

        /// <summary>
        /// 将Slot更新为空
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;

                inventoryUI.UpdateSlotHighlight(-1);
                if (slotIndex < 9)
                {
                    ActionBarUI.Instance.currIndex = isSelected ? slotIndex : -1;
                }else
                {
                    ActionBarUI.Instance.currIndex = -1;
                }
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            itemDetails = null;
            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if(!inventoryUI.bagUI.activeInHierarchy)return;
            if(inventoryUI.isMovingItem)
            {
                //鼠标左键
                if (eventData.button == PointerEventData.InputButton.Left)
                {
                    if (slotType == SlotType.Bag)
                    {
                        //背包内交换
                        if (inventoryUI.fromSlot.slotType == SlotType.Bag)
                        {
                            InventoryManager.Instance.
                                SwapItem(inventoryUI.fromSlot.slotIndex, slotIndex, inventoryUI.swapItemNum);
                        }
                        //买
                        else if (inventoryUI.fromSlot.slotType == SlotType.Shop)
                        {
                            EventHandler.CallShowTradeUI(inventoryUI.fromSlot.itemDetails, false);
                        }
                        //从箱子取出
                        else if (inventoryUI.fromSlot.slotType == SlotType.Box)
                        {
                            //跨背包数据交换物品
                            InventoryManager.Instance.
                                SwapItem(inventoryUI.fromSlot.Location, inventoryUI.fromSlot.slotIndex, 
                                Location, slotIndex);
                        }
                    }
                    else if(slotType == SlotType.Shop)
                    {
                        //卖
                        if (inventoryUI.fromSlot.slotType == SlotType.Bag)
                        {
                            EventHandler.CallShowTradeUI
                                (inventoryUI.fromSlot.itemDetails, true);
                        }
                    }
                    else if(slotType == SlotType.Box)
                    {
                        //背包进箱子
                        if (inventoryUI.fromSlot.slotType == SlotType.Bag)
                        {
                            //跨背包数据交换物品
                            InventoryManager.Instance.
                                SwapItem(inventoryUI.fromSlot.Location, inventoryUI.fromSlot.slotIndex, 
                                Location, slotIndex);
                        }
                        //箱子内交换
                        else if (inventoryUI.fromSlot.slotType == SlotType.Box)
                        {
                            //TODO 拖拽也要加
                            //跨背包数据交换物品
                            //TODO 好像这样也能 看看要不要改
                            InventoryManager.Instance.
                                SwapItem(inventoryUI.fromSlot.Location, inventoryUI.fromSlot.slotIndex, 
                                Location, slotIndex);
                        }
                    }
                    EndDragItem();
                    //清空所有高亮显示
                    inventoryUI.UpdateSlotHighlight(-1);
                }
                else if (eventData.button == PointerEventData.InputButton.Right)
                {
                    if (this == inventoryUI.fromSlot)
                    {
                        if (inventoryUI.swapItemNum < itemAmount)
                        {
                            inventoryUI.swapItemNum += 1;
                            inventoryUI.dragItemUI.SetNumText(inventoryUI.swapItemNum);
                        }
                    }
                }
            }
            else{
                if(eventData.button == PointerEventData.InputButton.Right)
                {
                    inventoryUI.fromSlot = this;
                    inventoryUI.swapItemNum = 1;
                    // Debug.Log("from"+this.name);
                    BeginDragItem();
                }
            }
        }



        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null) return;
            // if (InventoryManager.Instance.playerBag.itemList[slotIndex].isEmpty()) return;
            isSelected = !isSelected;

            inventoryUI.UpdateSlotHighlight(slotIndex);

            if (slotType == SlotType.Bag)
            {
                //通知物品被选中的状态和信息
                if (slotIndex < 9)
                {
                    ActionBarUI.Instance.currIndex = isSelected ? slotIndex : -1;
                }else
                {
                    ActionBarUI.Instance.currIndex = -1;
                }
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemDetails == null) return;
            if (itemAmount <= 0) return;

            BeginDragItem(itemAmount);
            isSelected = true;
            inventoryUI.UpdateSlotHighlight(slotIndex);

        }

        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItemUI.MoveByMouse();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItemUI.CloseDragItemImage();
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                if (targetSlot == null)
                    return;
                int targetIndex = targetSlot.slotIndex;

                //在Player自身背包范围内交换
                if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }
                else if (slotType == SlotType.Shop && targetSlot.slotType == SlotType.Bag)  //买
                {
                    EventHandler.CallShowTradeUI(itemDetails, false);
                }
                else if (slotType == SlotType.Bag && targetSlot.slotType == SlotType.Shop)  //卖
                {
                    EventHandler.CallShowTradeUI(itemDetails, true);
                }
                else if (slotType != SlotType.Shop && targetSlot.slotType != SlotType.Shop && slotType != targetSlot.slotType)
                {
                    //跨背包数据交换物品
                    InventoryManager.Instance.SwapItem(Location, slotIndex, targetSlot.Location, targetSlot.slotIndex);
                }
                //清空所有高亮显示
                inventoryUI.UpdateSlotHighlight(-1);
            }
            // else    //测试扔在地上
            // {
            //     if (itemDetails.canDropped)
            //     {
            //         //鼠标对应世界地图坐标
            //         var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

            //         EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            //     }
            // }
        }

        void BeginDragItem(int num = 1)
        {
            if (itemDetails == null || itemDetails.itemID == 0 || itemAmount < 1) return;
            inventoryUI.dragItemUI.ShowDragItemImage(slotImage.sprite, num);
        }
        void EndDragItem()
        {
            inventoryUI.dragItemUI.CloseDragItemImage();
        }

    }
}