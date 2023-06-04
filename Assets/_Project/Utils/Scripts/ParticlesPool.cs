using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticlesPool : MonoBehaviour
{
    public GameObject particlePrefab;
    // Collection checks will throw errors if we try to release an item that is already in the pool.
    public bool collectionChecks = true;
    public int maxPoolSize = 50;

    IObjectPool<GameObject> m_Pool;

    public IObjectPool<GameObject> Pool
    {
        get
        {
            if (m_Pool == null)
            {
                m_Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 30, maxPoolSize);
            }
            return m_Pool;
        }
    }

    GameObject CreatePooledItem()
    {
        var ps = Instantiate(particlePrefab);
        ParticleSystem psParticles = ps.GetComponent<ParticleSystem>();
        psParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        //TODO: HACER QUE ESTA VUELTA FUNCIONE XD

        var main = psParticles.main;
        main.duration = 1;
        main.startLifetime = 1;
        main.loop = false;

        // This is used to return ParticleSystems to the pool when they have stopped.
        var returnToPool = ps.AddComponent<SwimingParticle>();
        returnToPool.pool = Pool;

        return ps;
    }

    // Called when an item is returned to the pool using Release
    void OnReturnedToPool(GameObject particle)
    {
        particle.SetActive(false);
    }

    // Called when an item is taken from the pool using Get
    void OnTakeFromPool(GameObject particle)
    {
        // particle.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(GameObject particle)
    {
        Destroy(particle);
    }
}