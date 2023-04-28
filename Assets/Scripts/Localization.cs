using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Localization
{
    private static readonly string LANGUAGE_CODES_DIR = "Languages/LanguageCodes";
    public enum BufferVariations
    {
        MainMenu = 0,
        Level,
        TransitionScene,
        DialogueScene
    }
    private static string languageRootPath
    {
        get
        {
            return $"Languages/{languageCode}";
        }
    }
    private static string languageCode;
    private static List<string> languageCodes;
    private static BufferVariations bufferVariation;
    private static Dictionary<string, string> nameBuffer = new Dictionary<string, string>();
    public static string LanguageCode
    {
        get => languageCode;
    }
    public static List<string> LanguageCodes
    {
        get
        {
            if (languageCodes == null) languageCodes = Resources.Load<LanguageCodeBase>(LANGUAGE_CODES_DIR).codes;
            return languageCodes;
        }
    }
    public static Action onLanguageChange = OnLanguageChange;

    private static bool languageCodeExists(string languageCode)
    {
        return LanguageCodes.Contains(languageCode);
    }
    public static void SetLanguage(string languageToSetCode)
    {
        languageToSetCode = languageToSetCode.ToUpper();
        if (languageCode != languageToSetCode)
        {
            if (!languageCodeExists(languageToSetCode)) throw (new Exception($"No language with code {languageToSetCode} found."));
            languageCode = languageToSetCode;
            SaveLanguage(languageCode);
            onLanguageChange.Invoke();
        }
    }
    static void OnLanguageChange()
    {
        LoadBuffer(bufferVariation);
    }
    public static void LoadBuffer(BufferVariations variationToSet)
    {
        nameBuffer.Clear();
        bufferVariation = variationToSet;
        nameBuffer = LoadNamesFromFile(bufferVariation.ToString());                
    }

    public static string GetNameFromBuffer(string objectName)
    {
        if (nameBuffer.ContainsKey(objectName)) return nameBuffer[objectName];
        else throw new MissingFieldException($"There is no {objectName} in name buffer");
    }

    public static List<string> LoadLinesFromFile(string fileName)
    {
        string filePath = $"{languageRootPath}/{fileName}";
        TextAsset text = Resources.Load(filePath) as TextAsset;
        List<string> lines = new List<string>(text.text.Split('\n'));
        return lines;
    }

    public static Dictionary<string, string> LoadNamesFromFile(string fileName)
    {
        string filePath = $"{languageRootPath}/{fileName}";
        Dictionary<string, string> names = new Dictionary<string, string>();
        List<string> lines = LoadLinesFromFile(fileName);
        foreach (string line in lines)
        {
            if (line == "\n") continue;
            string[] namePair = line.Split(" ", 2);
            if (namePair.Length != 2) throw new FormatException($"Failed to read translation from {filePath}");
            names.Add(namePair[0], namePair[1]);
        }
        return names;
    }
    private static void SaveLanguage(string languageCode)
    {
        PlayerPrefs.SetString("language", languageCode); 
    }
    private static string LoadLanguage()
    {
        if(PlayerPrefs.HasKey("language")) return PlayerPrefs.GetString("language");
        else return LanguageCodes[0];
    }
    public static void Init()
    {
        languageCodes = Resources.Load<LanguageCodeBase>(LANGUAGE_CODES_DIR).codes;
        if (languageCode == null)
        {
            languageCode = LoadLanguage();
        }
    }
}
