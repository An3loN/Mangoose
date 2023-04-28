using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activatable : MonoBehaviour
{
    public Action onGetAbleActivate;
    public Action onGetUnableActivate;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onGetAbleActivate.Invoke();
        }
    }
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onGetUnableActivate.Invoke();
        }
    }
}
