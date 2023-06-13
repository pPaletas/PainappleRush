using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Intermission, Wave, GameOver }

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;

    public event Action<GameState> gameStateChanged;

    [SerializeField] private List<GameObject> _instructions = new List<GameObject>();
    [SerializeField] private int _intermissionTime = 5;
    [SerializeField] private int _waveTime = 60;

    // Instructions
    private bool _givingInstructions = true;
    private int _currentInstruction = 0;

    // This will change everytime state changes
    private GameState _currentState = GameState.Intermission;
    private float _currentMaxTime = 0f;

    // WaveState
    private int _currentWave = 1;
    private float _currentStateTime = 0f;

    private bool gameOver = false;

    public int CurrentWave { get => _currentWave; }
    public GameState CurrentState { get => _currentState; }
    public int CurrentStateTime { get => (int)_currentStateTime; }
    public int WaveTime { get => _waveTime; }

    public void StartNextWave()
    {
        if (gameOver) return;

        _currentWave++;

        _currentState = GameState.Intermission;
        _currentStateTime = 0f;
        _currentMaxTime = _intermissionTime;

        gameStateChanged?.Invoke(_currentState);
    }

    private void NextInstruction()
    {
        bool touchedScreen = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        touchedScreen = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#else
        touchedScreen = Input.GetMouseButtonDown(0);
#endif

        if (touchedScreen)
        {
            if (_currentInstruction + 1 < _instructions.Count)
            {
                _instructions[_currentInstruction].SetActive(false);
                _currentInstruction++;
                _instructions[_currentInstruction].SetActive(true);
            }
            else
            {
                _instructions[_currentInstruction].transform.parent.gameObject.SetActive(false);
                _givingInstructions = false;
            }
        }
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
            gameOver = true;
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
        NextInstruction();

        if (!_givingInstructions && !gameOver)
        {
            TickTimer();
        }
    }
}