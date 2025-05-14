using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmartAimAssist : MonoBehaviour
{
    [Header("Assist Settings")]
    public bool aimAssistEnabled = true;
    public float detectionRadius = 3f;
    public float aimPullStrength = 3f;
    public float minLockOnDistance = 0.1f;
    public float maxLockOnDistance = 20f;
    public LayerMask targetLayer;

    [Header("Crosshair UI")]
    public Image crosshairImage;
    public Color normalColor = Color.white;
    public Color lockedColor = Color.red;
    public float colorLerpSpeed = 10f;

    [Header("References")]
    public Transform cameraFollowPos;
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

        detectedTarget = FindClosestTargetInRange();

        if (detectedTarget != null)
        {
            Vector3 targetPoint = GetTargetCenter(detectedTarget);
            Vector3 aimDirection = (targetPoint - cameraFollowPos.position).normalized;
            Vector3 currentDirection = cam.transform.forward;

            Vector3 newDirection = Vector3.Slerp(currentDirection, aimDirection, Time.deltaTime * aimPullStrength);
            cameraFollowPos.rotation = Quaternion.Slerp(cameraFollowPos.rotation, Quaternion.LookRotation(newDirection), Time.deltaTime * aimPullStrength);
        }

        UpdateCrosshairFeedback();
    }

    Transform FindClosestTargetInRange()
    {
        Collider[] candidates = Physics.OverlapSphere(
            cam.transform.position + cam.transform.forward * (maxLockOnDistance * 0.5f),
            detectionRadius,
            targetLayer
        );

        float closestDistanceToCenter = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (Collider col in candidates)
        {
            Transform target = col.transform;
            Vector3 targetCenter = GetTargetCenter(target);
            float distance = Vector3.Distance(cameraFollowPos.position, targetCenter);

            // Skip if out of lock-on range
            if (distance < minLockOnDistance || distance > maxLockOnDistance)
                continue;

            // NEW: Check if enemy has health and is alive
            EnemyHealth enemyHealth = target.GetComponentInParent<EnemyHealth>();
            if (enemyHealth == null || enemyHealth.isDead)
                continue;

            // Check if in view
            Vector3 screenPoint = cam.WorldToScreenPoint(targetCenter);
            if (screenPoint.z < 0) continue;

            float distanceToCenter = Vector2.Distance(screenPoint, new Vector2(Screen.width / 2f, Screen.height / 2f));

            if (distanceToCenter < closestDistanceToCenter)
            {
                closestDistanceToCenter = distanceToCenter;
                bestTarget = target;
            }
        }

        return bestTarget;
    }

    Vector3 GetTargetCenter(Transform target)
    {
        Collider col = target.GetComponent<Collider>();
        return col != null ? col.bounds.center : target.position;
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
