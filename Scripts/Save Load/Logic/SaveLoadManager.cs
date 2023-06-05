using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace MFarm.Save
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        private List<ISaveable> saveableList = new List<ISaveable>();

        public List<DataSlot> dataSlots = new List<DataSlot>(new DataSlot[3]);

        private string jsonFolder;
        private int currentDataIndex;
        private bool isPlaying = false;

        protected override void Awake()
        {
            base.Awake();
            jsonFolder = Application.persistentDataPath + "/SAVE DATA/";
            ReadSaveData();
        }
        private void OnEnable()
        {
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
            EventHandler.EndGameEvent += OnEndGameEvent;
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        }

        private void OnDisable()
        {
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            EventHandler.EndGameEvent -= OnEndGameEvent;
            EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        }


        private void Update()
        {
            if (isPlaying)
            {
                if (Input.GetKeyDown(KeyCode.I))
                {
                    EventHandler.CallUpdateGameStateEvent(GameState.Pause);
                    UIManager.Instance.GoTo(1, SaveAction,
                        () => EventHandler.CallUpdateGameStateEvent(GameState.Gameplay));
                }
                if (Input.GetKeyDown(KeyCode.O))
                {
                    EventHandler.CallUpdateGameStateEvent(GameState.Pause);
                    UIManager.Instance.GoTo(2, LoadAction,
                        () => EventHandler.CallUpdateGameStateEvent(GameState.Gameplay));
                }
            }
        }


        private void OnEndGameEvent()
        {
            Save(currentDataIndex);
            isPlaying = false;
        }

        private void OnStartNewGameEvent(int index)
        {
            currentDataIndex = index;
            isPlaying = true;
        }
        public void RegisterSaveable(ISaveable saveable)
        {
            if (!saveableList.Contains(saveable))
                saveableList.Add(saveable);
        }

        private void ReadSaveData()
        {
            if (Directory.Exists(jsonFolder))
            {
                for (int i = 0; i < dataSlots.Count; i++)
                {
                    var resultPath = jsonFolder + "data" + i + ".json";
                    if (File.Exists(resultPath))
                    {
                        var stringData = File.ReadAllText(resultPath);
                        var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);
                        dataSlots[i] = jsonData;
                    }
                }
            }
        }


        public void Save()
        {
            Save(currentDataIndex);
        }
        private void Save(int index)
        {
            DataSlot data = new DataSlot();
            foreach (var saveable in saveableList)
            {
                data.dataDict.Add(saveable.GUID, saveable.GenerateSaveData());
            }
            dataSlots[index] = data;
            var resultPath = jsonFolder + "data" + index + ".json";
            var jsonData = JsonConvert.SerializeObject(dataSlots[index], Formatting.Indented);
            if (!File.Exists(resultPath))
            {
                Directory.CreateDirectory(jsonFolder);
            }
            File.WriteAllText(resultPath, jsonData);
            UIManager.Instance.GoTo(7);
        }

        public void Load(int index)
        {
            currentDataIndex = index;
            var resultPath = jsonFolder + "data" + index + ".json";
            if (!File.Exists(resultPath))
            {
                UIManager.Instance.GoTo(3);
                EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
                return;
            }
            var stringData = File.ReadAllText(resultPath);
            var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);
            foreach (var saveable in saveableList)
            {
                if (jsonData.dataDict.ContainsKey(saveable.GUID))
                    saveable.RestoreData(jsonData.dataDict[saveable.GUID]);
            }
        }

        public void Kill(int index)
        {
            var resultPath = jsonFolder + "data" + index + ".json";
            if (!File.Exists(resultPath))
            {
                UIManager.Instance.GoTo(3);
                return;
            }
            else
            {
                dataSlots[index] = null;
                File.Delete(resultPath);
            }
        }

        void SaveAction()
        {
            Save(currentDataIndex);
            EventHandler.CallUpdateGameStateEvent(GameState.Gameplay);
        }

        void LoadAction()
        {
            Load(currentDataIndex);
        }

        void OnUpdateGameStateEvent(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.Gameplay:
                    isPlaying  = true;
                    break;
                case GameState.Pause:
                    isPlaying = false;
                    break;
                case GameState.GameEnd:
                    isPlaying = false;
                    break;
                default: break;
            }
        }
    }


}
