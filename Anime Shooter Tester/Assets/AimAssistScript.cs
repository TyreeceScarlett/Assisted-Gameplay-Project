using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SmartAimAssist : MonoBehaviour
{
    [Header("Assist Settings")]
    public bool aimAssistEnabled = true;
    public float detectionRadius = 5f;
    public float aimPullStrength = 5f; // Lower strength for smooth subtle movement
    public float minLockOnDistance = 1f;
    public float maxLockOnDistance = 15f;

    [Header("Target Objects")]
    public Transform[] targets;

    [Header("Crosshair UI")]
    public RectTransform crosshairRect;
    public Color normalColor = Color.white;
    public Color lockedColor = Color.red;
    public float colorLerpSpeed = 10f;

    [Header("Camera and Target")]
    public Transform cameraFollowPos;
    private Camera cam;
    private Transform detectedTarget;

    private float lockoutTimer = 0f;
    private const float lockoutDuration = 10f;

    void Start()
    {
        cam = Camera.main;
        if (cam == null) Debug.LogError("No main camera found!");
    }

    void Update()
    {
        if (!aimAssistEnabled || cam == null)
            return;

        if (lockoutTimer > 0f)
            lockoutTimer -= Time.deltaTime;

        detectedTarget = lockoutTimer <= 0f ? FindClosestTargetInRange() : null;

        UpdateCrosshairFeedback();

        if (Input.GetMouseButtonDown(2)) // Middle mouse button unlock
        {
            UnlockTarget();
            lockoutTimer = lockoutDuration;
        }
    }

    Transform FindClosestTargetInRange()
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform target in targets)
        {
            if (target == null) continue;

            Vector3 targetCenter = GetTargetCenter(target);
            float distance = Vector3.Distance(cameraFollowPos.position, targetCenter);

            if (distance < minLockOnDistance || distance > maxLockOnDistance)
                continue;

            Vector3 screenPoint = cam.WorldToScreenPoint(targetCenter);
            if (screenPoint.z < 0) continue;

            float distanceToCenter = Vector2.Distance(screenPoint, new Vector2(Screen.width / 2f, Screen.height / 2f));

            if (distanceToCenter < closestDistance)
            {
                closestDistance = distanceToCenter;
                closestTarget = target;
            }
        }

        return closestTarget;
    }

    Vector3 GetTargetCenter(Transform target)
    {
        Collider col = target.GetComponent<Collider>();
        return col != null ? col.bounds.center : target.position;
    }

    void UnlockTarget()
    {
        detectedTarget = null;
        if (crosshairRect != null)
        {
            crosshairRect.anchoredPosition = Vector2.zero;
        }
    }

    void UpdateCrosshairFeedback()
    {
        if (crosshairRect == null) return;

        Image crosshairImage = crosshairRect.GetComponent<Image>();
        if (crosshairImage == null) return;

        if (detectedTarget != null)
        {
            Vector3 worldPos = GetTargetCenter(detectedTarget);
            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0f)
            {
                Vector2 localPoint;
                RectTransform canvasRect = crosshairRect.parent as RectTransform;

                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint))
                {
                    // Smoothly move the crosshair toward the target screen position
                    crosshairRect.anchoredPosition = Vector2.Lerp(crosshairRect.anchoredPosition, localPoint, Time.deltaTime * aimPullStrength);
                }

                crosshairImage.color = Color.Lerp(crosshairImage.color, lockedColor, Time.deltaTime * colorLerpSpeed);
            }
        }
        else
        {
            crosshairRect.anchoredPosition = Vector2.Lerp(crosshairRect.anchoredPosition, Vector2.zero, Time.deltaTime * aimPullStrength);
            crosshairImage.color = Color.Lerp(crosshairImage.color, normalColor, Time.deltaTime * colorLerpSpeed);
        }
    }
}
