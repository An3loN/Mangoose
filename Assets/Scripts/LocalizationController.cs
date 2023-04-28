using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationController : MonoBehaviour
{
    [SerializeField] private Localization.BufferVariations bufferVariation;
    private void Awake()
    {
        Localization.Init();
        Localization.LoadBuffer(bufferVariation);
        Localization.onLanguageChange += Translatable.UpdateAll;
    }
    public void SetLanguage(string language)
    {
        Localization.SetLanguage(language);
    }
}
