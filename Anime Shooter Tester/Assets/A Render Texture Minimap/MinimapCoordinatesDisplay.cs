using UnityEngine;
using TMPro;

public class MinimapCoordinatesDisplay : MonoBehaviour
{
    public Transform player;
    public TMP_Text coordinatesText;

    public GameObject minimap; // Assign your minimap GameObject here

    private float minCoordinate = -500f;
    private float maxCoordinate = 500f;

    // Track minimap visibility
    private bool minimapVisible = true;

    void Start()
    {
        if (minimap != null)
            minimapVisible = minimap.activeSelf;
    }

    void Update()
    {
        if (player == null || coordinatesText == null) return;

        Vector3 pos = player.position;

        float x = Mathf.Clamp(pos.x, minCoordinate, maxCoordinate);
        float y = Mathf.Clamp(pos.y, minCoordinate, maxCoordinate);
        float z = Mathf.Clamp(pos.z, minCoordinate, maxCoordinate);

        coordinatesText.text = $"X: {x:0} Y: {y:0} Z: {z:0}";

        // Toggle minimap visibility with F7 key
        if (Input.GetKeyDown(KeyCode.F7) && minimap != null)
        {
            minimapVisible = !minimapVisible;
            minimap.SetActive(minimapVisible);
            Debug.Log($"Minimap toggled. Now visible? {minimapVisible}");
        }
    }
}
