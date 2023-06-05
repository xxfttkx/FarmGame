using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;

public class AchievementManager : Singleton<AchievementManager>, ISaveable
{
    public int finishTaskNum;
    public Dictionary<int, int> getItemNumDict;
    public string GUID => GetComponent<DataGUID>().guid;
    protected override void Awake()
    {
        base.Awake();
    }
    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }
    private void OnEnable()
    {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }



    private void OnDisable()
    {
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void OnStartNewGameEvent(int obj)
    {
        getItemNumDict = new Dictionary<int, int>();
    }
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.finishTaskNum = this.finishTaskNum;
        saveData.getItemNumDict = this.getItemNumDict;
        return saveData;
    }
    public void RestoreData(GameSaveData saveData)
    {
        this.finishTaskNum = saveData.finishTaskNum;
        this.getItemNumDict = saveData.getItemNumDict;
    }
    public void AddGetItemNum(int itemID, int AddNum)
    {
        if (getItemNumDict.ContainsKey(itemID))
        {
            getItemNumDict[itemID] += AddNum;
        }
        else
        {
            getItemNumDict.Add(itemID, AddNum);
        }
    }
}
