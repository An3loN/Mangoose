using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Translatable : MonoBehaviour
{
    private static List<Translatable> translatableList = new List<Translatable>();
    [SerializeField] private string translationName;
    private Text text;

    void Start()
    {
        if(!text) text = GetComponent<Text>();
        translatableList.Add(this);
        text.text = Localization.GetNameFromBuffer(translationName);
    }
    void UpdateText()
    {
        text.text = Localization.GetNameFromBuffer(translationName);
    }
    public static void UpdateAll()
    {
        foreach(Translatable translatable in translatableList)
        {
            translatable.UpdateText();
        }
    }
    private void OnDestroy()
    {
        translatableList.Remove(this);
    }
}
