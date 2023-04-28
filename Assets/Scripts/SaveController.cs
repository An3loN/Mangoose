using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveController
{
    static Save save;
    static Dictionary<int, string> dialogueLevels = new Dictionary<int, string> {
        { 1, "DialogueScene" },
        { 16, "FinalScene" },
    };

    public enum LevelRanks
    {
        C = 0,
        B,
        A,
        S,
        Monkey
    }
    public static void CreateSave()
    {
        string saveString;
        save = new Save();
        List<string> levelNames = GetLevelNames();
        foreach(string levelName in levelNames)
        {
            save.levelPassData.Add(levelName, false);
            save.levelTimesData.Add(levelName, -1f);
        }
        saveString = JsonConvert.SerializeObject(save);
        PlayerPrefs.SetString("ActiveSave", saveString);
    }
    static Save LoadSave()
    {
        if (!PlayerPrefs.HasKey("ActiveSave")) return null;
        Save loadedSave = JsonConvert.DeserializeObject<Save>(PlayerPrefs.GetString("ActiveSave"));
        return loadedSave;
    }
    public static bool ValidateSave()
    {
        if (!PlayerPrefs.HasKey("ActiveSave"))
        {
            CreateSave();
            return false;
        }
        Save playerSave = LoadSave();

        List<string> levelNames = GetLevelNames();
        if(playerSave.levelPassData.Count != levelNames.Count)
        {
            if(levelNames.Count > playerSave.levelPassData.Count)
            {
                for(int levelIndex = playerSave.levelPassData.Count + 1; levelIndex <= levelNames.Count; levelIndex++)
                {
                    playerSave.levelPassData.Add($"Level{levelIndex}", false);
                    playerSave.levelTimesData.Add($"Level{levelIndex}", 10000f);
                }
                save = playerSave;
            }
            else
            {
                CreateSave();
            }
        }
        else
        {
            save = playerSave;
        }
        return true;
    }
    static List<string> GetLevelNames()
    {
        List<string> levelNames = new List<string>();
        var sceneNumber = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < sceneNumber; i++)
        {
            string sceneName = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            if (sceneName.Substring(0, 5) == "Level")
                levelNames.Add(sceneName);
        }
        return levelNames;
    }
    public static string GetFirstIncompletedLevelName()
    {
        int levelCount = save.levelPassData.Count;
        for(int levelIndex = 1; levelIndex <= levelCount; levelIndex++)
        {
            string levelName = $"Level{levelIndex}";
            if (!save.levelPassData[levelName])
            {
                return GetScene(levelIndex);
            }
        }
        return "Level1";
    }
    public static string GetLevel(int index)
    {
        int levelCount = save.levelPassData.Count;
        if (index <= levelCount)
        {
            return $"Level{index}";
        }
        else throw new ArgumentOutOfRangeException($"Level {index} is out of range");
    }
    public static string GetScene(int index)
    {
        int levelCount = save.levelPassData.Count;
        if (index <= levelCount && save.levelPassData[$"Level{index}"])
        {
            return $"Level{index}";
        }

        if (dialogueLevels.ContainsKey(index))
        {
            DialogueSceneController.dialogueIndex = index;
            return dialogueLevels[index];
        }
        if (index <= levelCount)
        {
            return $"Level{index}";
        }
        throw new Exception($"No level presented with index {index} either in dialogues and levels");

    }
    public static string GetNextLevel(int levelIndex)
    {
        return GetScene(levelIndex + 1);
    }
    public static void SetLevelCompleted(string name)
    {
        if (save == null) return;
        save.levelPassData[name] = true;
        SaveCurrentData();
    }
    public static void SetLevelCompleted(string name, float time)
    {
        if (save == null) return;
        save.levelPassData[name] = true;
        save.levelTimesData[name] = time;
        SaveCurrentData();
    }
    public static void SaveCurrentData()
    {
        string saveString = JsonConvert.SerializeObject(save);
        PlayerPrefs.SetString("ActiveSave", saveString);
    }
    public static Save GetSave()
    {
        if(save == null) save = LoadSave();
        return save;
    }
    public static bool EverythingIsMonkey()
    {
        foreach(string level in save.levelTimesData.Keys)
        {
            if (save.levelTimesData[level] < 0) return false;
            int levelId = Int32.Parse(level.Substring(5));
            LevelTimeDataObject levelTimeData = Resources.Load<LevelTimeDataObject>($"LevelTimeObjects/LevelTime{levelId}");
            int rankId = levelTimeData.GetRankId(save.levelTimesData[level]);
            if (rankId < 4)
            {
                return false;
            }
        }
        return true;
    }
    public static LevelRanks GetMinLevelRank()
    {
        int minLevelRank = 1000;
        foreach (string level in save.levelTimesData.Keys)
        {
            if (save.levelTimesData[level] < 0) continue;
            int levelId = Int32.Parse(level.Substring(5));
            LevelTimeDataObject levelTimeData = Resources.Load<LevelTimeDataObject>($"LevelTimeObjects/LevelTime{levelId}");
            int rankId = levelTimeData.GetRankId(save.levelTimesData[level]);
            if (rankId < minLevelRank)
            {
                minLevelRank = rankId;
            }
        }

        return (LevelRanks)minLevelRank;
    }
}
