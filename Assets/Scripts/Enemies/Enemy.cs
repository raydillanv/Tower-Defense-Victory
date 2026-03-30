using UnityEngine;

public class Enemy : MonoBehaviour
{
    //.net style coding conventions
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Path currentPath;
    private Vector3 _targetPosition;

    private void Awake()
    {
        // awake runs before on enable
        currentPath = GameObject.Find("Path").GetComponent<Path>();
    }

    private void OnEnable()
    {
        // on enable runs before start
        _targetPosition = currentPath.GetPosition(0);
    }

    void Update()
    {
        
        // moving towards target position
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
    }

}
