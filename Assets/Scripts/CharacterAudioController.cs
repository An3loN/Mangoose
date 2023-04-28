using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(GroundChecker))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))]

public class CharacterAudioController : MonoBehaviour
{
    [SerializeField] private float minLandFallDistance;
    [SerializeField] private float walkVolume = 0.2f;
    [SerializeField] private float jumpVolume = 0.85f;

    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] AudioSource walkAudioSource;
    [SerializeField] AudioSource jumpAudioSource;

    GroundChecker groundChecker;
    Rigidbody2D rigidBody2D;
    
    // Start is called before the first frame update
    void Start()
    {
        groundChecker = GetComponent<GroundChecker>();

        groundChecker.onBecomeGrounded += OnBecomeGrounded;
        walkAudioSource.volume = walkVolume;
        jumpAudioSource.volume = jumpVolume;
    }
    void PlaySound(AudioClip sound)
    {
        jumpAudioSource.clip = sound;
        jumpAudioSource.Play();
    }
    void StartWalkSound(AudioClip sound)
    {
        walkAudioSource.clip = sound;
        walkAudioSource.loop = true;
        walkAudioSource.Play();
    }
    void StopWalkSound()
    {
        walkAudioSource.Stop();
        walkAudioSource.loop = false;
    }
    public void OnStartMoving()
    {
        StartWalkSound(walkSound);
    }
    public void OnEndMoving()
    {
        StopWalkSound();
    }
    public void OnJump()
    {
        PlaySound(jumpSound);
    }
    void OnBecomeGrounded()
    {
        if (groundChecker.GetFallDistance() > minLandFallDistance)
            PlaySound(landSound);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
