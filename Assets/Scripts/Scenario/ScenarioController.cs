using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioController : MonoBehaviour
{
    public static ScenarioController Instance;
    Dictionary<string, object> spawnedObjects = new Dictionary<string, object>();
    
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public void PerformScenario(string scenarioName)
    {
        Scenario scenario = Resources.Load($"/Scenarios/{scenarioName}") as Scenario;
    }
    public void PerformScenario(Scenario scenario)
    {
        StartCoroutine(ScenarioPerformingCoroutine(scenario));
    }
    
    void CheckArgumentCount(string commandName, int argumentCountExpected, int argumentCountGot)
    {
        if(argumentCountGot != argumentCountExpected) 
        {
            throw new ArgumentException($"\"{commandName}\" expects {argumentCountExpected} arguments, not {argumentCountGot}.");
        }
    }

    IEnumerator ScenarioPerformingCoroutine(Scenario scenario)
    {
        string commandMatter;
        foreach (var command in scenario.commands)
        {
            string[] commandWords = command.Split(' ');
            commandMatter = commandWords[0];
            switch (commandMatter)
            {
                case "spawn":
                    {
                        CheckArgumentCount(commandMatter, 3, commandWords.Length);
                        string prefabName = commandWords[1];
                        string gameObjectName = commandWords[2];
                        GameObject gameObjectToSpawn = Resources.Load($"Prefabs/{prefabName}") as GameObject;
                        spawnedObjects.Add(gameObjectName, Instantiate(gameObjectToSpawn));
                        break;
                    }
                    
                case "wait": //waits in miliseconds
                    {
                        CheckArgumentCount(commandMatter, 2, commandWords.Length);
                        if(Int32.TryParse(commandWords[1], out int timeToWait))
                        {
                            yield return new WaitForSeconds(timeToWait/1000f);
                        }
                        else throw new ArgumentException("Wrong time format.");
                        break;
                    }
                
                case "destroy":
                    {
                        CheckArgumentCount(commandMatter, 2, commandWords.Length);
                        string gameObjectName = commandWords[1];
                        Destroy((spawnedObjects[gameObjectName] as MonoBehaviour).gameObject);
                        spawnedObjects.Remove(gameObjectName);
                        break;
                    }
                    
                    //case "wait_for_input"
                    //case "play_animation"
                    //case "disable_input"
            }
        }
    }
}
