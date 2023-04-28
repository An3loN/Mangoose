using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
public class SpeedLimitter : MonoBehaviour
{
    public float speedLimit = 50f;
    private Rigidbody2D rigidBody2D;

    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rigidBody2D.velocity = Vector2.ClampMagnitude(rigidBody2D.velocity, speedLimit);
    }
}
