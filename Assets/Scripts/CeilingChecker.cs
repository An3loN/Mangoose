using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingChecker : MonoBehaviour
{
    const int GROUND_LAYER = 6;

    [SerializeField] Vector2 pivot;
    [SerializeField] float length;
    private int layerMask;
    public Action onCeilingHit;

    // Start is called before the first frame update
    void Awake()
    {
        layerMask = 1 << GROUND_LAYER;
    }

    // Update is called once per frame
    void Update()
    {
        GetBoundPositions(out Vector2 left, out Vector2 right);
        var hit = Physics2D.Linecast(left, right, layerMask);
        if (hit) onCeilingHit.Invoke();
    }
    void GetBoundPositions(out Vector2 left, out Vector2 right)
    {
        Vector2 scaledPivot = new Vector2((transform.localScale.x > 0 ? 1 : -1) * pivot.x, pivot.y) + (Vector2) transform.position;
        left = new Vector2(scaledPivot.x - length/2f, scaledPivot.y);
        right = new Vector2(scaledPivot.x + length / 2f, scaledPivot.y);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere((Vector2)transform.position + pivot, 0.05f);
        GetBoundPositions(out Vector2 left, out Vector2 right);
        Gizmos.DrawLine(left,right);
    }
}
