using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CameraTransitionController : MonoBehaviour
{
    public static CameraTransitionController Instance;
    
    [SerializeField] private Image blackImage;
    
    [NonSerialized] public Camera mainCamera;
    [NonSerialized] public bool isBlackScreen;
    private float defaultSize;
    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        defaultSize = mainCamera.orthographicSize;
    }

    void Start()
    {
        Instance = this;
        HideBlackScreen(3);
    }

    /*void SetBlackScreenAlpha(float alpha, float fadeTime)
    {
        blackImage.color = new Color(0, 0, 0, -(alpha - 1));
        blackImage.DOColor(new Color(0, 0, 0, alpha), fadeTime);
    }*/
    void SetBlackScreenAlpha(float alpha, float fadeTime, Action action)
    {
        blackImage.color = new Color(0, 0, 0, -(alpha - 1));
        blackImage.DOColor(new Color(0, 0, 0, alpha), fadeTime).onComplete = action.Invoke;
    }
    Func<bool> SetBlackScreenAlpha(float alpha, float fadeTime)
    {
        bool done = false;
        Func<bool> waiter = () => done;
        blackImage.color = new Color(0, 0, 0, -(alpha - 1));
        blackImage.DOColor(new Color(0, 0, 0, alpha), fadeTime).OnComplete(() => done = true);
        return waiter;
    }

    /*public void ShowBlackScreen(float fadeTime = 0.7f)
    {
        SetBlackScreenAlpha(1, fadeTime);
    }

    public void HideBlackScreen(float fadeTime = 0.7f)
    {
        SetBlackScreenAlpha(0, fadeTime);
    }*/
    public Func<bool> ShowBlackScreen(float fadeTime = 0.7f)
    {
        return SetBlackScreenAlpha(1, fadeTime);
    }

    public Func<bool> HideBlackScreen(float fadeTime = 0.7f)
    {
        return SetBlackScreenAlpha(0, fadeTime);
    }
    public void ShowBlackScreenAndDo(Action action, float fadeTime = 0.7f)
    {
        SetBlackScreenAlpha(1, fadeTime, action);
    }
    public Func<bool> SetPosition(float x, float y, float time)
    {
        bool completed = false;
        transform.DOMove(new Vector3(x, y, transform.position.z), time).OnComplete(() => completed = true);
        Func<bool> onComplete = () => completed;
        return onComplete;
    }
    public Func<bool> SetZoom(float zoom, float time)
    {
        bool completed = false;
        mainCamera.DOOrthoSize(defaultSize / zoom, time).OnComplete(() => completed = true);
        Func<bool> onComplete = () => completed;
        return onComplete;
    }
}