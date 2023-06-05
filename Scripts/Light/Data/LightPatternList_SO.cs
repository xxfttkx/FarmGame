using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightPatternList_SO", menuName = "Light/Light Patten")]
public class LightPatternList_SO : ScriptableObject
{
    public List<LightDetails> LightPatternList;

    /// <summary>
    /// 根据季节和周期返回灯光详情
    /// </summary>
    /// <param name="season">季节</param>
    /// <param name="lightShift">周期</param>
    /// <returns></returns>
    public LightDetails GetLightDetails(Season season, LightShift lightShift)
    {
        return LightPatternList.Find(l => l.season == season && l.lightShift == lightShift);
    }
}


[System.Serializable]
public class LightDetails
{
    public Season season;
    public LightShift lightShift;
    public Color lightColor;
    public float lightAmount;
}