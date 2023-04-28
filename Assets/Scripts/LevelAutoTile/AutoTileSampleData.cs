using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/Auto Tile Sample", fileName = "AutoTileSample")]
[Serializable]
public class AutoTileSampleData : ScriptableObject
{
    public int[] tileSchematasInt;
    public byte[] tileSchematasByte
    {
        get
        {
            byte[] byteArray = new byte[tileSchematasInt.Length];
            for (int index = 0; index < tileSchematasInt.Length; index++)
            {
                byteArray[index] = (byte) tileSchematasInt[index];
            }
            return byteArray;
        }
    }
    public List<Sprite> rotations;
}
