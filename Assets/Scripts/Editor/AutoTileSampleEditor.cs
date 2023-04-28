using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Plastic.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(AutoTileSampleData))]
public class AutoTileSampleEditor : Editor
{
    private Byte[] schemata = {0,0,0,0,0,0,0,0};
    private const byte MAX_QUESTIONABLE_COUNT = 4;
    private SerializedProperty tileSchemataProperty; 
    void OnEnable()
    {
        tileSchemataProperty = serializedObject.FindProperty("tileSchematasInt");
    }

    string GetButtonTextBySchemataData(byte data)
    {
        switch(data)
        {
            case 0:
                return "0";
            case 1:
                return "#";
            case 2:
                return "?";
            default:
                return "0";
        }

    }

    byte[] GetDiscreteArrayWithSetValueByIndex(byte[] array, int index, byte value)
    {
        byte[] newArray = new byte[8];
        for (int i = 0; i < 8; i++)
        {
            if (i == index) newArray[index] = value;
            else if (array[i] == 2 && i < index)
            {
                newArray[i] = 0;
            }
            else newArray[i] = array[i];
        }
        return newArray;
    }
    
    List<byte> GetByteSchematas(byte[] schemata)
    {
        List<byte> byteSchematas = new List<byte>();
        byte byteSchemata = 0;
        for (int index = 0; index < 8; index++)
        {
            switch (schemata[index])
            {
                case 1:
                    byteSchemata += (byte) Mathf.Pow(2, index);
                    break;
                case 2:
                    byteSchematas.AddRange(GetByteSchematas(GetDiscreteArrayWithSetValueByIndex(schemata, index, 1)));
                    break;
            }
        }
        byteSchematas.Add(byteSchemata);
        return byteSchematas;
    }
    
    void SetSerialisedByteArrayData(SerializedProperty serializedProperty, List<byte> array)
    {
        int arrayLength = array.Count;
        serializedProperty.arraySize = arrayLength;
        SerializedProperty arrayElementProperty;
        for (int index = 0; index < arrayLength; index++)
        {
            arrayElementProperty = serializedObject.FindProperty($"tileSchematasInt.Array.data[{index}]");
            arrayElementProperty.intValue = array[index];
        }
    }
    
    void UpdateData()
    {
        List<byte> byteSchematas = GetByteSchematas(schemata);
        SetSerialisedByteArrayData(tileSchemataProperty, byteSchematas);
        serializedObject.ApplyModifiedProperties();
    }

    byte GetQuestionableTilesCount(byte[] schemata)
    {
        byte count = 0;
        for (int index = 0; index < 8; index++)
        {
            if (schemata[index] == 2) count++;
        }

        return count;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        GUILayout.Space(100f);
        GUILayout.BeginArea(new Rect(20, 160, 150, 150));
        
        for (int line = 0; line < 3; line++)
        {
            GUILayout.BeginHorizontal();
            for (int row = 0; row < 3; row++)
            {
                if (line == 1 && row == 1)
                {
                    GUILayout.Box("#####");
                    continue;
                }
                int schemataCell = LevelAutoTile.schemataMatrixOrder[line, row];
                bool pressed = GUILayout.Button(GetButtonTextBySchemataData(schemata[schemataCell]));
                if (pressed)
                {
                    schemata[schemataCell]++;
                    if (GetQuestionableTilesCount(schemata) > MAX_QUESTIONABLE_COUNT && schemata[schemataCell] > 1)
                    {
                        schemata[schemataCell] = 0;
                    }
                    else if (schemata[schemataCell] > 2)
                    {
                        schemata[schemataCell] = 0;
                    }
                    UpdateData();
                }
            }
            GUILayout.EndHorizontal();
        }

        /*if (schemata != lastSchemata)
        {
            lastSchemata = schemata;
        }*/
        GUILayout.EndArea();
        
    }
}
