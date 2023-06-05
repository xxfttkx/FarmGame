using System;
using UnityEngine;
using UnityEngine.UI;

namespace MFarm.Inventory
{
    public class TradeUI : MonoBehaviour
    {
        public Image itemIcon;
        public Text itemName;
        public Text BuyOrSellText;
        public InputField tradeAmount;
        public Button addButton;
        public Button reduceButton;
        public Button submitButton;
        public Button cancelButton;

        private ItemDetails item;
        private bool isSellTrade;

        private void Awake()
        {
            addButton.onClick.AddListener(AddTradeAmount);
            reduceButton.onClick.AddListener(ReduceTradeAmount);
            cancelButton.onClick.AddListener(CancelTrade);
            submitButton.onClick.AddListener(TradeItem);
        }
        private void OnEnable()
        {
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
        }
        private void OnDisable()
        {
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
        }
        private void OnBaseBagCloseEvent(SlotType type, InventoryBag_SO sO)
        {
            CancelTrade();
        }


        /// <summary>
        /// 设置TradeUI显示详情
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isSell"></param>
        public void SetupTradeUI(ItemDetails item, bool isSell)
        {
            this.item = item;
            itemIcon.sprite = item.itemIcon;
            itemName.text = item.itemName;
            //TODO 建个字典表
            if(isSell)
            {
                BuyOrSellText.text = "你想卖几个？";
            }
            else
            {
                BuyOrSellText.text = "你想买几个？";
            }
            isSellTrade = isSell;
            tradeAmount.text = 1.ToString();
        }

        private void TradeItem()
        {
            if (tradeAmount.text == "") return;
            var amount = Convert.ToInt32(tradeAmount.text);
            if (amount <= 0) return;
            InventoryManager.Instance.TradeItem(item, amount, isSellTrade);
            CancelTrade();
        }


        private void CancelTrade()
        {
            this.gameObject.SetActive(false);
        }

        void AddTradeAmount()
        {
            if (tradeAmount.text == "")
            {
                tradeAmount.text = "1";
                return;
            }
            var amount = Convert.ToInt32(tradeAmount.text);
            amount++;
            tradeAmount.text = amount.ToString();
        }
        void ReduceTradeAmount()
        {
            if (tradeAmount.text == "")
            {
                return;
            }
            var amount = Convert.ToInt32(tradeAmount.text);
            if (amount > 1)
            {
                amount--;
                tradeAmount.text = amount.ToString();
            }
        }
    }
}
