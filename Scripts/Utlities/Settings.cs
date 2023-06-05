using System;
using UnityEngine;
using System.Collections.Generic;

public class Settings
{
    public const float itemFadeDuration = 0.35f;
    public const float targetAlpha = 0.45f;

    //时间相关
    public const float secondThreshold = 0.01f;    //数值越小时间越快
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 30;
    public const int seasonHold = 3;
    public const int monthHold = 4; //1~4

    //Transition
    public const float fadeDuration = 0.8f;

    //割草数量限制
    public const int reapAmount = 20;//todo :2

    //NPC网格移动
    public const float gridCellSize = 1;
    public const float gridCellDiagonalSize = 1.41f;
    public const float pixelSize = 0.05f;   //20*20 占 1 unit
    public const float animationBreakTime = 5f; //动画间隔时间
    public const int maxGridSize = 9999;

    //灯光
    public const float lightChangeDuration = 25f;
    public static TimeSpan morningTime = new TimeSpan(5, 0, 0);
    public static TimeSpan nightTime = new TimeSpan(19, 0, 0);

    public static Vector3 playerStartPos = new Vector3(-1.7f, -5f, 0);

    //TODO：start new game 时用这个
    public static string playerStartScene = "01.Field";
    public const int playerStartMoney = 100;

    public static Dictionary<string, string> nameChange = new Dictionary<string, string>()
    {
        {"Mayor", "镇长"},
        {"Girl01", "袁书鹏"},
        {"Girl02", "黄鹏飞"},
        {"Player","谢西风"}
    };
    public const float dailyGenerateStone = 0.01f;
    public const float dailyGenerateGrass = 0.005f;
    public const float dailyGenerateTree = 0.005f;
    public const float dialogueTextShowTime = 0.3f;
    public const int pixelsPerUnit = 20;
}
