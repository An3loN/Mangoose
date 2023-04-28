using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/Level Time Object", fileName = "LevelTime")]
[Serializable]
public class LevelTimeDataObject : ScriptableObject
{
    public float bTime;
    public float aTime;
    public float sTime;
    public float monkeyTime;

    public int GetRankId(float time)
    {
        if (time < monkeyTime) return 5;
        if (time < sTime) return 4;
        if (time < aTime) return 3;
        if (time < bTime) return 2;
        return 1;
    }
    public Sprite GetRankImage(float time)
    {
        int rankId = GetRankId(time);
        return Resources.Load<Sprite>($"RankImages/RankImage{rankId}");
    }
}
