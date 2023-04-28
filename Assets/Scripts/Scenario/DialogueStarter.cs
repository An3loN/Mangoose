using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueStarter : MonoBehaviour
{
    [SerializeField] DialogueController dialogueController;
    [SerializeField] string dialogueName;
    void Start()
    {
        dialogueController.StartDialogue(dialogueName);
    }
}
