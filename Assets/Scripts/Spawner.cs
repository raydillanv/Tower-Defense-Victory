using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float _spawnTimer;
    private float _spawnInterval = 1.5f;

    [SerializeField] private ObjectPooler pool;

    void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0)
        {
            _spawnTimer = _spawnInterval;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        GameObject spawnedObject = pool.GetPooledObject();
        spawnedObject.transform.position = transform.position;
        spawnedObject.SetActive(true);
    }
}
