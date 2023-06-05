using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class Box : MonoBehaviour
    {
        public InventoryBag_SO boxBagTemplate;
        public InventoryBag_SO boxBagData;

        public GameObject mouseIcon;
        private bool canOpen = false;
        private bool isOpen;
        public int index;

        private void OnEnable()
        {
            if (boxBagData == null)
            {
                boxBagData = Instantiate(boxBagTemplate);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = true;
                mouseIcon.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = false;
                mouseIcon.SetActive(false);
            }
        }

        private void Update()
        {
            if (!isOpen && canOpen && Input.GetMouseButtonDown(1))
            {
                //打开箱子
                // var coll = this.gameObject.GetComponent<Collider2D>();
                // var mainCamera = Camera.main;
                // var mouseWorldPos = mainCamera.ScreenToWorldPoint
                //     (new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
                
                // EventHandler.CallBaseBagOpenEvent(SlotType.Box, boxBagData);
                // isOpen = true;
                // EventHandler.CallUpdateGameStateEvent(GameState.Pause);
            }

            if (!canOpen && isOpen)
            {
                //关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
                EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
            }

            if (isOpen && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)))
            {
                //关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
                EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
            }
        }

        /// <summary>
        /// 初始化Box和数据
        /// </summary>
        /// <param name="boxIndex"></param>
        public void InitBox(int boxIndex)
        {
            index = boxIndex;
            var key = this.name + index;
            if (InventoryManager.Instance.GetBoxDataList(key) != null)  //刷新地图读取数据
            {
                boxBagData.itemList = InventoryManager.Instance.GetBoxDataList(key);
            }
            else     //新建箱子
            {
                InventoryManager.Instance.AddBoxDataDict(this);
            }
        }

        public void OpenBox()
        {
            EventHandler.CallBaseBagOpenEvent(SlotType.Box, boxBagData);
            isOpen = true;
            EventHandler.CallUpdateGameStateEvent(GameState.Pause);
        }
    }
}
