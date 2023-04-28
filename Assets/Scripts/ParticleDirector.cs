using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleDirector : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> particleSystems;
    private Particle[] particles = new Particle[40];

    void SetParticlesRotation(ParticleSystem particleSystem)
    {
        particleSystem.GetParticles(particles);
        for(int index = 0; index < particles.Length; index++)
        {
            Vector3 direction = particles[index].velocity.normalized;
            float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            particles[index].rotation = rotation;
        }
        particleSystem.SetParticles(particles);
    }

    public void Play()
    {
        foreach(ParticleSystem particleSystem in particleSystems)
            StartCoroutine(LatePlay(particleSystem));
    }

    IEnumerator LatePlay(ParticleSystem particleSystem)
    {
        yield return new WaitForEndOfFrame();
        SetParticlesRotation(particleSystem);
    }
}
