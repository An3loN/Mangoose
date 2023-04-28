using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class SlimeSoundController : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip bounceSound;
    float minVelocity = 1f;
    float maxVelocity = 10f;
    float minVolume = 0;
    float maxVolume = 0.5f;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    float Map(float x, float minIn, float maxIn, float minOut, float maxOut)
    {
        if (x < minIn) return minOut;
        if (x > maxIn) return maxOut;
        return ((x - minIn)/(maxIn - minIn)) * (maxOut - minOut) + minOut;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Rigidbody2D rigidBody2d))
        {
            float playerVelocity = rigidBody2d.velocity.magnitude;
            if(playerVelocity > minVelocity)
            {
                audioSource.clip = bounceSound;
                audioSource.volume = Map(playerVelocity, minVelocity, maxVelocity, minVolume, maxVolume);
                audioSource.Play();
            }
        }
    }
}
