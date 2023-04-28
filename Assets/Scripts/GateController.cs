using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
public class GateController : MonoBehaviour
{
    [SerializeField] private bool isOpened = false;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private int activeCoroutinesCount = 0;
    [SerializeField] GameObject smokePrefab;

    void Start()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        if (isOpened) Open();
    }

    void Open()
    {
        animator.SetTrigger("Open");
        isOpened = true;
    }

    void Close()
    {
        animator.SetTrigger("Close");
        isOpened = false;
    }
    
    IEnumerator DelayedCloseCoroutine(float time)
    {
        activeCoroutinesCount += 1;
        yield return new WaitForSeconds(time);
        activeCoroutinesCount -= 1;
        if(activeCoroutinesCount == 0) Close();
    }
    void OnTriggered()
    {
        if (isOpened)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    void OnTriggeredParameter(float delayTime)
    {
        if(!isOpened) Open();
        StartCoroutine(DelayedCloseCoroutine(delayTime));
    }

    public void DisableCollider()
    {
        boxCollider.enabled = false;
    }

    public void EnableCollider()
    {
        boxCollider.enabled = true;
    }
    public void MakeSmoke()
    {
        var smokeObject = Instantiate(smokePrefab, transform);
    }
}
