using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Camera mainCamera;

    private void Start()
    {
        // Cache reference to main camera
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Get the direction to the camera
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            directionToCamera.y = 0f; // Lock the rotation to the Y-axis

            // Rotate the health bar to face the camera horizontally
            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }
}