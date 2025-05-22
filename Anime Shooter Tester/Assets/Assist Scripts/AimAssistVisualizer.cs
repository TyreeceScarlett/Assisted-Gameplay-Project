using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeadTrackerVisualizer : MonoBehaviour
{
    [Header("References")]
    public Camera uiCamera;
    public GameObject markerPrefab;
    public RectTransform canvasTransform;

    [Header("Targets")]
    public List<Transform> headTargets = new();

    [Header("Settings")]
    [Tooltip("Max detection distance for the marker.")]
    public float detectionRange = 50f;

    [Tooltip("Distance considered 'far' but still detected.")]
    public float farDistanceThreshold = 30f;

    [Tooltip("Distance considered 'close' and triggers close color.")]
    public float closeDistanceThreshold = 10f;

    [Tooltip("Color when marker is default (out of detection range).")]
    public Color defaultColor = Color.white;

    [Tooltip("Color when target is far but within detection range.")]
    public Color farColor = Color.yellow;

    [Tooltip("Color when target is close.")]
    public Color closeColor = Color.red;

    private readonly List<GameObject> markers = new();

    void Start()
    {
        if (!uiCamera || !canvasTransform || !markerPrefab)
        {
            Debug.LogError("Missing required references.");
            enabled = false;
            return;
        }

        foreach (Transform head in headTargets)
        {
            GameObject marker = Instantiate(markerPrefab, canvasTransform);
            RectTransform rt = marker.GetComponent<RectTransform>();
            if (rt != null)
                rt.sizeDelta = new Vector2(20f, 20f);

            RawImage rawImg = marker.GetComponent<RawImage>();
            if (rawImg != null)
                rawImg.color = defaultColor;

            markers.Add(marker);
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

            Vector3 headPos = head.position + Vector3.up * 0.2f;
            Vector3 screenPos = uiCamera.WorldToScreenPoint(headPos);

            bool onScreen = screenPos.z > 0 &&
                            screenPos.x >= 0 && screenPos.x <= Screen.width &&
                            screenPos.y >= 0 && screenPos.y <= Screen.height;

            marker.SetActive(onScreen);
            if (!onScreen)
                continue;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform, screenPos, uiCamera, out Vector2 localPoint))
            {
                marker.GetComponent<RectTransform>().anchoredPosition = localPoint;
            }

            float distance = Vector3.Distance(uiCamera.transform.position, head.position);

            RawImage rawImg = marker.GetComponent<RawImage>();
            if (rawImg != null)
            {
                if (distance > detectionRange)
                {
                    rawImg.color = defaultColor;
                }
                else if (distance > farDistanceThreshold)
                {
                    rawImg.color = farColor;
                }
                else if (distance <= closeDistanceThreshold)
                {
                    rawImg.color = closeColor;
                }
                else
                {
                    // Optional: color when distance is between close and far thresholds
                    rawImg.color = farColor;
                }
            }
        }
    }
}
