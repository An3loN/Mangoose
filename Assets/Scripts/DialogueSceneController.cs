using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

[RequireComponent(typeof(ScenarioController))]
public class DialogueSceneController : MonoBehaviour
{
    public static int dialogueIndex;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private CameraTransitionController cameraTransitionController;
    private void Start()
    {
        try
        {
            dialogueController.StartDialogue(CalculateDialogue());
        }
        catch (Exception e) 
        { 
            Debug.Log(e.Message);
            OnDialogueEnd();
        }
    }
    public void OnDialogueEnd()
    {
        cameraTransitionController.ShowBlackScreenAndDo(
            () => SceneManager.LoadScene(SaveController.GetLevel(dialogueIndex)));
    }
    string CalculateDialogue()
    {
        if (dialogueIndex == 1) return "dialogue1";
        throw new Exception($"No dialogue with index {dialogueIndex}");
    }

    
}