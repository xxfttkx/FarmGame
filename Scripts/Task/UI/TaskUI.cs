using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MFarm.Inventory;

public class TaskUI : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject taskButtonPrefab;
    public GameObject taskContent;
    public Button getDailyTask;
    public Button submitButton;
    public Text needNumText;
    public Image needItemImage;
    public Image coinImage;
    public Text HaveNumText;
    public Text RemainDayText;
    public Text CoinText;

    public Task currTask;
    public int currItemIndex;
    private void Awake()
    {
        getDailyTask.onClick.AddListener(() => TaskManager.Instance.GetTask());
        getDailyTask.onClick.AddListener(() => ShowTaskContents());
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ShowTaskContents()
    {
        var currTaskButtons = GetComponentsInChildren<TaskButtonUI>(true);
        var playTasks = TaskManager.Instance.playerTasks;
        int showNum = playTasks.Count + 1;
        int needAddNum = showNum - currTaskButtons.Length;
        for (int i = 0; i < needAddNum; ++i)
        {
            Instantiate(taskButtonPrefab, Vector3.zero, Quaternion.identity, taskContent.transform);
        }
        var newTaskButtons = GetComponentsInChildren<TaskButtonUI>(true);
        var currActiveTaskButtons = GetComponentsInChildren<TaskButtonUI>(false);
        int index = 0;
        //每日任务
        if (index < currActiveTaskButtons.Length &&
            currActiveTaskButtons[index] == newTaskButtons[0])
            ++index;
        if (TaskManager.Instance.CanGetDailyTask())
        {
            var button = newTaskButtons[0].gameObject;
            if(button.activeInHierarchy)
            {
                
            }
            else
            {
                button.SetActive(true);
            }
            
            // TextMeshPro text = button.transform.GetComponentInChildren<TextMeshPro>(); 
            // TextMeshPro获取不了 改用Text
            var text = button.transform.GetComponentInChildren<Text>();
            text.text = "今日任务";
            var image = button.transform.GetComponent<Image>();
            image.color = Color.red;
            //设定Button onclick响应
            var b = button.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener(() => ShowDetails(TaskManager.Instance.dailyTask, true));
        }
        else
        {
            var button = newTaskButtons[0].gameObject;
            button.SetActive(false);
        }

        //玩家拥有的任务
        for (int i = 0; i < playTasks.Count; ++i)
        {
            if (index < currActiveTaskButtons.Length &&
                    currActiveTaskButtons[index] ==newTaskButtons[i + 1])
                    ++index;
            var button = newTaskButtons[i + 1].gameObject;
            button.SetActive(true);
            var text = button.transform.GetComponentInChildren<Text>();
            string npcName = "";
            Settings.nameChange.TryGetValue(playTasks[i].NPCName, out npcName);
            text.text = npcName + "的需求";
            var image = button.transform.GetComponent<Image>();
            image.color = Color.white;
            //设定Button onclick响应
            var b = button.GetComponent<Button>();
            b.onClick.RemoveAllListeners();
            var t = playTasks[i];
            b.onClick.AddListener(() => ShowDetails(t, false));
        }
        //取消激活多余的button
        for (int i = index; i < currActiveTaskButtons.Length; ++i)
        {
            currActiveTaskButtons[i].gameObject.SetActive(false);
        }
        //默认点击第一个
        if (TaskManager.Instance.CanGetDailyTask())
        {
            ShowDetails(TaskManager.Instance.dailyTask, true);
        }
        else
        {
            if (playTasks.Count > 0)
            {
                ShowDetails(playTasks[0], false);
            }
            else
            {
                ClearRightArea();
            }
        }
    }
    public void CloseTaskContents()
    {
        var taskButtons = GetComponentsInChildren<TaskButtonUI>();
        if (taskButtons != null)
        {
            foreach (var t in taskButtons)
            {
                t.gameObject.SetActive(false);
            }
        }
    }

    public void ShowDetails(Task t, bool isDailyTask)
    {
        if (isDailyTask)
        {
            getDailyTask.gameObject.SetActive(true);
            submitButton.gameObject.SetActive(false);
        }
        else
        {
            getDailyTask.gameObject.SetActive(false);
            submitButton.gameObject.SetActive(true);
        }

        currTask = t;
        currItemIndex = t.needItemID;
        string npcName = "";
        Settings.nameChange.TryGetValue(t.NPCName, out npcName);
        needNumText.text = npcName + "需要" + t.needNum + "个";
        RemainDayText.text = "剩余" + t.day + "天";
        int currHaveNum = InventoryManager.Instance.GetItemNumInBag(currItemIndex);
        needItemImage.gameObject.SetActive(true);
        coinImage.gameObject.SetActive(true);
        var item = InventoryManager.Instance.GetItemDetails(t.needItemID);
        needItemImage.sprite = item.itemIcon;
        int price = (int)(item.itemPrice * item.sellPercentage * 3 * t.needNum);
        CoinText.text = price.ToString();
        HaveNumText.text = currHaveNum + "/" + t.needNum;
        if (currHaveNum >= t.needNum)
        {
            HaveNumText.color = Color.green;
            submitButton.interactable = true;
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(() => TaskManager.Instance.FinishTask(t));
            submitButton.onClick.AddListener(() => InventoryManager.Instance.playerMoney += price);
            submitButton.onClick.AddListener(() => 
                EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, InventoryManager.Instance.playerBag.itemList));
            
            //submitButton.onClick.AddListener(() => ShowTaskContents());
        }
        else
        {
            HaveNumText.color = Color.red;
            submitButton.interactable = false;
        }
    }
    void ClearRightArea()
    {
        getDailyTask.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        needNumText.text = "";
        HaveNumText.text = "";
        RemainDayText.text = "";
        CoinText.text = "";
        needItemImage.gameObject.SetActive(false);
        coinImage.gameObject.SetActive(false);
    }
}
