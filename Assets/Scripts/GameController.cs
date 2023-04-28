using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public static bool playerAlive;
    [NonSerialized] public static int levelCompletedId;
    [NonSerialized] public static float levelCompletionTime;
    [SerializeField] private string transitionSceneName = "TransitionScene";
    [SerializeField] private EscapeMenuController escapeMenuController;
    [SerializeField] private float deathTime = 1f;

    void Start()
    {
        Instance = this;
        playerAlive = true;
        escapeMenuController.Initialize();
        Physics2D.IgnoreLayerCollision(3, 9);
        Time.timeScale = 1;
        TimerController.levelStartTime = Time.time;

        if (TimerController.levelId != SceneManager.GetActiveScene().buildIndex)
        {
            TimerController.levelId = SceneManager.GetActiveScene().buildIndex;
            
        }
        AudioSource[] audioSources = Resources.FindObjectsOfTypeAll<AudioSource>();
        foreach(AudioSource audioSource in audioSources)
        {
            audioSource.volume *= PlayerPrefs.GetFloat("volume");
        }
    }

    public void PerformPlayerDeath()
    {
        playerAlive = false;
        DeathController.PlayerDeathController.Kill();
        StartCoroutine(ReloadSceneInSeconds(deathTime));
    }
    IEnumerator ReloadSceneInSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    public void RestartCurrentLevel()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ExitToMainMenu()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(0);
    }
    public void SetNextLevel()
    {
        string activeSceneName = SceneManager.GetActiveScene().name;
        levelCompletionTime = Time.time - TimerController.levelStartTime;
        levelCompletedId = SceneManager.GetActiveScene().buildIndex;

        Save save = SaveController.GetSave();
        if (save != null && (!save.levelPassData[activeSceneName] || save.levelTimesData[activeSceneName] > levelCompletionTime))
        {
            SaveController.SetLevelCompleted(activeSceneName, levelCompletionTime);
            if(SaveController.EverythingIsMonkey())
            {
                LogSender.Instance.SendChallangeCompletionData();
            }
        }
        CameraTransitionController.Instance.ShowBlackScreenAndDo(SetTransitionLevel);
        
    }
    void SetTransitionLevel()
    {
        SceneManager.LoadScene(transitionSceneName);
    }
}
