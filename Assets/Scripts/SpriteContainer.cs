using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/Sprite Container", fileName = "SpriteContainer")]
[Serializable]
public class SpriteContainer : ScriptableObject
{
    public List<Sprite> sprites = new List<Sprite>();
}
