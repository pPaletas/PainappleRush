using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hurtbox : MonoBehaviour
{
    private Health _health;
    private EntityMovement _movement;
    private EntityInfo _entityInfo;

    public EntityInfo EntityInfo { get => _entityInfo; }

    public void Hurt(HitData hitdata)
    {
        _health.TakeDamage(hitdata.damage);

        // Ya explotó, ragdoll no funcionará
        if (_health.CurrentHealth <= 0f) return;

        if (hitdata.pushType == 0)
        {
            // _movement.Dash(info.Pivot.forward, 30f, 0.05f);
            _entityInfo.Ragdoll.Push(hitdata.force, 0.8f);
        }
        else
        {
            _entityInfo.Ragdoll.PushKO(hitdata.force, 2f);
        }
    }

    private void Awake()
    {
        _entityInfo = GetComponentInParent<EntityInfo>();
        _health = GetComponentInParent<Health>();
        _movement = GetComponentInParent<EntityMovement>();
    }
}