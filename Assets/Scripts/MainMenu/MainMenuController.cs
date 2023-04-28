using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance;

    [SerializeField] private float menuUnfoldTime = 2f;
    [SerializeField] private Transform backgroundImageTransform;
    [SerializeField] private GameObject pressStartObject;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject chooseLevelButton;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private SmoothUIPop mainMenu;
    [SerializeField] private SmoothUIPop levelChooseMenu;
    [SerializeField] private SmoothUIPop optionsMenu;
    private Resolution[] resolutions;
    Resolution selectedResolution;
    private bool inFullscreen;
    private bool waitForAnyInput = true;
    private PlayerInput playerInput;
    private SmoothUIPop activeMenu;


    private void Awake()
    {
        LoadSettings();
    }
    void Start()
    {
        Instance = this;

        activeMenu = mainMenu;
        playerInput = GetComponent<PlayerInput>();

        resolutions = Screen.resolutions;

        
        CreateResolutionDropdown();

        Time.timeScale = 1;
        if (!SaveController.ValidateSave())
        {
            DestroyImmediate(continueButton);
            DestroyImmediate(chooseLevelButton);
        }
        playerInput.actions.FindActionMap("UI").Disable();
    }

    private void CreateResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);
            if (Mathf.Approximately(resolutions[i].width, selectedResolution.width) && Mathf.Approximately(resolutions[i].height, selectedResolution.height))
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    private void LoadSettings()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume");

        selectedResolution = new Resolution();
        selectedResolution.width = PlayerPrefs.GetInt("resolution_x", Screen.currentResolution.width);
        selectedResolution.height = PlayerPrefs.GetInt("resolution_y", Screen.currentResolution.height);
        selectedResolution.refreshRate = PlayerPrefs.GetInt("refresh", Screen.currentResolution.refreshRate);

        //fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen") == 1;
        fullscreenToggle.isOn = Screen.fullScreen;
        inFullscreen = fullscreenToggle.isOn;

        Screen.SetResolution(
            selectedResolution.width,
            selectedResolution.height,
            fullscreenToggle.isOn
        );
    }
    
    public void OnStartInput(InputAction.CallbackContext context)
    {
        if (waitForAnyInput)
        {
            waitForAnyInput = false;
            UnfoldMenu();
            playerInput.actions.FindActionMap("UI").Enable();
        }
    }

    void UnfoldMenu()
    {
        Destroy(pressStartObject);
        backgroundImageTransform.DOMoveY(0, menuUnfoldTime).onComplete = () =>
        {
            mainMenu.Show();
        };
    }

    public void OnContinue()
    {
        SceneManager.LoadScene(SaveController.GetFirstIncompletedLevelName());
    }
    public void OnNewGame()
    {
        SaveController.CreateSave();
        TimerController.newGameStartTime = Time.time;
        SceneManager.LoadScene(SaveController.GetFirstIncompletedLevelName());
    }
    public void StartLevel(string levelName)
    {
        SceneManager.LoadScene($"{levelName}");
    }
    public void OnChooseLevel()
    {
        SetMenu(levelChooseMenu);
    }
    public void OnOptions()
    {
        SetMenu(optionsMenu);
        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen") == 1;
        resolutionDropdown.captionText.text = $"{Screen.currentResolution.width}x{Screen.currentResolution.height}";
    }
    public void OnMainMenu()
    {
        if (activeMenu == mainMenu) return;
        SetMenu(mainMenu);
    }

    public void OnVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("volume", value);
        audioSource.volume = value;
    }

    public void OnFullscreenCheck(bool check)
    {
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, check);
        inFullscreen = check;
        PlayerPrefs.SetInt("fullscreen", check ? 1 : 0);
    }

    public void OnResolutionChanged(Int32 resolutionIndex)
    {
        selectedResolution = resolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, inFullscreen);
        PlayerPrefs.SetInt("resolution_x", selectedResolution.width);
        PlayerPrefs.SetInt("resolution_y", selectedResolution.height);
        PlayerPrefs.SetInt("refresh", selectedResolution.refreshRate);
    }

    void SetMenu(SmoothUIPop menu)
    {
        SmoothUIPop lastMenu = activeMenu;
        activeMenu = menu;
        lastMenu.Hide();
        activeMenu.Show();
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
