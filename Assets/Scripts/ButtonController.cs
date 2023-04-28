using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class ButtonController : MonoBehaviour
{
    public event Action OnButtonPressed;
    
    [SerializeField] private Sprite pressedSprite;
    [SerializeField] private Sprite notPressedSprite;
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private AudioClip releaseSound;
    [Tooltip("Sends OnTriggered message to this object")]
    [SerializeField] private List<GameObject> objectsToTrigger = new List<GameObject>();
    [SerializeField] private bool applyOnRelease;
    [SerializeField] private float pressTime = 0.4f;
    //private float checkTime = 0.05f;

    private bool isBeingPressed = false;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool someoneInTrigger = false;
    private int objectsInTriggerZoneCount = 0;

    IEnumerator ButtonReleaseCoroutine()
    {
        yield return new WaitForSeconds(pressTime);
        while (someoneInTrigger)
        {
            yield return null;
        }
        if (applyOnRelease) SendTriggerMessage();
        isBeingPressed = false;
        spriteRenderer.sprite = notPressedSprite;
        audioSource.clip = releaseSound;
        audioSource.Play();
    }

    void SendTriggerMessage()
    {
        if (objectsToTrigger.Count == 0) return;
        foreach (GameObject objectToTrigger in objectsToTrigger) objectToTrigger.SendMessage("OnTriggered");        
    }
    void ApplyPressing()
    {
        isBeingPressed = true;
        spriteRenderer.sprite = pressedSprite;
        SendTriggerMessage();
        StartCoroutine(ButtonReleaseCoroutine());
        audioSource.clip = pressSound;
        audioSource.Play();
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        OnButtonPressed += ApplyPressing;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isBeingPressed) 
            OnButtonPressed?.Invoke();
        someoneInTrigger = true;
        objectsInTriggerZoneCount++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        objectsInTriggerZoneCount--;
        if (objectsInTriggerZoneCount == 0) someoneInTrigger = false;
    }
}
