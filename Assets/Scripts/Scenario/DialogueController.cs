using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private List<string> personsShortnames;
    [SerializeField] private List<Animator> personsAnimators;
    [SerializeField] private List<GameObject> triggers;
    [SerializeField] private List<Transform> cameraPoints;
    [SerializeField] private List<Transform> goPoints;
    [SerializeField] private ThemeController themeController;
    [SerializeField] private DialogueWindow dialogueWindow;
    [SerializeField] private UnityEvent onDialogueEnd;
    Dialogue dialogue;

    public void StartDialogue(string dialogueName)
    {
        TextAsset text = Resources.Load($"Dialogues/{dialogueName}") as TextAsset;
        List<string> lines = new List<string>(text.text.Split('\n'));
        dialogue = new Dialogue(dialogueName, dialogueWindow, themeController, ref lines, GetPersonsDict(), ref triggers, ref cameraPoints, ref goPoints);
        dialogue.onDialogueEnd += OnDialogueEnd;
        //dialogueWindow.Show();
        StartCoroutine(dialogue.PerformCoroutine());
    }
    Dictionary<string, Person> GetPersonsDict()
    {
        Dictionary<string, Person> personsDict = new Dictionary<string, Person>();
        for(int personIndex = 0; personIndex < personsShortnames.Count; personIndex++)
        {
            string personName = personsShortnames[personIndex];
            personsDict.Add(personName, new Person(personName, personsAnimators[personIndex]));
        }
        return personsDict;
    }
    public void OnDialogueEnd()
    {
        //dialogueWindow.Hide();
        onDialogueEnd.Invoke();
        //SceneManager.LoadScene(SaveController.GetLevel(dialogueIndex));
    }

    public void OnNextSpeech(CallbackContext context)
    {
        if (dialogue != null) dialogue.OnNextSpeech(context);
    }
    private void OnDisable()
    {
        GetComponent<PlayerInput>().actions = null;
    }
}
