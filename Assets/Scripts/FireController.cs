using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class FireController : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip cookSound;
    public Action onEnvironmentChanged;

    private void Start()
    {
        audioSource.clip = cookSound;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Food"))
        {
            if(collision.TryGetComponent(out FoodController foodController))
            {
                foodController.CookLevel += 1;
                audioSource.Play();
            }
        }
        if(collision.CompareTag("Player"))
        {
            GameController.Instance.PerformPlayerDeath();
        }
    }
}
