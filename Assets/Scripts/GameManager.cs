using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<int> OnLivesChanged;

    private int _lives = 20;

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
    }

    private void Start()
    {
        OnLivesChanged?.Invoke(_lives);
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        // Mathf.Max returns the larger of 2 options, so itll never go below 0 when lives is otherwise negatives
        _lives = Mathf.Max(0, _lives - data.damage);
        OnLivesChanged?.Invoke(_lives);
    }
}
