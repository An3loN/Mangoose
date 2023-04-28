using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionPoint : MonoBehaviour
{
    [SerializeField] private List<CameraPositionPoint> relatedPoints = new List<CameraPositionPoint>();
    private void OnDrawGizmosSelected()
    {
        if (Camera.main)
        {
            float height = Camera.main.orthographicSize * 2f;
            float width = height * Camera.main.aspect;
            
            Gizmos.color = new Color(1,0,1,0.2f);
            Gizmos.DrawCube(transform.position, new Vector3(width, height));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && GameController.playerAlive && CameraPositionController.Instance.activePoint == this)
        {
            CameraPositionController.Instance.SetNextPoint(relatedPoints);
        }
    }
}
