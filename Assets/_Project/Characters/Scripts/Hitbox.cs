using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ForceDirection { Forward, Air }

public struct HitData
{
    public float damage;
    public int pushType;
    public Vector3 force;
    public EntityInfo info;
}

public class Hitbox : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
    [SerializeField] private int _pushType = 0;
    [SerializeField] private float _force = 100f;
    [SerializeField] private ForceDirection _forceDirection;
    private EntityInfo _playerInfo;

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
        }

        return Vector3.zero;
    }

    private void Awake()
    {
        _playerInfo = GetComponentInParent<EntityInfo>();
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_playerInfo.PlrNetwork.Photonview.IsMine)
        {
            HitData newData = new HitData
            {
                damage = _damage,
                pushType = _pushType,
                force = GetForceDirection() * _force,
                info = _playerInfo
            };

            if (other.TryGetComponent<Hurtbox>(out Hurtbox hurtbox) && hurtbox.EntityInfo != _playerInfo)
            {
                hurtbox.Hurt(newData);
            }
        }
    }
}