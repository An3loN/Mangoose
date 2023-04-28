using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save
{
    public Dictionary<string, bool> levelPassData;
    public Dictionary<string, float> levelTimesData;
    public Save()
    {
        levelPassData = new Dictionary<string, bool>();
        levelTimesData = new Dictionary<string, float>();
    }
}
