using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class DragonController : MonoBehaviour
{
    private const float MAX_FIRE_DISTANCE = 50f;
    private readonly Vector2[] WATCH_DIRECTION = {
        new Vector2(-1, 0),
        new Vector2(-1, 1),
        new Vector2(0, 1),
        new Vector2(1, 1),
        new Vector2(1, 0)
    };

    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform firePointTransform;
    [SerializeField] private GameObject firePrefab;
    [SerializeField] private int watchPreset = 0;
    [SerializeField] private List<int> obstacleLayersList;
    [SerializeField] private AudioClip eatSound;

    AudioSource audioSource;
    int obstacleLayerMask = 0;
    private GameObject instantiatedFireObject;
    private int lastWatchPreset = 0;
    private float lastHitLength = 0;
    void OnValidate()
    {
        if (watchPreset != lastWatchPreset)
        {
            lastWatchPreset = watchPreset;
            ApplyWatchPreset();
        }
        InitializeLayerMask();
    }
    void InitializeLayerMask()
    {
        obstacleLayerMask = 0;
        foreach (var layer in obstacleLayersList)
        {
            obstacleLayerMask += 1 << layer;
        }
    }
    private void Update()
    {
        if (!instantiatedFireObject) return;
        RaycastHit2D hit = GetHit();
        if(lastHitLength != hit.distance)
        {
            lastHitLength = hit.distance;
            StartFire();
        }
    }
    Vector2 GetRotatedWatchDirection()
    {
        return transform.rotation * WATCH_DIRECTION[watchPreset % WATCH_DIRECTION.Length];
    }
    void ApplyWatchPreset()
    {
        Vector2 watchDirection = WATCH_DIRECTION[watchPreset % WATCH_DIRECTION.Length];
        int xScaleSign = watchDirection.x >= 0 ? 1 : -1;
        transform.localScale = Vector3.Scale(PositiveScale(transform.localScale), new Vector3(xScaleSign, 1, 1));
        headTransform.localRotation = GetRotationFromVector(new Vector2(Mathf.Abs(watchDirection.x), watchDirection.y));
    }
    Quaternion GetRotationFromVector(Vector2 vector)
    {
        return Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, vector) );
    }
    Vector3 PositiveScale(Vector3 vector)
    {
        Vector2 result = new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));
        return result;
    }
    public void SetWatchPreset(int preset)
    {
        watchPreset = preset;
        ApplyWatchPreset();
    }
    
    void StartFire()
    {
        if (instantiatedFireObject) Destroy(instantiatedFireObject);
        Vector2 watchDirection = GetRotatedWatchDirection();
        instantiatedFireObject = Instantiate(firePrefab, firePointTransform);
        Transform fireTransform = instantiatedFireObject.transform;
        RaycastHit2D hit = GetHit();
        Vector3 fireDestination = hit ? hit.point : firePointTransform.position + ((Vector3)watchDirection.normalized * MAX_FIRE_DISTANCE);
        fireTransform.position = Vector2.Lerp(firePointTransform.position, fireDestination, 0.5f);
        float fireLength = Vector2.Distance(firePointTransform.position, fireDestination);
        fireTransform.localScale = new Vector2(fireLength, fireTransform.localScale.y);
    }
    private RaycastHit2D GetHit() =>
        Physics2D.Raycast(firePointTransform.position, GetRotatedWatchDirection(), MAX_FIRE_DISTANCE, obstacleLayerMask);        

    private void EatFood(FoodController food)
    {
        if (instantiatedFireObject) Destroy(instantiatedFireObject);
        watchPreset = food.CookLevel;
        
        audioSource.Play();
        StartCoroutine(DelayedFireStart(1f));
        food.Die();
    }
    IEnumerator DelayedFireStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        ApplyWatchPreset();
        StartFire();
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = eatSound;

        InitializeLayerMask();
        ApplyWatchPreset();
        StartFire();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) GameController.Instance.PerformPlayerDeath();
        else if (collision.CompareTag("Food")) {
            if (collision.TryGetComponent(out FoodController food)) EatFood(food);
        }
    }
}
