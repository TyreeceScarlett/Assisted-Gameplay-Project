using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmartAimAssist : MonoBehaviour
{
    [Header("Assist Settings")]
    public bool aimAssistEnabled = true;
    public float detectionRadius = 3f; // Radius in world space for nearby target detection
    public float aimPullStrength = 3f; // How strong the aim pull is
    public float detectionRange = 20f;
    public LayerMask targetLayer;

    [Header("Crosshair UI")]
    public Image crosshairImage;
    public Color normalColor = Color.white;
    public Color lockedColor = Color.red;
    public float colorLerpSpeed = 10f;

    [Header("References")]
    public Transform cameraFollowPos; // The object Cinemachine follows (e.g., player head or pivot)
    private Camera cam;
    private Transform detectedTarget;
    private RectTransform crosshairRect;
    private Vector3 originalCrosshairPosition;

    void Start()
    {
        cam = Camera.main;

        if (cam == null)
            Debug.LogError("No main camera found!");

        if (crosshairImage != null)
        {
            crosshairImage.color = normalColor;
            crosshairRect = crosshairImage.GetComponent<RectTransform>();
            originalCrosshairPosition = crosshairRect.position;
        }
    }

    void Update()
    {
        if (!aimAssistEnabled || cam == null)
            return;

        // 1. Find target near center of screen
        detectedTarget = FindClosestTargetToScreenCenter();

        // 2. Gently pull camera aim toward target
        if (detectedTarget != null)
        {
            Vector3 targetPoint = GetTargetCenter(detectedTarget);
            Vector3 aimDirection = (targetPoint - cameraFollowPos.position).normalized;
            Vector3 currentDirection = cam.transform.forward;

            Vector3 newDirection = Vector3.Slerp(currentDirection, aimDirection, Time.deltaTime * aimPullStrength);

            // Apply smoothed aim adjustment by rotating cameraFollowPos toward new direction
            cameraFollowPos.forward = newDirection;
        }

        // 3. Update UI
        UpdateCrosshairFeedback();
    }

    Transform FindClosestTargetToScreenCenter()
    {
        Collider[] candidates = Physics.OverlapSphere(cam.transform.position + cam.transform.forward * detectionRange / 2f, detectionRadius, targetLayer);

        float closestDistance = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (Collider col in candidates)
        {
            Vector3 screenPoint = cam.WorldToScreenPoint(GetTargetCenter(col.transform));

            if (screenPoint.z < 0) continue; // Behind the camera

            float distanceToCenter = Vector2.Distance(screenPoint, new Vector2(Screen.width / 2f, Screen.height / 2f));

            if (distanceToCenter < closestDistance)
            {
                closestDistance = distanceToCenter;
                bestTarget = col.transform;
            }
        }

        return bestTarget;
    }

    Vector3 GetTargetCenter(Transform target)
    {
        Collider col = target.GetComponent<Collider>();
        if (col != null)
        {
            return col.bounds.center;
        }
        return target.position;
    }

    void UpdateCrosshairFeedback()
    {
        if (detectedTarget != null && crosshairImage != null)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(GetTargetCenter(detectedTarget));
            crosshairRect.position = screenPos;

            crosshairImage.color = Color.Lerp(crosshairImage.color, lockedColor, Time.deltaTime * colorLerpSpeed);
        }
        else if (crosshairImage != null)
        {
            crosshairRect.position = originalCrosshairPosition;
            crosshairImage.color = Color.Lerp(crosshairImage.color, normalColor, Time.deltaTime * colorLerpSpeed);
        }
    }
}
