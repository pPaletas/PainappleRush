using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hurtbox : MonoBehaviour
{
    public event Action hurted;

    public string bloodPoolName;
    [HideInInspector] public bool isReceivingDamage = false;
    [HideInInspector] public bool receiveDamage = true;

    private ParticlesPool _bloodPool;
    private Health _health;
    private EntityMovement _movement;
    private EntityInfo _entityInfo;
    private GameObject _vfxBlood;

    public EntityInfo EntityInfo { get => _entityInfo; }

    public void Hurt(HitData hitdata)
    {
        // Si el hurtbox no está habilitado, o es jugador y esta dasheando
        if (!receiveDamage || (_entityInfo.Agent == null && _entityInfo.Movement.Dashing)) return;

        hurted?.Invoke();

        _health.TakeDamage(hitdata.damage);


        isReceivingDamage = true;

        // Ya explotó, ragdoll no funcionará
        if (_health.CurrentHealth <= 0f) return;

        Vector3 force = hitdata.force;

        if (force.normalized == Vector3.down)
            force = (transform.position - hitdata.info.Pivot.transform.position).normalized * hitdata.force.magnitude;

        if (hitdata.damage > 0f)
        {
            SpawnBloodParticle(force);
        }

        if (hitdata.pushType == 0)
        {
            // _movement.Dash(info.Pivot.forward, 30f, 0.05f);
            _entityInfo.Ragdoll.Push(force, 0.8f);
        }
        else
        {
            _entityInfo.Ragdoll.PushKO(force, 2f);
        }
    }

    private void SpawnBloodParticle(Vector3 force)
    {
        if (_bloodPool == null) return;

        GameObject particle = _bloodPool.Pool.Get();
        particle.transform.position = transform.position + force.normalized;

        Quaternion vectorToRot = Quaternion.LookRotation(force.normalized, Vector3.up);
        particle.transform.rotation = vectorToRot;

        particle.SetActive(true);
    }

    private void Awake()
    {
        _entityInfo = GetComponentInParent<EntityInfo>();
        _health = GetComponentInParent<Health>();
        _movement = GetComponentInParent<EntityMovement>();

        _bloodPool = GameObject.Find("ParticlePools/" + bloodPoolName).GetComponent<ParticlesPool>();
    }
}