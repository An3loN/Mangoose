using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathController : MonoBehaviour
{
    public static DeathController PlayerDeathController;
    [SerializeField] GameObject particlesObject;
    [SerializeField] AudioClip deathSound;
    [SerializeField] float deathVolume = 0.55f;

    private void Start()
    {
        PlayerDeathController = this;

    }
    public void Kill()
    {
        GameObject instantiatedParticles = Instantiate(particlesObject);
        instantiatedParticles.transform.position = transform.position;
        if(instantiatedParticles.TryGetComponent(out ParticleDirector particleDirector)) particleDirector.Play();
        
        if (deathSound)
        {
            AudioSource audioSource = instantiatedParticles.AddComponent<AudioSource>();
            audioSource.volume = deathVolume;
            audioSource.clip = deathSound;
            audioSource.Play();
        }
        Destroy(gameObject);
    }
}
