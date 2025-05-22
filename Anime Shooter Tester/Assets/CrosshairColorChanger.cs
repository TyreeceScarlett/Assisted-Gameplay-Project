using UnityEngine;
using UnityEngine.UI;

public class CrosshairColorChanger : MonoBehaviour
{
    public Image crosshairImage;      // Crosshair UI Image
    public Camera playerCamera;       // Your player/camera
    public float maxDistance = 30f;   // Max detection distance

    public string enemyTag = "Enemy"; // Tag to identify enemy head or enemy

    // Colors for crosshair
    private Color farColor = Color.white;
    private Color closeColor = new Color(0.5f, 0f, 0f); // Dark red

    void Update()
    {
        if (crosshairImage == null || playerCamera == null)
            return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // center of screen

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            if (hit.collider.CompareTag(enemyTag))
            {
                // Enemy hit, get distance and lerp color
                float distance = hit.distance;
                float t = Mathf.Clamp01(distance / maxDistance);
                crosshairImage.color = Color.Lerp(closeColor, farColor, t);
            }
            else
            {
                // Hit something else (wall or other object)
                crosshairImage.color = farColor;
            }
        }
        else
        {
            // Nothing hit
            crosshairImage.color = farColor;
        }
    }
}
