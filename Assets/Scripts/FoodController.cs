using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class FoodController : MonoBehaviour
{
    [SerializeField] private List<Sprite> cookLevelSprites;
    [SerializeField] public int startCookLevel = 0;
    [SerializeField] private GameObject particleObject;

    [NonSerialized] public FoodSpawner foodSpawner;
    SpriteRenderer spriteRenderer;
    private int cookLevel;
    public int CookLevel
    {
        get => cookLevel;
        set
        {
            cookLevel = value;
            if (particleObject != null)
            {
                Instantiate(particleObject).transform.position = transform.position;
            }
            if (cookLevel >= cookLevelSprites.Count)
            {
                Die();
                return;
            }
            
            spriteRenderer.sprite = cookLevelSprites[value];
        }
    }
    public void ResetCookLevel()
    {
        cookLevel = startCookLevel;
    }
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        CookLevel = startCookLevel;
    }
    public void Die()
    {
        if (foodSpawner && foodSpawner.respawnFood) foodSpawner.SpawnFood(this);
        else Destroy(gameObject);
    }
}
