using System.Collections.Generic;
using MFarm.Save;
using UnityEngine;
namespace MFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>, ISaveable
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;
        [Header("建造蓝图")]
        public BluePrintDataList_SO bluePrintData;
        [Header("背包数据")]
        public InventoryBag_SO playerBagTemp;
        public InventoryBag_SO playerBag;
        private InventoryBag_SO currentBoxBag;

        [Header("交易")]
        public int playerMoney;

        private Dictionary<string, List<InventoryItem>> boxDataDict = new Dictionary<string, List<InventoryItem>>();
        public int BoxDataAmount => boxDataDict.Count;

        public string GUID => GetComponent<DataGUID>().guid;

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
            //建造
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;

        }

        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }


        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        private void OnStartNewGameEvent(int obj)
        {
            playerBag = Instantiate(playerBagTemp);
            playerMoney = Settings.playerStartMoney;
            boxDataDict.Clear();
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
        {
            currentBoxBag = bag_SO;
        }


        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            RemoveItem(ID, 1);
            BluePrintDetails bluePrint = bluePrintData.GetBluePrintDetails(ID);
            if (bluePrint.resourceItem.Length > 0)
            {
                foreach (var item in bluePrint.resourceItem)
                {
                    RemoveItem(item.itemID, item.itemAmount);
                }
            }
        }

        private void OnDropItemEvent(int ID, Vector3 pos, ItemType itemType)
        {
            RemoveItem(ID, 1);
        }

        private void OnHarvestAtPlayerPosition(int ID)
        {
            //是否已经有该物品
            var index = GetItemIndexInBag(ID);

            AddItemAtIndex(ID, index, 1);

            //更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 通过ID返回物品信息
        /// </summary>
        /// <param name="ID">Item ID</param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }

        /// <summary>
        /// 添加物品到Player背包里
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">是否要销毁物品</param>
        public void AddItem(Item item, bool toDestory)
        {
            //是否已经有该物品
            AddItem(item.itemID, 1);

            //Debug.Log(GetItemDetails(item.itemID).itemID + "Name: " + GetItemDetails(item.itemID).itemName);
            if (toDestory)
            {
                Destroy(item.gameObject);
            }

        }

        /// <summary>
        /// 检查背包是否有空位
        /// </summary>
        /// <returns></returns>
        public bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                    return true;
            }
            return false;
        }

        public bool CanAddItem(int itemID)
        {
            var index = GetItemIndexInBag(itemID);
            if (index != -1 || CheckBagCapacity()) return true;
            return false;
        }

        /// <summary>
        /// 通过物品ID找到背包已有物品位置
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns>-1则没有这个物品否则返回序号</returns>
        public int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }

        public int CountEmptySlots()
        {
            int count = 0;
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                    ++count;
            }
            return count;
        }
        private List<int> GetItemIndexsInBag(int ID)
        {
            List<int> res = new List<int>();
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                    res.Add(i);
            }
            return res;
        }
        public int GetItemNumInBag(int ID)
        {
            var itemIndexs = GetItemIndexsInBag(ID);
            int num = 0;
            for (int i = 0; i < itemIndexs.Count; ++i)
            {
                var index = itemIndexs[i];
                num += playerBag.itemList[index].itemAmount;
            }
            return num;
        }

        /// <summary>
        /// 在指定背包序号位置添加物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="index">序号</param>
        /// <param name="amount">数量</param>
        public bool AddItemAtIndex(int ID, int index, int amount, bool AddToAchievement = true)
        {//TODO: 改成void
            if (index == -1)    //背包没有这个物品 同时背包有空位
            {
                if(CheckBagCapacity())
                {
                    var item = new InventoryItem { itemID = ID, itemAmount = amount };
                    for (int i = 0; i < playerBag.itemList.Count; i++)
                    {
                        if (playerBag.itemList[i].itemID == 0)
                        {
                            playerBag.itemList[i] = item;
                            break;
                        }
                    }
                }
                else
                {
                    UIManager.Instance.GoTo(10);
                    return false;
                }
            }
            else    //背包有这个物品
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem(ID, currentAmount);
                playerBag.itemList[index] = item;
            }
            
            
            if(AddToAchievement)
                AchievementManager.Instance.AddGetItemNum(ID, amount);
            return true;
        }

        public void AddItem(int itemID,int num)
        {
            //是否已经有该物品
            var index = GetItemIndexInBag(itemID);

            AddItemAtIndex(itemID, index, num);
            //更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// Player背包范围内交换物品
        /// </summary>
        /// <param name="fromIndex">起始序号</param>
        /// <param name="targetIndex">目标数据序号</param>
        /// <param name="fromnum">交换个数</param>
        public void SwapItem(int fromIndex, int targetIndex, int fromnum = -1)
        {
            if (fromIndex == targetIndex)
            {
                return;
            }
            InventoryItem currentItem = playerBag.itemList[fromIndex];
            InventoryItem targetItem = playerBag.itemList[targetIndex];
            if(fromnum > currentItem.itemAmount)
            {
                Debug.Log("fromnum > currentItem.itemAmount");
                return;
            }
            if (fromnum > 0)
            {
                if (targetItem.isEmpty())
                {
                    currentItem.itemAmount -= fromnum;
                    playerBag.itemList[fromIndex] = currentItem;
                    currentItem.itemAmount = fromnum;
                    playerBag.itemList[targetIndex] = currentItem;
                    
                }
                else
                {
                    if (targetItem.itemID == currentItem.itemID)
                    {
                        currentItem.itemAmount -= fromnum;
                        playerBag.itemList[fromIndex] = currentItem;
                        targetItem.itemAmount += fromnum;
                        playerBag.itemList[targetIndex] = targetItem;
                    }
                    else
                    {
                        if (currentItem.itemAmount == fromnum)
                        {
                            playerBag.itemList[targetIndex] = currentItem;
                            playerBag.itemList[fromIndex] = targetItem;
                        }
                        else
                        {
                            UIManager.Instance.GoTo(10);
                        }
                    }
                }
            }
            else
            {
                // fromnum==-1 交换全部
                if (targetItem.itemID == currentItem.itemID)
                {
                    playerBag.itemList[fromIndex] = new InventoryItem();;
                    targetItem.itemAmount += currentItem.itemAmount;
                    playerBag.itemList[targetIndex] = targetItem;
                }
                else
                {
                    playerBag.itemList[fromIndex] = targetItem;
                    playerBag.itemList[targetIndex] = currentItem;
                }

            }



            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 跨背包交换数据
        /// </summary>
        /// <param name="locationFrom"></param>
        /// <param name="fromIndex"></param>
        /// <param name="locationTarget"></param>
        /// <param name="targetIndex"></param>
        public void SwapItem(InventoryLocation locationFrom, int fromIndex, InventoryLocation locationTarget, int targetIndex, int fromnum = -1)
        {
            var currentList = GetItemList(locationFrom);
            var targetList = GetItemList(locationTarget);

            InventoryItem currentItem = currentList[fromIndex];

            if (targetIndex < targetList.Count)
            {
                InventoryItem targetItem = targetList[targetIndex];

                if (!targetItem.isEmpty() && currentItem.itemID != targetItem.itemID)  //有不相同的两个物品
                {
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }
                else if (currentItem.itemID == targetItem.itemID) //相同的两个物品
                {
                    targetItem.itemAmount += currentItem.itemAmount;
                    targetList[targetIndex] = targetItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                else    //目标空格子
                {
                    targetList[targetIndex] = currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                EventHandler.CallUpdateInventoryUI(locationFrom, currentList);
                EventHandler.CallUpdateInventoryUI(locationTarget, targetList);
            }
        }

        /// <summary>
        /// 根据位置返回背包数据列表
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private List<InventoryItem> GetItemList(InventoryLocation location)
        {
            return location switch
            {
                InventoryLocation.Player => playerBag.itemList,
                InventoryLocation.Box => currentBoxBag.itemList,
                _ => null
            };
        }

        /// <summary>
        /// 移除指定数量的背包物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="removeAmount">数量</param>
        public bool RemoveItem(int ID, int removeAmount)
        {
            int haveNum = GetItemNumInBag(ID);
            if (haveNum < removeAmount)//应该都有在前面做判断，以防万一
            {
                UIManager.Instance.GoTo(10);
                return false;
            }
            var indexs = GetItemIndexsInBag(ID);
            
            foreach(int index in indexs)
            {
                if (playerBag.itemList[index].itemAmount > removeAmount)
                {
                    var amount = playerBag.itemList[index].itemAmount - removeAmount;
                    var item = new InventoryItem { itemID = ID, itemAmount = amount };
                    playerBag.itemList[index] = item;
                    break;
                }
                else if (playerBag.itemList[index].itemAmount == removeAmount)
                {
                    var item = new InventoryItem();
                    playerBag.itemList[index] = item;
                    break;
                }
                else
                {
                    removeAmount -= playerBag.itemList[index].itemAmount;
                    var item = new InventoryItem();
                    playerBag.itemList[index] = item;
                }
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
            return true;
        }
        public void RemoveItemAtIndex(int removeAmount, int index)
        {
            int ID = playerBag.itemList[index].itemID;
            if (playerBag.itemList[index].itemAmount > removeAmount)
            {
                var amount = playerBag.itemList[index].itemAmount - removeAmount;
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                playerBag.itemList[index] = item;
            }
            else if (playerBag.itemList[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem();
                playerBag.itemList[index] = item;
            }else
            {
                UIManager.Instance.GoTo(10);
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
            
        }

        /// <summary>
        /// 交易物品
        /// </summary>
        /// <param name="itemDetails">物品信息</param>
        /// <param name="amount">交易数量</param>
        /// <param name="isSellTrade">是否卖东西</param>
        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            int cost = itemDetails.itemPrice * amount;
            //获得物品背包位置
            int index = GetItemIndexInBag(itemDetails.itemID);

            if (isSellTrade)    //卖
            {
                if (playerBag.itemList[index].itemAmount >= amount)
                {
                    if(itemDetails.itemPrice<=0)
                    {
                        UIManager.Instance.GoTo(3);
                        return;
                    }
                    RemoveItem(itemDetails.itemID, amount);
                    //卖出总价
                    cost = (int)(cost * itemDetails.sellPercentage);
                    playerMoney += cost;
                }
                else
                {
                    UIManager.Instance.GoTo(10);
                }
            }
            else 
            {
                if (playerMoney - cost >= 0)   //买
                {
                    if (CanAddItem(itemDetails.itemID))
                    {
                        AddItemAtIndex(itemDetails.itemID, index, amount);
                        playerMoney -= cost;
                    }
                    else
                    {
                        UIManager.Instance.GoTo(12);
                    }
                }
                else
                {
                    UIManager.Instance.GoTo(10);
                }
            }
            //刷新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 检查建造资源物品库存
        /// </summary>
        /// <param name="ID">图纸ID</param>
        /// <returns></returns>
        public bool CheckStock(int ID)
        {
            var bluePrintDetails = bluePrintData.GetBluePrintDetails(ID);

            if (bluePrintDetails == null) return false;
            if (bluePrintDetails.resourceItem.Length <= 0) return true;
            foreach (var resourceItem in bluePrintDetails.resourceItem)
            {
                int num = InventoryManager.Instance.GetItemNumInBag(resourceItem.itemID);
                if (num >= resourceItem.itemAmount)
                {
                    continue;
                }
                else return false;
            }
            return true;
        }

        /// <summary>
        /// 查找箱子数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBoxDataList(string key)
        {
            if (boxDataDict.ContainsKey(key))
                return boxDataDict[key];
            return null;
        }

        /// <summary>
        /// 加入箱子数据字典
        /// </summary>
        /// <param name="box"></param>
        public void AddBoxDataDict(Box box)
        {
            var key = box.name + box.index;
            if (!boxDataDict.ContainsKey(key))
                boxDataDict.Add(key, box.boxBagData.itemList);
        }

        public void ArrangePlayerBag()
        {
            var list = playerBag.itemList;
            list.Sort();
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        public void ArrangeAndMergePlayerBag()
        {
            ArrangePlayerBag();
            var list = playerBag.itemList;
            int slowIndex = 0;
            int fastIndex = 1;
            int needAddNum = 0;
            while (fastIndex < list.Count)
            {
                if (list[slowIndex].itemID == list[fastIndex].itemID)
                {
                    needAddNum += list[fastIndex].itemAmount;
                    list[fastIndex] = new InventoryItem();
                }
                else
                {
                    if (needAddNum != 0)
                    {
                        AddItemAtIndex(list[slowIndex].itemID, slowIndex, needAddNum, false);
                        needAddNum = 0;
                    }
                    slowIndex = fastIndex;
                }
                fastIndex++;
            }
            if (needAddNum != 0)
            {
                AddItemAtIndex(list[slowIndex].itemID, slowIndex, needAddNum, false);
            }
            ArrangePlayerBag();
        }


        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.playerMoney = this.playerMoney;

            saveData.inventoryDict = new Dictionary<string, List<InventoryItem>>();
            saveData.inventoryDict.Add(playerBag.name, playerBag.itemList);

            foreach (var item in boxDataDict)
            {
                saveData.inventoryDict.Add(item.Key, item.Value);
            }
            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.playerMoney = saveData.playerMoney;
            playerBag = Instantiate(playerBagTemp);
            playerBag.itemList = saveData.inventoryDict[playerBag.name];

            foreach (var item in saveData.inventoryDict)
            {
                if (boxDataDict.ContainsKey(item.Key))
                {
                    boxDataDict[item.Key] = item.Value;
                }
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
    }
}