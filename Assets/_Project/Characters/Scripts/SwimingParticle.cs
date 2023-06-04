using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SwimingParticle : MonoBehaviour
{
    [HideInInspector] public IObjectPool<GameObject> pool;
    private ParticleSystem system;

    void Start()
    {
        system = GetComponent<ParticleSystem>();
        var main = system.main;
    }

    void OnParticleSystemStopped()
    {
        // Return to the pool
        pool.Release(gameObject);
    }
}