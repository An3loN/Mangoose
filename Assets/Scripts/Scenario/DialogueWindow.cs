using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class DialogueWindow : MonoBehaviour
{
    [SerializeField] private Image personImage;
    [SerializeField] private Text personName;
    [SerializeField] private Text text;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Say(string textToSay, string displayName, Person person)
    {
        personName.text = displayName;
        text.text = textToSay;
    }
}
