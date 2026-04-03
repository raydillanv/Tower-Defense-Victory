using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //.net style coding conventions

    // data driven design
    // data is assigned in the prefab and can help set things for each enemy type without ever hard coding things
    [SerializeField] private EnemyData data;

    public static event Action<EnemyData> OnEnemyReachedEnd;
    public static event Action<Enemy> OnEnemyDestroyed;

    //[SerializeField] private float moveSpeed = 3f;
    private Path _currentPath;
    private Vector3 _targetPosition;
    private int _currentWaypoint;
    private float _lives;

    [SerializeField] private Transform healthBar;
    private Vector3 _healthBarOriginalScale;


    private void Awake()
    {
        // awake runs before on enable
        _currentPath = GameObject.Find("Path").GetComponent<Path>();
        _healthBarOriginalScale= healthBar.localScale;
    }

    private void OnEnable()
    {
        UpdateHealthBar();
        // on enable runs before start
        _currentWaypoint = 0;
        _targetPosition = _currentPath.GetPosition(0);
        _lives = data.lives;
    }

    void Update()
    {
        
        // moving towards target position
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, data.speed * Time.deltaTime);

        // when the target position is reached we set a new target position
        float relativeDistance = (transform.position - _targetPosition).magnitude;
        if (relativeDistance < 0.1f)
        {
            // check to make sure its not the last index
            if (_currentWaypoint < _currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPosition = _currentPath.GetPosition(_currentWaypoint);
            }
            else // reached last waypoint
            {
                // null checking + event invocation
                // also invokes the data which contains how many lives a player loses when enemy reaches the end
                OnEnemyReachedEnd?.Invoke(data);
                gameObject.SetActive(false);
                print(gameObject.name + " reached the end of the map.");
            }

        }
    }

    public void TakeDamage(float damage)
    {

        _lives -= damage;
        _lives = Math.Max(_lives, 0);
        UpdateHealthBar();
        if (_lives <= 0)
        {
            OnEnemyDestroyed?.Invoke(this);
            gameObject.SetActive(false);
        }
    }

    private void UpdateHealthBar()
    {
        float healthPercent = _lives / data.lives;
        Vector3 scale = _healthBarOriginalScale;
        scale.x = _healthBarOriginalScale.x * healthPercent;
        healthBar.localScale = scale;
    }
}
