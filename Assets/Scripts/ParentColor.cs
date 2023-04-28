using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ParentColor : MonoBehaviour
{
    private Color color;
    public Color Color 
    {
        get
        {
            return color;
        }
        set
        {
            color = value;
            SetColor(color);
        }
    }
    private void SetColor(Color color)
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach(var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = color;
        }
        Tilemap[] tilemaps = GetComponentsInChildren<Tilemap>();
        foreach(var tilemap in tilemaps)
        {
            tilemap.color = color;
        }
    }
}
