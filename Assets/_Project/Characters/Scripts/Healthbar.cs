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
    private Animator _heartAnim;

    private Vector3 _offset;
    private float _lastHealth;

    private int _animHurt = Animator.StringToHash("Hurt");

    private void Awake()
    {
        _healthBar = GetComponent<Slider>();
        _health = _isWorldSpace ? GetComponentInParent<Health>() : GameObject.Find("Player").GetComponentInChildren<Health>();
        _virtualCam = GameObject.Find("VirtualCam").transform;
        _heartAnim = GetComponentInChildren<Animator>();

        _parent = transform.parent.parent;
        _offset = transform.parent.localPosition;

        _lastHealth = _health.CurrentHealth;

        transform.parent.SetParent(null);
    }

    private void Update()
    {
        _healthBar.value = _health.CurrentHealth / _health.MaxHealth;

        if (_isWorldSpace && _virtualCam != null)
        {
            // Establece la posición siempre arriba, para evitar que rote cuando salga volando
            if (_parent != null) transform.parent.position = _parent.position + _offset;

            transform.LookAt(_virtualCam.position, _virtualCam.up);

            if (_health.CurrentHealth <= 0f)
            {
                transform.parent.gameObject.SetActive(false);
                Destroy(transform.parent.gameObject);
            }
        }
        // Si se hace daño, entonces hacer la animación del corazón
        else
        {
            if (_health.CurrentHealth < _lastHealth)
            {
                _lastHealth = _health.CurrentHealth;
                _heartAnim.SetTrigger(_animHurt);
            }
        }
    }
}