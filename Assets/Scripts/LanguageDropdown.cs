using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(Dropdown))]
public class LanguageDropdown : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] LocalizationController localizationController;
    private Dropdown dropdown;
    private void Awake()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(delegate { OnValueChanged(); });
        AddOptions();
        SetDefaultValue();
    }
    void SetDefaultValue()
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == Localization.LanguageCode)
            {
                dropdown.value = i;
                break;
            }
        }
    }
    private void Start()
    {
        
    }
    private void OnValueChanged()
    {
        localizationController.SetLanguage(dropdown.options[dropdown.value].text);
    }
    private void AddOptions()
    {
        dropdown.options.Clear();
        foreach (string code in Localization.LanguageCodes)
        {
            dropdown.options.Add(new Dropdown.OptionData(code));
        }
    }
}
