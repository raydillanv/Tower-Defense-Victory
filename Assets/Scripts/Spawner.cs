using UnityEngine;

public class Spawner : MonoBehaviour
{
    private float _spawnTimer;
    private float _spawnInterval = 1.5f;

    public GameObject Prefab;

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
        GameObject spawnedObject = GameObject.Instantiate(Prefab);
        spawnedObject.transform.position = transform.position;
    }
}
