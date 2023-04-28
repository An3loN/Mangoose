using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public Action onBecomeGrounded;
    public Action onBecomeNotGrounded;
    [NonSerialized] public bool OnPlatform = false;
    [NonSerialized] public bool IsGrounded = false;
    [SerializeField] public Vector3 pivot;
    [SerializeField] public float length;
    
    [Header("Do not edit layers on play")]
    [SerializeField] private int groundLayer;
    [SerializeField] private int platformLayer;
    private int groundLayerMask;
    private int platformLayerMask;
    private Vector3 leftPivotPoint;
    private bool groundedLastFrame = false;
    public bool isFasingRight = true;
    public float maxHeight = -9999;
    bool isMeasuringHeight = false;
    
    public void FlipHorizontally()
    {
        pivot = new Vector3(-pivot.x, pivot.y, pivot.z);
        isFasingRight = !isFasingRight;
    }
    void Start()
    {
        groundLayerMask = 1 << groundLayer;
        platformLayerMask = 1 << platformLayer;

        onBecomeGrounded += OnBecomeGrounded;
        onBecomeNotGrounded += OnBecomeNotGrounded;
    }

    // Update is called once per frame
    void Update()
    {
        leftPivotPoint = transform.position + pivot + Vector3.left * (length / 2);
        IsGrounded = Physics2D.Raycast(leftPivotPoint, Vector2.right, length,
            groundLayerMask | platformLayerMask);
        if (IsGrounded && !groundedLastFrame)
        {
            onBecomeGrounded.Invoke();
        }
        else if (!IsGrounded && groundedLastFrame)
        {
            onBecomeNotGrounded.Invoke();
        }
        if(IsGrounded) OnPlatform = Physics2D.Raycast(leftPivotPoint, Vector2.right, length, 
            platformLayerMask);
        if(isMeasuringHeight && transform.position.y > maxHeight)
        {
            maxHeight = transform.position.y;
        }
        groundedLastFrame = IsGrounded;
    }

    void OnBecomeGrounded()
    {
        isMeasuringHeight = false;
    }
    void OnBecomeNotGrounded()
    {
        maxHeight = transform.position.y;
        isMeasuringHeight = true;
    }
    public float GetFallDistance()
    {
        return maxHeight - transform.position.y;
    }
    private void OnDrawGizmosSelected()
    {
        float halflength = length / 2f;
        Vector3 globalPivot = transform.position + pivot;
        Gizmos.DrawLine(globalPivot + Vector3.left * halflength, globalPivot + Vector3.right * halflength);
    }
}
