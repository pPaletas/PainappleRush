using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Intermission, Wave, GameOver }

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    public event Action<GameState> gameStateChanged;

    [SerializeField] private int _intermissionTime = 5;
    [SerializeField] private int _waveTime = 60;

    // This will change everytime state changes
    private GameState _currentState = GameState.Intermission;
    private float _currentMaxTime = 0f;

    // WaveState
    private int _currentWave = 1;
    private float _currentStateTime = 0f;

    public int CurrentWave { get => _currentWave; }
    public GameState CurrentState { get => _currentState; }
    public int CurrentStateTime { get => (int)_currentStateTime; }
    public int WaveTime { get => _waveTime; }

    public void StartNextWave()
    {
        _currentWave++;

        _currentState = GameState.Intermission;
        _currentStateTime = 0f;
        _currentMaxTime = _intermissionTime;

        gameStateChanged?.Invoke(_currentState);
    }

    private void TickTimer()
    {
        _currentStateTime += Time.deltaTime;

        if (_currentStateTime >= _currentMaxTime)
        {
            _currentStateTime = 0f;
            ChangeState();
        }
    }

    private void ChangeState()
    {
        if (_currentState == GameState.Intermission)
        {
            _currentState = GameState.Wave;
            _currentMaxTime = _waveTime;
            SceneInfo.Instance.Plr.Health.Heal(25f);
        }
        else
        {
            _currentState = GameState.GameOver;
            _currentMaxTime = 1000000f;
        }

        gameStateChanged?.Invoke(_currentState);
    }

    private void Awake()
    {
        Instance = this;
        _currentMaxTime = _intermissionTime;
    }

    private void Update()
    {
        TickTimer();
    }
}