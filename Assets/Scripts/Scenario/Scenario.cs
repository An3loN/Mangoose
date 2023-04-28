using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Scenario", menuName = "ScriptableObjects/Scenario", order = 1)]
[Serializable]
public class Scenario : ScriptableObject
{
    public List<string> commands = new List<string> {  };
}
