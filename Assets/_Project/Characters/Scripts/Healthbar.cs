using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private bool _isWorldSpace = true;
    private Slider _healthBar;
    private Health _health;
    private Transform _virtualCam;
    private Transform _parent;
    private Vector3 _offset;

    private void Start()
    {
        _healthBar = GetComponent<Slider>();
        _health = GetComponentInParent<EntityInfo>().GetComponentInChildren<Health>();
        _virtualCam = GameObject.Find("VirtualCam").transform;
        _parent = transform.parent.parent;
        _offset = transform.parent.localPosition;

        transform.parent.SetParent(null);
    }

    private void Update()
    {
        _healthBar.value = _health.CurrentHealth / _health.MaxHealth;

        if (_isWorldSpace)
        {
            // Establece la posición siempre arriba, para evitar que rote cuando salga volando
            transform.parent.position = _parent.position + _offset;

            transform.LookAt(_virtualCam.position, _virtualCam.up);
        }
    }
}