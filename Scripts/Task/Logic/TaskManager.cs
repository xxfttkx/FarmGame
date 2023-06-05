using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Inventory;
using MFarm.Save;

public class TaskManager : Singleton<TaskManager>, ISaveable
{
    public TaskDataList_SO taskDataList;
    public Task dailyTask;
    public List<Task> playerTasks;

    public string GUID => GetComponent<DataGUID>().guid;

    protected override void Awake()
    {
        base.Awake();
        playerTasks = new List<Task>();
        dailyTask = null;
    }
    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }
    private void OnEnable()
    {
        EventHandler.GameDayEvent += OnGameDayEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }



    private void OnDisable()
    {
        EventHandler.GameDayEvent -= OnGameDayEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }
    private void Update()
    {

    }
    private void OnGameDayEvent(int arg1, Season season)
    {
        UpdateDailyTask();

        //删除过期Task
        if (playerTasks == null) return;
        foreach (var task in playerTasks)
        {
            --task.day;
        }
        playerTasks.RemoveAll(t => t.day < 0);
    }
    void UpdateDailyTask()
    {
        var taskList = taskDataList.taskList;
        int count = taskList.Count;
        if (count < 1) return;
        int index = Random.Range(0, count);
        dailyTask = new Task(taskList[index]);
    }
    /// <summary>
    /// 获得今日任务
    /// </summary>
    public void GetTask()
    {
        if (dailyTask == null || playerTasks == null) return;
        playerTasks.Add(dailyTask);
        dailyTask = null;
        UIManager.Instance.GoTo(9);
    }
    public bool CanGetDailyTask()
    {
        return dailyTask != null && dailyTask.needItemID != 0;
    }

    /// <summary>
    /// 完成任务
    /// </summary>
    /// <param name="itemID"></param>
    /// <param name="num"></param>
    /// <returns></returns>
    public bool TryFinishTask(int itemID, int num)
    {
        for (int i = 0; i < playerTasks.Count; ++i)
        {
            if (playerTasks[i].needItemID != itemID) continue;
            if (num < playerTasks[i].needItemID) return false;
            playerTasks.RemoveAt(i);
            //todo 写在调用该函数那 对应物品减少
            AchievementManager.Instance.finishTaskNum++;
            return true;
        }
        return false;
    }
    public void FinishTask(Task t)
    {
        InventoryManager.Instance.RemoveItem(t.needItemID, t.needNum);
        playerTasks.Remove(t);
        ++AchievementManager.Instance.finishTaskNum;
        UIManager.Instance.taskUI?.GetComponent<TaskUI>()?.ShowTaskContents();
        UIManager.Instance.GoTo(11);
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Gameplay:
                //TODO:应该什么都不做
                break;
            case GameState.Pause:
                break;
            case GameState.GameEnd:
                break;
            default: break;
        }
    }

    void OnStartNewGameEvent(int index)
    {
        UpdateDailyTask();
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.playerTasks = this.playerTasks;
        saveData.dailyTask = this.dailyTask;
        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        this.playerTasks = saveData.playerTasks;
        this.dailyTask = saveData.dailyTask;
    }


}
