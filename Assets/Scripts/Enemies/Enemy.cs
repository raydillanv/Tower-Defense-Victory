using UnityEngine;

public class Enemy : MonoBehaviour
{
    //.net style coding conventions
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Path currentPath;
    private Vector3 _targetPosition;
    private int _currentWaypoint;


    private void Awake()
    {
        // awake runs before on enable
        currentPath = GameObject.Find("Path").GetComponent<Path>();
    }

    private void OnEnable()
    {
        // on enable runs before start
        _currentWaypoint = 0;
        _targetPosition = currentPath.GetPosition(0);
    }

    void Update()
    {
        
        // moving towards target position
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);

        // when the target position is reached we set a new target position
        float relativeDistance = (transform.position - _targetPosition).magnitude;
        if (relativeDistance < 0.1f)
        {
            // check to make sure its not the last index
            if (_currentWaypoint < currentPath.Waypoints.Length - 1)
            {
                _currentWaypoint++;
                _targetPosition = currentPath.GetPosition(_currentWaypoint);
            }
            else
            {
                gameObject.SetActive(false);
                print(gameObject.name + " reached the end of the map.");
            }

        }
    }

}
