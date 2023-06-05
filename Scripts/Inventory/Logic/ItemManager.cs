using System.Collections;
using System.Collections.Generic;
using MFarm.Save;
using MFarm.Map;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace MFarm.Inventory
{
    public class ItemManager : Singleton<ItemManager>, ISaveable
    {
        public Item itemPrefab;
        public Item bounceItemPrefab;
        private Transform itemParent;

        private Transform PlayerTransform => FindObjectOfType<Player>().transform;

        public string GUID => GetComponent<DataGUID>().guid;

        //记录场景Item
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
        //记录场景家具
        private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            //建造
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
            EventHandler.GameDayEvent1 += OnGameDayEvent1;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            EventHandler.GameDayEvent1 += OnGameDayEvent1;
        }


        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();
        }

        private void OnStartNewGameEvent(int obj)
        {
            sceneItemDict.Clear();
            sceneFurnitureDict.Clear();
            //默认有床
            SceneFurniture bed = new SceneFurniture
            {
                itemID = 1030,
                position = new SerializableVector3(new Vector3(6.79f, -5.5f))
            };
            sceneFurnitureDict.Add("02.Home", new List<SceneFurniture>(){bed});
        }
        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(ID);
            var pos = mousePos;
            if (bluePrint.inGridCenter)
            {
                pos.x = Mathf.Floor(pos.x) + 0.5f;
                pos.y = Mathf.Floor(pos.y) + 0.5f;
            }
            var buildItem = Instantiate(bluePrint.buildPrefab, pos, Quaternion.identity, itemParent);
            if (buildItem.GetComponent<Box>())
            {
                buildItem.GetComponent<Box>().index = InventoryManager.Instance.BoxDataAmount;
                buildItem.GetComponent<Box>().InitBox(buildItem.GetComponent<Box>().index);
            }
            //添加到sceneFurnitureDict中
            List<SceneFurniture> currentSceneFurniture;
            if(sceneFurnitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFurniture))
            {

            }
            else
            {
                currentSceneFurniture = new List<SceneFurniture>();
            }

            SceneFurniture sceneFurniture = new SceneFurniture
            {
                itemID = ID,
                position = new SerializableVector3(pos)
            };
            if (buildItem.GetComponent<Box>())
                sceneFurniture.boxIndex = buildItem.GetComponent<Box>().index;
            currentSceneFurniture.Add(sceneFurniture);
            sceneFurnitureDict.TryAdd(SceneManager.GetActiveScene().name,currentSceneFurniture);
        }

        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
            GetAllSceneFurniture();
        }

        private void OnAfterSceneLoadedEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItems();
            RebuildFurniture();
        }


        /// <summary>
        /// 在指定坐标位置生成物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="pos">世界坐标</param>
        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(bounceItemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;
            item.GetComponent<Item>().Init(ID);
            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);
        }

        private void OnDropItemEvent(int ID, Vector3 mousePos, ItemType itemType)
        {
            if (itemType == ItemType.Seed) return;

            var item = Instantiate(bounceItemPrefab, PlayerTransform.position, Quaternion.identity, itemParent);
            item.itemID = ID;
            var dir = (mousePos - PlayerTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);
        }


        /// <summary>
        /// 获得当前场景所有Item
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();

            var items = FindObjectsOfType<Item>();
            if (items != null && items.Length > 0)
            {
                foreach (var item in items)
                {
                    SceneItem sceneItem = new SceneItem
                    {
                        itemID = item.itemID,
                        position = new SerializableVector3(item.transform.position)
                    };
                    currentSceneItems.Add(sceneItem);
                }
            }
            var currActiveScene = SceneManager.GetActiveScene().name;
            if (sceneItemDict.ContainsKey(currActiveScene))
            {
                //找到数据就更新item数据列表
                sceneItemDict[currActiveScene] = currentSceneItems;
            }
            else    //如果是新场景
            {
                sceneItemDict.Add(currActiveScene, currentSceneItems);
            }
        }


        /// <summary>
        /// 刷新重建当前场景物品
        /// </summary>
        private void RecreateAllItems()
        {
            //清场
            var items = FindObjectsOfType<Item>();
            if (items != null && items.Length > 0)
            {
                foreach (var item in items)
                {
                    Destroy(item.gameObject);
                }
            }
            
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
            {
                if (currentSceneItems != null)
                {
                    if (currentSceneItems.Count > 0)
                    {
                        foreach (var item in currentSceneItems)
                        {
                            Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                            newItem.Init(item.itemID);
                        }
                    }

                }
            }
        }


        /// <summary>
        /// 获得场景所有家具
        /// </summary>
        private void GetAllSceneFurniture()
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();

            var furnitures = FindObjectsOfType<Furniture>();
            if (furnitures != null && furnitures.Length > 0)
            {
                foreach (var item in furnitures)
                {
                    SceneFurniture sceneFurniture = new SceneFurniture
                    {
                        itemID = item.itemID,
                        position = new SerializableVector3(item.transform.position)
                    };
                    if (item.GetComponent<Box>())
                        sceneFurniture.boxIndex = item.GetComponent<Box>().index;

                    currentSceneFurniture.Add(sceneFurniture);
                }
            }
            if (sceneFurnitureDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //找到数据就更新item数据列表
                sceneFurnitureDict[SceneManager.GetActiveScene().name] = currentSceneFurniture;
            }
            else    //如果是新场景
            {
                sceneFurnitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFurniture);
            }
        }

        /// <summary>
        /// 重建当前场景家具
        /// </summary>
        private void RebuildFurniture()
        {
            //清场
            var furnitures = FindObjectsOfType<Furniture>();
            if (furnitures != null && furnitures.Length > 0)
            {
                foreach (var furniture in furnitures)
                {
                    Destroy(furniture.gameObject);
                }
            }

            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();

            if (sceneFurnitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFurniture))
            {
                if (currentSceneFurniture != null && currentSceneFurniture.Count > 0)
                {
                    foreach (SceneFurniture sceneFurniture in currentSceneFurniture)
                    {
                        BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(sceneFurniture.itemID);
                        var buildItem = Instantiate(bluePrint.buildPrefab, sceneFurniture.position.ToVector3(), Quaternion.identity, itemParent);
                        if (buildItem.GetComponent<Box>())
                        {
                            buildItem.GetComponent<Box>().InitBox(sceneFurniture.boxIndex);
                        }
                    }
                }
            }
        }

        void OnGameDayEvent1(int day, Season season)
        {
            foreach(var sceneFurnitures in sceneFurnitureDict)
            {
                string sceneName = sceneFurnitures.Key;
                foreach(var furniture in sceneFurnitures.Value)
                {
                    //sprinkler
                    if (furniture.itemID == 1043)
                    {
                        SprinklerOnGameDayEvent1(furniture.position,sceneName);
                    }
                }
                
            }
        }

        //Sprinkler
        void SprinklerOnGameDayEvent1(SerializableVector3 position, string sceneName)
        {
            Vector2Int sprinklerpos = position.ToVector2Int();
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if ((i != 0 || j != 0) && ((i == 0 || j == 0)))
                    {
                        var pos = new Vector2Int(sprinklerpos.x + i, sprinklerpos.y + j);
                        var tileDetails = GridMapManager.Instance.GetTileDetailsByPosAndSceneName(pos, sceneName);
                        if (tileDetails != null && tileDetails.daysSinceDug >= 0)
                        {
                            tileDetails.daysSinceDug += 1;
                            tileDetails.daysSinceWatered = 1;
                            if (sceneName == SceneManager.GetActiveScene().name)
                            {
                                GridMapManager.Instance.SetWaterGround(tileDetails);
                            }
                        }
                    }
                }
            }
        }
        public GameSaveData GenerateSaveData()
        {
            GetAllSceneItems();
            GetAllSceneFurniture();

            GameSaveData saveData = new GameSaveData();
            saveData.sceneItemDict = this.sceneItemDict;
            saveData.sceneFurnitureDict = this.sceneFurnitureDict;

            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.sceneItemDict = saveData.sceneItemDict;
            this.sceneFurnitureDict = saveData.sceneFurnitureDict;

            RecreateAllItems();
            RebuildFurniture();
        }
    }
}