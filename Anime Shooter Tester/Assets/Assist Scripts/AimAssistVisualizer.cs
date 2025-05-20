using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeadTrackerVisualizer : MonoBehaviour
{
    [Tooltip("UI Camera rendering the canvas.")]
    public Camera uiCamera;

    [Tooltip("Prefab with RawImage component (light pink) and optional Text.")]
    public GameObject markerPrefab;

    [Tooltip("Canvas RectTransform to hold the UI markers.")]
    public RectTransform canvasTransform;

    [Tooltip("List of enemy head Transforms to track.")]
    public List<Transform> headTargets = new();

    private readonly List<GameObject> markers = new();

    void Start()
    {
        // Spawn one marker per head
        foreach (Transform head in headTargets)
        {
            GameObject marker = Instantiate(markerPrefab, canvasTransform);

            RectTransform rt = marker.GetComponent<RectTransform>();
            if (rt != null)
                rt.sizeDelta = new Vector2(20f, 20f);  // reasonable crosshair size

            RawImage rawImg = marker.GetComponent<RawImage>();
            if (rawImg != null)
                rawImg.color = new Color(1f, 0.8f, 0.9f, 1f); // light pink

            markers.Add(marker);

            Debug.Log("Created marker for: " + head.name);
        }
    }

    void Update()
    {
        for (int i = 0; i < headTargets.Count; i++)
        {
            Transform head = headTargets[i];
            GameObject marker = markers[i];

            if (head == null || marker == null)
                continue;

            Vector3 headPosition = head.position + new Vector3(0, 0.2f, 0); // offset for better alignment
            Vector3 screenPos = uiCamera.WorldToScreenPoint(headPosition);

            bool isVisible = screenPos.z > 0 &&
                             screenPos.x >= 0 && screenPos.x <= Screen.width &&
                             screenPos.y >= 0 && screenPos.y <= Screen.height;

            marker.SetActive(isVisible);

            if (!isVisible)
                continue;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, screenPos, uiCamera, out Vector2 uiPos))
            {
                marker.GetComponent<RectTransform>().anchoredPosition = uiPos;
            }
        }
    }
}
