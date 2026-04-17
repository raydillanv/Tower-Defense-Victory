using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Platform : MonoBehaviour
{
    public static event Action<Platform> OnPlatformClicked;
    [SerializeField] private LayerMask platformLayerMask;

    public static bool towerPanelOpen { get; set; } = false;

    private void Update()
    {
        if (towerPanelOpen || Time.timeScale == 0f)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D raycastHit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, platformLayerMask);

            if (raycastHit.collider != null)
            {
                Platform platform = raycastHit.collider.GetComponent<Platform>();
                if (platform != null)
                {
                    // when the platform is clicked we pass an event that shows which one was clicked
                    OnPlatformClicked?.Invoke(platform);
                }
            }
        }
    }

    public void PlaceTower(TowerData data)
    { // set platform as parent of tower
        Instantiate(data.prefab, transform.position, Quaternion.identity, transform);
    }

}
