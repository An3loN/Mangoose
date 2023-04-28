using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/Language Code Base", fileName = "LanguageCodes")]
[Serializable]
public class LanguageCodeBase : ScriptableObject
{
    public List<string> codes;
}