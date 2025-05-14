using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SmartAimAssist : MonoBehaviour
{
    [Header("Assist Settings")]
    public bool aimAssistEnabled = true;
    public float detectionRadius = 5f; // Reasonable radius for aiming assistance (adjustable)
    public float aimPullStrength = 20f; // Moderate pull strength for a subtle effect (adjustable)
    public float minLockOnDistance = 1f; // Minimum distance for locking onto a target (adjustable)
    public float maxLockOnDistance = 15f; // Maximum distance for locking onto a target (adjustable)

    [Header("Target Objects")]
    public Transform[] targets; // Array for dragging and dropping targets

    [Header("Crosshair UI")]
    public RectTransform crosshairRect;
    public Color normalColor = Color.white;
    public Color lockedColor = Color.red;
    public float colorLerpSpeed = 10f;

    [Header("Camera and Target")]
    public Transform cameraFollowPos;
    private Camera cam;
    private Transform detectedTarget;
    private Transform originalCameraFollowPos;

    // Cooldown after unlock
    private float lockoutTimer = 0f;
    private const float lockoutDuration = 10f; // Adjust lockout duration to 10 seconds

    private Vector3 initialCameraPosition;

    void Start()
    {
        cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("No main camera found!");
        }

        if (cameraFollowPos != null)
        {
            originalCameraFollowPos = cameraFollowPos;
        }

        // Store initial camera position to allow smooth camera movement
        initialCameraPosition = cam.transform.position;
    }

    void Update()
    {
        if (!aimAssistEnabled || cam == null)
            return;

        // Reduce lockout timer
        if (lockoutTimer > 0f)
            lockoutTimer -= Time.deltaTime;

        // Handle camera rotation
        HandleCameraRotation();

        // Only detect target if not in lockout
        detectedTarget = lockoutTimer <= 0f ? FindClosestTargetInRange() : null;

        if (detectedTarget != null)
        {
            // Make the player look at the target smoothly while preventing flipping
            Vector3 targetPosition = detectedTarget.position;
            targetPosition.y = transform.position.y; // Keep the Y value to prevent flipping
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * aimPullStrength);

            // Update the crosshair
            UpdateCrosshairFeedback();
        }

        if (Input.GetMouseButtonDown(2)) // Middle mouse button
        {
            UnlockTarget();
            lockoutTimer = lockoutDuration;
        }
    }

    void HandleCameraRotation()
    {
        if (Input.GetMouseButton(2)) // Middle mouse button pressed
        {
            // Lock the camera rotation (no movement)
            return;
        }

        // Free camera rotation logic when not locked
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");

        // Rotate around the target (camera follow position)
        cam.transform.RotateAround(cameraFollowPos.position, Vector3.up, horizontal * 5f);
        cam.transform.RotateAround(cameraFollowPos.position, cam.transform.right, -vertical * 5f);
    }

    Transform FindClosestTargetInRange()
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform target in targets)
        {
            if (target == null) continue; // Skip null targets

            Vector3 targetCenter = GetTargetCenter(target);
            float distance = Vector3.Distance(cameraFollowPos.position, targetCenter);

            if (distance < minLockOnDistance || distance > maxLockOnDistance)
                continue;

            Vector3 screenPoint = cam.WorldToScreenPoint(targetCenter);
            if (screenPoint.z < 0) continue; // Skip targets behind the camera

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

        if (cameraFollowPos != null && originalCameraFollowPos != null)
        {
            cameraFollowPos.position = Vector3.Lerp(cameraFollowPos.position, originalCameraFollowPos.position, Time.deltaTime * 5f);
        }

        if (crosshairRect != null)
        {
            crosshairRect.anchoredPosition = Vector2.zero; // Reset crosshair position to center
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
                    crosshairRect.anchoredPosition = localPoint;
                }

                crosshairImage.color = Color.Lerp(crosshairImage.color, lockedColor, Time.deltaTime * colorLerpSpeed);
            }
        }
        else
        {
            crosshairRect.anchoredPosition = Vector2.zero; // Reset crosshair position to center
            crosshairImage.color = Color.Lerp(crosshairImage.color, normalColor, Time.deltaTime * colorLerpSpeed);
        }
    }
}
