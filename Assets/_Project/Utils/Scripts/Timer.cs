using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public event Action onTimerEnded;

    [SerializeField] private float _waitTime;

    private float _currentTime;
    private bool _isStopped = true;

    public bool IsStopped { get => _isStopped; }

    public void StartTimer()
    {
        _currentTime = _waitTime;
        _isStopped = false;
    }

    public void SetWaitTime(float waitTime)
    {
        if (_isStopped)
            _waitTime = waitTime;
    }

    private void Update()
    {
        if (!_isStopped)
        {
            _currentTime -= Time.deltaTime;

            if (_currentTime <= 0f)
            {
                _isStopped = true;
                onTimerEnded?.Invoke();
            }
        }
    }
}