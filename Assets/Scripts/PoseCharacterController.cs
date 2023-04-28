using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(GroundChecker))]
public class PoseCharacterController : MonoBehaviour
{
    public static PoseCharacterController Instance;

    private BoxCollider2D boxCollider2D;
    private GroundChecker groundChecker;
    [NonSerialized] public Pose pose = Pose.DEFAULT;
    public Transform itemPivotTransform;
    [SerializeField] private PoseData defaultPoseData;
    [SerializeField] private PoseData duckPoseData;
    [SerializeField] private PoseData lyingPoseData;

    private void Start()
    {
        Instance = this;
        boxCollider2D = GetComponent<BoxCollider2D>();
        groundChecker = GetComponent<GroundChecker>();
    }
    public void ApplyPoseData(PoseData poseDataToApply)
    {
        boxCollider2D.size = poseDataToApply.colliderSize;
        boxCollider2D.size -= Vector2.one * boxCollider2D.edgeRadius * 2;
        boxCollider2D.offset = poseDataToApply.colliderPivot;
        itemPivotTransform.localPosition = poseDataToApply.itemPivot;
        groundChecker.pivot = poseDataToApply.groundCheckerPivot;
        groundChecker.length = poseDataToApply.groundCheckerLength;
        if (!groundChecker.isFasingRight)
        {
            groundChecker.FlipHorizontally();
            groundChecker.isFasingRight = false;
        }
    }
    public PoseData GetCurrentPoseData()
    {
        switch (pose)
        {
            case Pose.DEFAULT:
                return defaultPoseData;
            case Pose.DUCK:
                return duckPoseData;
            case Pose.LYING:
                return lyingPoseData;
            default:
                return defaultPoseData;
        }
    }
    void DrawPoseData(PoseData poseDataToDraw)
    {
        Gizmos.DrawCube((Vector2)transform.position + poseDataToDraw.colliderPivot, poseDataToDraw.colliderSize);
        Gizmos.DrawWireSphere(transform.position + poseDataToDraw.itemPivot, 0.05f);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        DrawPoseData(defaultPoseData);
        Gizmos.color = new Color(0, 0, 1, 0.2f);
        DrawPoseData(duckPoseData);
        Gizmos.color = new Color(1, 0.92f, 0.16f, 0.2f);
        DrawPoseData(lyingPoseData);
    }
}
public enum Pose
{
    DEFAULT,
    DUCK,
    LYING
}
[Serializable]
public struct PoseData
{
    [SerializeField] public Vector2 colliderPivot;
    [SerializeField] public Vector2 colliderSize;
    [SerializeField] public Vector3 itemPivot;
    [SerializeField] public Vector3 groundCheckerPivot;
    [SerializeField] public float groundCheckerLength;
}