using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ForceDirection { Forward, Air, Direction }

public struct HitData
{
    public float damage;
    public int pushType;
    public Vector3 force;
    public EntityInfo info;
}

public class Hitbox : MonoBehaviour
{
    [SerializeField] private string _hitPoolName;
    [SerializeField] private bool _isEnemy = false;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private int _pushType = 0;
    [SerializeField] private float _force = 100f;
    [SerializeField, Tooltip("0 Means is executed once"), Range(0f, 2f)]
    private float _updateTime = 0f;
    [SerializeField] private ForceDirection _forceDirection;

    [Header("Relative Force")]
    [SerializeField] private bool _applyRelativeForce = false;
    [SerializeField] private float _hitboxLength = 0f;
    [SerializeField, Range(0f, 1f)] private float _minForceMultiplier = 0.1f;

    private List<Hurtbox> _currentHurtboxes = new List<Hurtbox>();

    private EntityInfo _playerInfo;
    private ParticlesPool _hitPool;
    private float _timeSinceLastTriggerCheck = 1000f;

    private Vector3 GetForceDirection()
    {
        switch (_forceDirection)
        {
            case ForceDirection.Forward:
                return _playerInfo.Pivot.forward;
            case ForceDirection.Air:
                float randomRot = Random.Range(0f, 180f);
                Vector3 groundVector = Quaternion.AngleAxis(randomRot, Vector3.up) * _playerInfo.Pivot.forward;
                return (groundVector + Vector3.up * 3f).normalized;
            // Ya que la cagué aqui, vamos a hacer que si la dirección es cero, que entonces el Hurtbox calcule la dirección
            case ForceDirection.Direction:
                return Vector3.down;
        }

        return Vector3.zero;
    }

    private void SpawnHitParticle(Vector3 targetPos)
    {
        if (_hitPool == null) return;

        GameObject particle = _hitPool.Pool.Get();
        particle.transform.position = targetPos;

        particle.SetActive(true);
    }

    private void Awake()
    {
        _hitPool = GameObject.Find("ParticlePools/" + _hitPoolName).GetComponent<ParticlesPool>();
        _playerInfo = GetComponentInParent<EntityInfo>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_updateTime > 0f)
        {
            if (_timeSinceLastTriggerCheck >= _updateTime)
            {
                _timeSinceLastTriggerCheck = 0f;

                HitData newData = new HitData
                {
                    damage = _damage,
                    pushType = _pushType,
                    info = _playerInfo,
                    force = GetForceDirection() * _force
                };

                foreach (Hurtbox hurtbox in _currentHurtboxes)
                {
                    if (hurtbox.EntityInfo != _playerInfo)
                    {
                        hurtbox.Hurt(newData);
                    }
                }
            }
            else
            {
                _timeSinceLastTriggerCheck += Time.deltaTime;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        HitData newData = new HitData
        {
            damage = _damage,
            pushType = _pushType,
            info = _playerInfo
        };

        bool hasHurtbox = other.TryGetComponent<Hurtbox>(out Hurtbox hurtbox);

        if (!hasHurtbox) return;

        bool isEnemy = !_isEnemy ? hurtbox.EntityInfo != _playerInfo : hurtbox.EntityInfo.Agent == null;

        if (isEnemy)
        {

            if (_updateTime == 0)
            {
                Vector3 dir = GetForceDirection();
                float force = _force;

                // Aplica force based on distance
                if (_applyRelativeForce)
                {
                    Vector3 pivotPos = _playerInfo.Pivot.transform.position;

                    Vector3 dif = hurtbox.transform.position - pivotPos;
                    // En realidad es la diferencia entre la posicion del jugador, y la posicion proyectada
                    Vector3 projectedPos = Vector3.Project(dif, dir);

                    float projectedMagnitude = (projectedPos).magnitude;
                    float forceMultiplier = (_hitboxLength - projectedMagnitude) / _hitboxLength;
                    forceMultiplier = Mathf.Clamp(forceMultiplier, _minForceMultiplier, _force);

                    force *= forceMultiplier;
                }

                newData.force = dir * force;

                hurtbox.Hurt(newData);
                if (newData.damage > 0f && hurtbox.receiveDamage)
                {
                    SpawnHitParticle(hurtbox.transform.position);
                }
            }
            else
            {
                if (!_currentHurtboxes.Contains(hurtbox))
                {
                    _currentHurtboxes.Add(hurtbox);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_updateTime > 0f)
        {
            bool hasHurtbox = other.TryGetComponent<Hurtbox>(out Hurtbox hurtbox);

            if (hasHurtbox && _currentHurtboxes.Contains(hurtbox))
            {
                _currentHurtboxes.Remove(hurtbox);
            }
        }
    }

    private void OnDisable()
    {
        _currentHurtboxes.Clear();
    }
}