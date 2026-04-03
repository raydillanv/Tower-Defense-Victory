using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<int> OnLivesChanged;
    public static event Action<int> OnResourcesChanged;

    private int _lives = 20;
    private int _resources = 0;

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void Start()
    {
        OnLivesChanged?.Invoke(_lives);
        OnResourcesChanged?.Invoke(_resources);
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        // Mathf.Max returns the larger of 2 options, so itll never go below 0 when lives is otherwise negatives
        _lives = Mathf.Max(0, _lives - data.damage);
        OnLivesChanged?.Invoke(_lives);
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        AddResources(Mathf.RoundToInt(enemy.Data.resourceReward));
    }

    private void AddResources(int amount)
    {
        _resources += amount;
        OnResourcesChanged?.Invoke(_resources);
    }
}
