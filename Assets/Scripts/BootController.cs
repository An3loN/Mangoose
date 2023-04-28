using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
[RequireComponent(typeof(AudioSource))]

public class BootController : MonoBehaviour
{
    [SerializeField] private float hitSpeed = 50f;
    [SerializeField] private Vector3 unscaledPivot;
    [Tooltip("Boundary angle between segments which defines whether heigher or lower angle of force should be used.")]
    [SerializeField] private float seperatingAngle = 45;
    [Tooltip("Angle in which direction force will be applied to hit object when falls from sides of the boot.")]
    [SerializeField] private float lowerAngle = 15;
    [Tooltip("Angle in which direction force will be applied to hit object when falls from the top of the boot.")]
    [SerializeField] private float higherAngle = 60;
    private Animator animator;
    private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = hitSound;
    }

    void OnHit()
    {
        animator.SetTrigger("Hit");
        audioSource.Play();
    }

    Vector2 GetScaledVectorFromAngleDegrees(float angle)
    {
        float angleRadian = angle / 180 * Mathf.PI;
        return new Vector2(Mathf.Cos(angleRadian) * - transform.localScale.x, Mathf.Sin(angleRadian) * transform.localScale.y);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Rigidbody2D rigidBody2D))
        {
            var localScale = transform.localScale;
            Vector3 scaledPivot = unscaledPivot;
            scaledPivot.x *= localScale.x;
            Vector3 globalPivot = transform.position + scaledPivot;
            Vector3 seperatingAngleVector = (Vector3)GetScaledVectorFromAngleDegrees(seperatingAngle) - scaledPivot;
            Vector3 seperatingAngleVectorNormolized = seperatingAngleVector.normalized;
            Vector3 hitDirection = (other.transform.position - globalPivot).normalized;
            
            Debug.DrawLine(globalPivot, globalPivot + seperatingAngleVectorNormolized, Color.blue, 10f);
            Debug.DrawLine(globalPivot, globalPivot + hitDirection, Color.red, 10f);

            Vector2 forceDirection;
            if (hitDirection.y > seperatingAngleVectorNormolized.y)
            {
                forceDirection = GetScaledVectorFromAngleDegrees(higherAngle);
            }
            else
            {
                forceDirection = GetScaledVectorFromAngleDegrees(lowerAngle);
            }
            Debug.DrawLine(rigidBody2D.position, rigidBody2D.position + forceDirection, Color.green, 10f);
            rigidBody2D.velocity = forceDirection * hitSpeed;
            OnHit();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + unscaledPivot, 0.05f);
    }
}
