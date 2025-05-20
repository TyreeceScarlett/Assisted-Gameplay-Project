using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AimAssistVisualizer : MonoBehaviour
{
    public BetterAimAssist aimAssist;
    public Camera uiCamera;
    public GameObject markerPrefab;
    public RectTransform canvasTransform;

    private List<GameObject> markers = new();

    void Update()
    {
        ClearMarkers();

        if (aimAssist == null || uiCamera == null || markerPrefab == null || canvasTransform == null)
            return;

        DrawMarker(aimAssist.stickyTarget, Color.yellow, "Sticky");
        DrawMarker(aimAssist.trackingTarget, Color.blue, "Track");
        DrawMarker(aimAssist.adsTarget, Color.red, "ADS");
        DrawMarker(aimAssist.bulletMagnetTarget, Color.magenta, "Magnet");
    }

    void DrawMarker(Transform target, Color color, string label)
    {
        if (target == null) return;

        Vector3 screenPos = uiCamera.WorldToScreenPoint(target.position);
        if (screenPos.z < 0) return;

        GameObject marker = Instantiate(markerPrefab, canvasTransform);
        marker.transform.position = screenPos;

        Image img = marker.GetComponent<Image>();
        if (img != null) img.color = color;

        Text txt = marker.GetComponentInChildren<Text>();
        if (txt != null) txt.text = label;

        markers.Add(marker);
    }

    void ClearMarkers()
    {
        foreach (var m in markers)
            Destroy(m);
        markers.Clear();
    }
}
