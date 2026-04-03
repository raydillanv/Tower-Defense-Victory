using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static event Action<int> OnWaveChanged;

    [SerializeField] private WaveData[] waves;
    private int _currentWaveIndex = 0;
    private int _waveCounter = 0;
    private WaveData CurrentWave => waves[_currentWaveIndex];
    
    private float _spawnTimer;
    private float _spawnCounter;
    private int _enemiesRemoved;

    [SerializeField] private ObjectPooler goblinPool;
    [SerializeField] private ObjectPooler impPool;
    [SerializeField] private ObjectPooler wolfPool;

    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;

    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            // links the enums from the static data to the non-data-persistent pools within the scene
            {EnemyType.Goblin, goblinPool },
            {EnemyType.Imp, impPool },
            {EnemyType.Wolf, wolfPool },
        };
    }

    private float _timeBetweenWaves = 2f;
    private float _waveCooldown;
    private bool _isBetweenWaves = false;

    private void OnEnable()
    {
        // subscribes to on enemy reached end message
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestoryed;
    }

    private void OnDisable()
    { // unsubscribe to prevent memory leaks
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed -= HandleEnemyDestoryed;
    }

    private void Start()
    {
        OnWaveChanged?.Invoke(_waveCounter);
    }

    void Update()
    {
        if (_isBetweenWaves)
        { 
            _waveCooldown -= Time.deltaTime;
            if ( _waveCooldown <= 0f)
            {
                // Modulo operator % (remainder operator)
                // Makes sure that if the index goes past the last wave, it wraps around to 0
                // returns whats left after divison

                _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length;
                _waveCounter++;
                OnWaveChanged?.Invoke(_waveCounter);
                _spawnCounter = 0;
                _enemiesRemoved = 0;
                _spawnTimer = 0f;
                _isBetweenWaves = false;
            }
        }
        else
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0 && _spawnCounter < CurrentWave.enemiesPerWave)
            {
                _spawnTimer = CurrentWave.spawnInterval;
                SpawnEnemy();
                _spawnCounter++;
            } // The observer pattern: a way to let one object 'broadcast' a message, other objects can 'listen' and react when the message is sent
            else if (_spawnCounter >= CurrentWave.enemiesPerWave && _enemiesRemoved >= CurrentWave.enemiesPerWave)
            {
                _isBetweenWaves = true;
                _waveCooldown = _timeBetweenWaves;
            }
        }

    }

    private void SpawnEnemy()
    {
        // matching enemy type key with pool in inspector
        if (_poolDictionary.TryGetValue(CurrentWave.enemyType, out var pool))
        {
            GameObject spawnedObject = pool.GetPooledObject();
            spawnedObject.transform.position = transform.position;
            spawnedObject.SetActive(true);
        }
        
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _enemiesRemoved++;
    }

    private void HandleEnemyDestoryed(Enemy enemy)
    {
        _enemiesRemoved++;
    }

}
