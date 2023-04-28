using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Dialogue
{
    public Action onDialogueEnd;
    private List<string> dialogueLines;
    private List<GameObject> triggers;
    private List<Transform> cameraPoints;
    private List<Transform> goPoints;
    private List<GameObject> backgrounds;
    private Dictionary<string, Person> persons;
    private Dictionary<string, string> localizationNames;
    private ThemeController themeController;
    private DialogueWindow dialogueWindow;
    private string dialogueName;
    private bool inputTrigger = false;
    private float EMOTE_JUMP_HEIGHT = 0.5f;
    private float EMOTE_JUMP_DURATION = 0.5f;


    public Dialogue(string dialogueName, DialogueWindow dialogueWindow, ThemeController themeController, ref List<string> dialogueLines, 
        Dictionary<string, Person> persons, ref List<GameObject> triggers, ref List<Transform> cameraPoints, ref List<Transform> goPoints)
    {
        this.dialogueLines = dialogueLines;
        this.persons = persons;
        this.dialogueName = dialogueName;
        this.dialogueWindow = dialogueWindow;
        this.themeController = themeController;
        this.triggers = triggers;
        this.cameraPoints = cameraPoints;
        this.goPoints = goPoints;
        onDialogueEnd += OnDialogueEnd;
    }
    
    public IEnumerator PerformCoroutine()
    {
        foreach (string line in dialogueLines)
        {
            string[] words = line.Split(" ");
            if (words[0] == "wait")
            {
                float seconds = Int32.Parse(words[1].Trim()) / 1000f;
                yield return new WaitForSeconds(seconds);
                continue;
            }
            if (words[0] == "trigger")
            {
                int triggerInd = Int32.Parse(words[1].Trim());
                triggers[triggerInd].SendMessage("OnTriggered");
                continue;
            }
            if (words[0] == "theme")
            {
                int themeInd = Int32.Parse(words[1].Trim());
                themeController.SetTheme(themeInd);
                continue;
            }
            if (words[0] == "camera")
            {
                switch(words[1])
                {
                    case "pos":
                        {
                            //set camera position to point words[2]
                            int posIndex = Int32.Parse(words[2].Trim());
                            if (posIndex < 0 || posIndex > cameraPoints.Count) throw new Exception($"{posIndex} is not valid camera point");
                            var pos = cameraPoints[posIndex].position;
                            Func<bool> waiter = 
                                CameraTransitionController.Instance.SetPosition(pos.x, pos.y, 0.5f);
                            if (words.Length > 3 && words[3].Trim() == "wait")
                                yield return new WaitUntil(waiter);
                            break;
                        }
                    case "zoom":
                        {
                            //set camera zoom to words[2]
                            float zoom = float.Parse(words[2].Trim());
                            if (zoom < 1) throw new Exception($"{zoom} is not valid zoom");
                            Func<bool> waiter = 
                                CameraTransitionController.Instance.SetZoom(zoom, 0.5f);
                            if (words.Length == 4 && words[3].Trim() == "wait")
                                yield return new WaitUntil(waiter);
                            break;
                        }
                    case "showBlack":
                        {
                            Func<bool> waiter =
                                CameraTransitionController.Instance.ShowBlackScreen();
                            if (words.Length == 3 && words[2].Trim() == "wait")
                                yield return new WaitUntil(waiter);
                            break;
                        }
                    case "hideBlack":
                        {
                            Func<bool> waiter =
                                CameraTransitionController.Instance.HideBlackScreen();
                            if (words.Length == 3 && words[2].Trim() == "wait")
                                yield return new WaitUntil(waiter);
                            break;
                        }
                }
                continue;
            }

            //if (words.Length != 3) throw new Exception($"Not expected argument amount in line:\n{line}");
            if (persons.ContainsKey(words[0]))
            {
                string personName = words[0].Trim();
                switch(words[1])
                {
                    case "playAnim":
                        {
                            persons[personName].animator.SetTrigger(words[2].Trim());
                            break;
                        }
                    case "go":
                        {
                            Person person = persons[personName];
                            int positionNumber = Int32.Parse(words[2]);
                            float targetX = goPoints[positionNumber].position.x;
                            bool done = false;
                            float speed = 2f;
                            float duration = Mathf.Abs(targetX - person.animator.transform.position.x)/speed;
                            person.animator.transform.DOMoveX(targetX, duration).SetEase(Ease.Linear).OnComplete(() => {
                                person.animator.SetTrigger("Idle");
                                done = true;
                            });
                            person.animator.SetTrigger("Run");

                            bool facingRight = person.animator.transform.localScale.x > 0;
                            if (facingRight && (targetX < person.animator.transform.position.x) ||
                                !facingRight && (targetX > person.animator.transform.position.x))
                            {
                                person.animator.transform.localScale = new Vector3(
                                    -person.animator.transform.localScale.x,
                                    person.animator.transform.localScale.y,
                                    person.animator.transform.localScale.z);
                            }

                            Func<bool> waiter = () => done;
                            if (words.Length == 4 && words[3].Trim() == "wait")
                            {
                                yield return new WaitUntil(waiter);
                            }
                            break;
                        }
                    case "emote":
                        {
                            Person person = persons[personName];
                            EmotionController emotionController = person.animator.GetComponent<EmotionController>();
                            Func<bool> waiter = emotionController.PlayEmote(words[2].Trim());
                            Vector3[] path =
                            {
                                person.animator.transform.position + Vector3.up * EMOTE_JUMP_HEIGHT,
                                person.animator.transform.position
                            };
                            person.animator.transform.DOLocalPath(path, EMOTE_JUMP_DURATION, PathType.Linear, PathMode.Sidescroller2D);
                            if (words.Length == 4 && words[3].Trim() == "wait")
                                yield return new WaitUntil(waiter);
                            break;
                        }
                    case "setChild":
                        {
                            Person person = persons[personName];
                            Person caught = persons[words[2].Trim()];
                            caught.animator.transform.SetParent(person.animator.transform, true);
                            break;
                        }
                    default:
                        throw new Exception($"There is no command {words[1]}");
                }
            }
            else throw new Exception($"There is no person {words[0]}");
        }
        onDialogueEnd.Invoke();
    }
    IEnumerator WaitForInput()
    {
        while (!inputTrigger) yield return null;
        inputTrigger = false;
    }
    public void OnNextSpeech(CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            inputTrigger = true;
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            inputTrigger = false;
        }
    }

    void OnDialogueEnd()
    {
        dialogueLines.Clear();
    }
}
