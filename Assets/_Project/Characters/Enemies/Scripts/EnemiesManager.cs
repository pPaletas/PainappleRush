using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
struct WaveInfo
{
    public int policeAmount;
    public bool spawnTurtles;
}

public class EnemiesManager : MonoBehaviour
{
    public static EnemiesManager Instance;
    public event Action allEnemiesDefeated;

    [SerializeField] private List<WaveInfo> _wavesInfo = new List<WaveInfo>();
    [SerializeField] private GameObject _policePrefab;
    [SerializeField] private GameObject _turtlePrefab;
    [SerializeField] private Transform _spawnsContainer;
    [SerializeField] private Transform _enemiesContainer;

    private Transform _selectedSpawnPoint;

    private int _currentPoliceCount = 4;
    private bool _spawnTurtles = false;

    public int GetAmmountOfEnemies() => _enemiesContainer.childCount;

    public void NotifyEnemyDeath()
    {
        if (_enemiesContainer.childCount <= 0)
        {
            GameplayManager.Instance.StartNextWave();
            allEnemiesDefeated?.Invoke();
        }
    }

    private void OnGameStateChange(GameState state)
    {
        if (state == GameState.Wave)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < _currentPoliceCount; i++)
        {
            SpawnEntity(_policePrefab);

            // Si es par, esto para ver si spawneamos torguguetes o no
            if (i % 2 == 0 && _spawnTurtles)
            {
                SpawnEntity(_turtlePrefab);
            }
        }

        // Verificamos si hay mas WaveInfo en la lista y aplicamos la info para la siguiente ola
        int currentWave = GameplayManager.Instance.CurrentWave;
        if (currentWave < _wavesInfo.Count)
        {
            _currentPoliceCount = _wavesInfo[currentWave].policeAmount;
            _spawnTurtles = _wavesInfo[currentWave].spawnTurtles;
        }
        // Si no lo está, entonces solo le sumamos, y le decimos que si spawnee tortugas
        else
        {
            _currentPoliceCount += 1;
            _spawnTurtles = true;
        }
    }

    private void SpawnEntity(GameObject prefab)
    {
        _selectedSpawnPoint = _spawnsContainer.GetChild(UnityEngine.Random.Range(0, _spawnsContainer.childCount));

        GameObject newPolice = Instantiate(prefab, _selectedSpawnPoint.position, _selectedSpawnPoint.rotation);
        newPolice.transform.SetParent(_enemiesContainer);
        newPolice.GetComponentInChildren<NavMeshAgent>().Warp(newPolice.transform.position);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameplayManager.Instance.gameStateChanged += OnGameStateChange;

        if (_wavesInfo.Count > 0)
        {
            _currentPoliceCount = _wavesInfo[0].policeAmount;
            _spawnTurtles = _wavesInfo[0].spawnTurtles;
        }
    }
}