using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public Action onEntityDied;
    [SerializeField] private float _maxHealth = 100f;

    private float _currentHealth;

    public float MaxHealth { get => _maxHealth; }
    public float CurrentHealth { get => _currentHealth; }

    public void TakeDamage(float damage)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0f, _maxHealth);

        if (_currentHealth <= 0f)
        {
            onEntityDied?.Invoke();
        }
    }

    public void Heal(float health)
    {
        _currentHealth = Math.Clamp(_currentHealth + health, 0f, _maxHealth);
    }

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }
}