using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out FoodController food))
        {
            food.Die();
        }
        if(collision.CompareTag("Player"))
        {
            GameController.Instance.PerformPlayerDeath();
        }
    }
}
