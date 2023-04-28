using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CameraPositionController : MonoBehaviour
{
    public static CameraPositionController Instance;

    [SerializeField] private float moveTime = 1f;

    public Transform playerTransform;
    [SerializeField] private CameraPositionPoint startPoint;
    public CameraPositionPoint activePoint;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Vector3 targetPoint = startPoint.transform.position;
        targetPoint.z = transform.position.z;
        transform.position = targetPoint;
        activePoint = startPoint;
    }

    CameraPositionPoint FindClosestNotActivePoint(List<CameraPositionPoint> points)
    {
        if (points == null) return null; 
        CameraPositionPoint closest = null;
        float sqrClosestDistance = Mathf.Infinity;
        foreach (var point in points)
        {
            if (point == activePoint) continue;
            float sqrDistance = (point.transform.position - playerTransform.position).sqrMagnitude;
            if (sqrDistance < sqrClosestDistance)
            {
                closest = point;
                sqrClosestDistance = sqrDistance;
            }
        }

        return closest;
    }
    
    public void SetNextPoint(List<CameraPositionPoint> points)
    {
        CameraPositionPoint nextPoint = FindClosestNotActivePoint(points);
        if (!nextPoint) nextPoint = activePoint;
        activePoint = nextPoint;
        Vector3 targetPoint = activePoint.transform.position;
        targetPoint.z = transform.position.z;
        transform.DOMove(targetPoint, moveTime);
    }
}
