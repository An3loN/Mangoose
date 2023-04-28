using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeMenuController : MonoBehaviour
{
    public static EscapeMenuController Instance;
    private SmoothUIPop pop;
    public bool isOpened = false;

    public void Initialize()
    {
        Instance = this;
        pop = GetComponent<SmoothUIPop>();
        if (!isOpened) gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void OnEscapePressed()
    {
        if(isOpened) CloseMenu();
        else OpenMenu();
    }
    public void OpenMenu()
    {
        pop.Show();
        isOpened = true;
        PlayerInputController.Instance.OnMenuOpen();
        GameController.Instance.PauseGame();
    }
    public void CloseMenu()
    {
        isOpened = false;
        PlayerInputController.Instance.OnMenuClose();
        GameController.Instance.ResumeGame();
        gameObject.SetActive(false);
    }
    public void OnRestartLevelButtonPressed()
    {
        GameController.Instance.RestartCurrentLevel();
    }
    public void OnMainMenuButtonPressed()
    {
        GameController.Instance.ExitToMainMenu();
    }
}
