using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    const float CHECK_RADIUS = 0.1f;
    int groundLayer;
    [SerializeField] int startCookLevel = 0;
    [SerializeField] GameObject foodPrefab;
    [SerializeField] float spawnForce = 20f;
    [SerializeField] bool spawnFoodOnStart = false;
    [SerializeField] public bool respawnFood = true;
    [SerializeField] FoodController spawnedFood;

    public void SpawnFood()
    {
        Vector3 spawnDirection = transform.rotation * Vector3.down;
        if(CheckObstacle(transform.position + spawnDirection)) return;
        GameObject instantiatedFood = Instantiate(foodPrefab, transform.position + spawnDirection, Quaternion.identity);
        spawnedFood = instantiatedFood.GetComponent<FoodController>();
        spawnedFood.foodSpawner = this;
        spawnedFood.startCookLevel = startCookLevel;
        if (instantiatedFood.TryGetComponent(out Rigidbody2D rigidbody)) rigidbody.AddForce(spawnDirection * spawnForce);
    }
    public void SpawnFood(FoodController foodController)
    {
        if (foodController.foodSpawner != this) throw new System.Exception("Wrong food spawner attached");
        if (PlayerGrabController.Instance.CompareEquipped(gameObject)) PlayerGrabController.Instance.DropItem();
        Vector3 spawnDirection = transform.rotation * Vector3.down;
        foodController.transform.position = transform.position + spawnDirection;
        foodController.ResetCookLevel();

        if (foodController.TryGetComponent(out Rigidbody2D rigidbody))
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.AddForce(spawnDirection * spawnForce);
        }
    }
    void Start()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
        if (spawnFoodOnStart) SpawnFood();   
    }
    bool CheckObstacle(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, CHECK_RADIUS);
        foreach(Collider2D collider in colliders)
        {
            if (collider.gameObject.layer == groundLayer) return true;
        }
        return false;
    }
    public void OnTriggered()
    {
        if (spawnedFood) spawnedFood.Die();
        else SpawnFood();
    }
}
