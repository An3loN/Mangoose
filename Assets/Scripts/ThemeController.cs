using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeController : MonoBehaviour
{
    [SerializeField] ParentColor gridParentColor;
    [SerializeField] List<Color> colors;
    [SerializeField] List<GameObject> backgrounds;

    public void SetTheme(int ind)
    {
        foreach (var bg in backgrounds) bg.SetActive(false);
        backgrounds[ind].SetActive(true);

        gridParentColor.Color = colors[ind];
    }
}
