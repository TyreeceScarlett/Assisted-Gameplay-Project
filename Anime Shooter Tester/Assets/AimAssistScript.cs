using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimAssist : MonoBehaviour
{
    [Header("Assist Settings")]
    public bool aimAssistEnabled = true;
    public float assistRange = 5f;
    public float assistStrength = 5f;
    public LayerMask enemyLayer;

    Camera cam;
    float detectionDistance = 20f;

    [Header("Head Lock-On Settings")]
    public Transform enemyHeadTarget; // Drag your cube or enemy here
    public float headLockOnRange = 2f;
    public float headLockOnBonusStrength = 10f;

    private bool isLockedOn = false;

    [Header("Crosshair UI")]
    public Image crosshairImage;
    public Color normalColor = Color.white;
    public Color lockedColor = Color.red;
    public float colorLerpSpeed = 10f;

    [Header("Crosshair Bump Settings")]
    public float bumpScale = 1.5f;
    public float bumpDuration = 0.2f;

    private RectTransform crosshairRect;
    private Vector3 originalScale;
    private float bumpTimer = 0f;

    void Start()
    {
        if (Camera.main != null)
            cam = Camera.main;
        else
            Debug.LogError("No main camera found! Assign a camera to the 'MainCamera' tag.");

        if (crosshairImage != null)
        {
            crosshairImage.color = normalColor;
            crosshairRect = crosshairImage.GetComponent<RectTransform>();
            originalScale = crosshairRect.localScale;
        }
    }

    void Update()
    {
        if (!aimAssistEnabled || cam == null)
            return;

        AssistAim();

        // Lerp crosshair color and bump together
        UpdateCrosshairFeedback();

        if (Input.GetMouseButtonDown(2)) // Middle mouse button
        {
            UnlockHead();
        }
    }

    void AssistAim()
    {
        if (enemyHeadTarget == null || cam == null)
            return;

        // Get the position of the enemy head target
        Vector3 headPos = enemyHeadTarget.position;
        float distanceToHead = Vector3.Distance(cam.transform.position, headPos);

        bool shouldLockOn = false;

        // Check if we are within detection distance
        if (distanceToHead <= detectionDistance)
        {
            Vector3 viewportPoint = cam.WorldToViewportPoint(headPos);
            float screenCenterDist = Vector2.Distance(
                new Vector2(viewportPoint.x, viewportPoint.y),
                new Vector2(0.5f, 0.5f)
            );

            // Lock on when close enough to the center
            if (screenCenterDist <= headLockOnRange / 10f)
            {
                shouldLockOn = true;
            }
        }

        if (shouldLockOn)
        {
            if (!isLockedOn)
            {
                StartCrosshairBump(); // Juicy bump!
            }

            isLockedOn = true;

            // Maintain the lock-on by smoothly rotating towards the target
            MaintainLockOn(headPos);
        }
        else
        {
            isLockedOn = false;
        }

        // Update the crosshair position to follow the moving target
        UpdateCrosshairPosition(headPos);

        // Optional soft aim assist
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));  // Ray from the camera center
        Collider[] hits = Physics.OverlapSphere(ray.origin + ray.direction * detectionDistance, assistRange, enemyLayer);

        Transform closestTarget = null;
        float closestAngle = float.MaxValue;

        foreach (Collider hit in hits)
        {
            float angle = Vector3.Angle(cam.transform.forward, hit.transform.position - cam.transform.position);

            if (angle < closestAngle)
            {
                closestAngle = angle;
                closestTarget = hit.transform;
            }
        }

        if (closestTarget != null)
        {
            Vector3 targetDir = (closestTarget.position - cam.transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(targetDir);

            float smoothFactor = assistStrength * Time.deltaTime;
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRot, smoothFactor);
        }
    }

    // Adjusted method for smooth lock-on rotation
    void MaintainLockOn(Vector3 headPos)
    {
        // Vector from camera to target
        Vector3 targetDir = (headPos - cam.transform.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation(targetDir);

        // Smooth transition to the locked target
        float smoothFactor = headLockOnBonusStrength * Time.deltaTime;
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRot, smoothFactor);
    }

    void UnlockHead()
    {
        isLockedOn = false;
    }

    void UpdateCrosshairFeedback()
    {
        if (crosshairImage == null || crosshairRect == null)
            return;

        // Smooth color fade
        Color targetColor = isLockedOn ? lockedColor : normalColor;
        crosshairImage.color = Color.Lerp(crosshairImage.color, targetColor, Time.deltaTime * colorLerpSpeed);

        // Smooth bump scale
        if (bumpTimer > 0f)
        {
            bumpTimer -= Time.deltaTime;
            float t = 1f - (bumpTimer / bumpDuration);
            float scale = Mathf.Lerp(bumpScale, 1f, t);
            crosshairRect.localScale = originalScale * scale;
        }
        else
        {
            crosshairRect.localScale = originalScale;
        }
    }

    void StartCrosshairBump()
    {
        bumpTimer = bumpDuration;
    }

    // New method to update the crosshair position on the screen
    void UpdateCrosshairPosition(Vector3 targetWorldPos)
    {
        // Convert the target's world position to screen space
        Vector3 screenPos = cam.WorldToScreenPoint(targetWorldPos);

        // Update the crosshair position on the Canvas
        if (crosshairRect != null)
        {
            crosshairRect.position = screenPos;
        }
    }

    // Optional UI controls
    public void SetAssistEnabled(bool enabled)
    {
        aimAssistEnabled = enabled;
        if (!enabled) UnlockHead();
    }

    public void SetAssistRange(float range)
    {
        assistRange = range;
    }

    public void SetAssistStrength(float strength)
    {
        assistStrength = Mathf.Clamp(strength, 0f, 10f);
    }
}
