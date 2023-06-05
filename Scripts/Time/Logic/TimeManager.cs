using System;
using System.Collections;
using System.Collections.Generic;
using MFarm.Save;

using UnityEngine;
public class TimeManager : Singleton<TimeManager>, ISaveable
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season gameSeason = Season.春天;
    private int monthInSeason = 1;

    public bool gameClockPause;
    private float tikTime;

    //灯光时间差
    private float timeDifference;

    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);

    public string GUID => GetComponent<DataGUID>().guid;

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
        // EventHandler.GamePauseEvent += () => { gameClockPause = true; };
        // EventHandler.GameResumeEvent += () => { gameClockPause = false; };
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        // EventHandler.GamePauseEvent -= () => { gameClockPause = true; };
        // EventHandler.GameResumeEvent -= () => { gameClockPause = false; };
    }


    private void Start()
    {

        ISaveable saveable = this;
        saveable.RegisterSaveable();
        gameClockPause = true;
        // EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        // EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        // //切换灯光
        // EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
    }

    private void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;

            if (tikTime >= Settings.secondThreshold)
            {
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }

        if (Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < 60; i++)
            {
                UpdateGameTime();
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ToNextMorning();
        }
    }
    private void OnEndGameEvent()
    {
        gameClockPause = true;
    }
    private void OnStartNewGameEvent(int obj)
    {
        NewGameTime();
        // gameClockPause = false;
    }

    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 6;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 1;
        gameSeason = Season.春天;
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Gameplay:
                gameClockPause = false;
                break;
            case GameState.Pause:
                gameClockPause = true;
                break;
            case GameState.GameEnd:
                gameClockPause = true;
                break;
            default: break;
        }
    }

    private void OnAfterSceneLoadedEvent()
    {
        // gameClockPause = false;
        EventHandler.CallGameHourEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        //切换灯光
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
    }

    private void OnBeforeSceneUnloadEvent()
    {
        // gameClockPause = true;
    }

    private void UpdateGameTime()
    {
        gameSecond++;
        if (gameSecond > Settings.secondHold)
        {
            gameMinute++;
            gameSecond = 0;

            if (gameMinute > Settings.minuteHold)
            {
                gameHour++;
                gameMinute = 0;

                if (gameHour > Settings.hourHold)
                {
                    gameHour = 0;
                    AddGameDay();
                }
                //小时增加
                EventHandler.CallGameHourEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            }
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
            //切换灯光
            EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
        }

        // Debug.Log("Second: " + gameSecond + " Minute: " + gameMinute);
    }

    /// <summary>
    /// 返回lightshift同时计算时间差
    /// </summary>
    /// <returns></returns>
    private LightShift GetCurrentLightShift()
    {
        if (GameTime >= Settings.morningTime && GameTime < Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;
            return LightShift.Morning;
        }

        if (GameTime < Settings.morningTime || GameTime >= Settings.nightTime)
        {
            timeDifference = Mathf.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);
            // Debug.Log(timeDifference);
            return LightShift.Night;
        }

        return LightShift.Morning;
    }

    /// <summary>
    /// 到达下一个上午六点
    /// </summary>
    /// <returns></returns>
    public void ToNextMorning()
    {
        if (gameHour >= 6)
        {
            AddGameDay();
        }
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 6;
        EventHandler.CallGameHourEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        //切换灯光
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);

    }
    void AddGameDay()
    {
        gameDay++;
        if (gameDay > Settings.dayHold)
        {
            gameDay = 1;
            gameMonth++;

            if (gameMonth > Settings.monthHold)
                gameMonth = 1;

            monthInSeason--;
            if (monthInSeason == 0)
            {
                monthInSeason = 1;

                int seasonNumber = (int)gameSeason;
                seasonNumber++;

                if (seasonNumber > Settings.seasonHold)
                {
                    seasonNumber = 0;
                    gameYear++;
                }

                gameSeason = (Season)seasonNumber;

                if (gameYear > 9999)
                {
                    gameYear = 2023;
                }
            }
        }
        //用来刷新地图和农作物生长
        EventHandler.CallGameDayEvent(gameDay, gameSeason);
        EventHandler.CallGameDayEvent1(gameDay, gameSeason);
    }
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("gameYear", gameYear);
        saveData.timeDict.Add("gameSeason", (int)gameSeason);
        saveData.timeDict.Add("gameMonth", gameMonth);
        saveData.timeDict.Add("gameDay", gameDay);
        saveData.timeDict.Add("gameHour", gameHour);
        saveData.timeDict.Add("gameMinute", gameMinute);
        saveData.timeDict.Add("gameSecond", gameSecond);

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        gameYear = saveData.timeDict["gameYear"];
        gameSeason = (Season)saveData.timeDict["gameSeason"];
        gameMonth = saveData.timeDict["gameMonth"];
        gameDay = saveData.timeDict["gameDay"];
        gameHour = saveData.timeDict["gameHour"];
        gameMinute = saveData.timeDict["gameMinute"];
        gameSecond = saveData.timeDict["gameSecond"];
    }
}
