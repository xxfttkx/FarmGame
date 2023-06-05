using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Map;
using UnityEngine.SceneManagement;

public class Sprinkler : MonoBehaviour
{
    // //TODO 改场景名
    // public string sceneName;


    // private void Awake() {
    //     sceneName = SceneManager.GetActiveScene().name;
    // }
    // private void OnEnable()
    // {
    //     EventHandler.GameDayEvent1 += OnGameDayEvent1;
    // }
    // private void OnDisable()
    // {
    //     EventHandler.GameDayEvent1 -= OnGameDayEvent1;
    // }
    // //TODO 要不在场景中也能执行  要写在itemManager中
    // public void OnGameDayEvent1(int day, Season season)
    // {
    //     Vector2Int sprinklerpos = new
    //         Vector2Int((int)Mathf.Floor(this.transform.position.x), (int)Mathf.Floor(this.transform.position.y));
    //     for (int i = -1; i <= 1; ++i)
    //     {
    //         for (int j = -1; j <= 1; ++j)
    //         {
    //             if ((i != 0 || j != 0) && ((i == 0 || j == 0)))
    //             {
    //                 var pos = new Vector2Int(sprinklerpos.x + i, sprinklerpos.y + j);
    //                 var tileDetails = GridMapManager.Instance.GetTileDetailsByPosAndSceneName(pos, sceneName);
    //                 if (tileDetails != null && tileDetails.daysSinceDug >= 0)
    //                 {
    //                     tileDetails.daysSinceDug += 1;
    //                     tileDetails.daysSinceWatered = 1;
    //                     if(sceneName == SceneManager.GetActiveScene().name)
    //                     {
    //                         GridMapManager.Instance.SetWaterGround(tileDetails);
    //                     }
    //                 }
    //             }
    //         }
    //     }
    // }
}
