using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmartAimAssist : MonoBehaviour
{
    [Header("Assist Settings")]
    public bool aimAssistEnabled = true;
    public float detectionRadius = 20f;
    public float aimPullStrength = 5f;
    public float minLockOnDistance = 1f;
    public float maxLockOnDistance = 25f;
    public float maxTargetAngle = 45f;
    public float predictionTime = 0.25f;

    [Header("Target Detection")]
    public string targetTag = "Enemy";
    private List<Transform> dynamicTargets = new List<Transform>();
    private float targetRefreshRate = 1f;
    private float targetRefreshTimer = 0f;

    [Header("Crosshair UI")]
    public RectTransform crosshairRect;
    public Color normalColor = Color.white;
    public Color lockedColor = Color.red;
    public float colorLerpSpeed = 10f;
    public bool snapToTarget = false;

    [Header("Lockout Settings")]
    public float lockoutDuration = 10f;

    [Header("References")]
    public Transform cameraFollowPos;
    public MovementStateManager playerMovement;
    public AimStateManager aimStateManager;  // Reference your AimStateManager here
    private Camera cam;
    private Transform detectedTarget;

    [Header("Bullet Firing Settings")]
    public Transform barrelPos;
    public GameObject bulletPrefab;
    public float bulletVelocity = 50f;

    private float lockoutTimer = 0f;

    void Start()
    {
        cam = Camera.main;
        if (cam == null) Debug.LogError("No main camera found!");
    }

    void Update()
    {
        if (!aimAssistEnabled || cam == null) return;

        // Refresh target list periodically
        targetRefreshTimer += Time.deltaTime;
        if (targetRefreshTimer >= targetRefreshRate)
        {
            UpdateTargetList();
            targetRefreshTimer = 0f;
        }

        if (lockoutTimer > 0f)
            lockoutTimer -= Time.deltaTime;

        // Only find target if not locked out
        if (lockoutTimer <= 0f)
            detectedTarget = FindClosestTargetInRange();
        else
            detectedTarget = null;

        UpdateCrosshairFeedback();

        if (detectedTarget != null && aimStateManager != null)
        {
            Vector3 predictedPos = PredictTargetPosition(detectedTarget);

            // Override aim rotation to smoothly look at predicted moving target
            aimStateManager.overrideAimRotation = true;
            aimStateManager.overrideLookPos = predictedPos;

            // Rotate player towards predicted aim point
            if (playerMovement != null)
                playerMovement.RotateToward(predictedPos);
        }
        else
        {
            if (aimStateManager != null)
                aimStateManager.overrideAimRotation = false;
        }

        if (Input.GetMouseButtonDown(2)) // Middle click to unlock target
        {
            UnlockTarget();
            lockoutTimer = lockoutDuration;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetMouseButtonDown(0)) // Left click to fire bullet
        {
            FireBullet();
        }
    }

    void UpdateTargetList()
    {
        dynamicTargets.Clear();
        GameObject[] foundTargets = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (var obj in foundTargets)
        {
            if (obj != null)
                dynamicTargets.Add(obj.transform);
        }
    }

    Transform FindClosestTargetInRange()
    {
        Transform closestTarget = null;
        float closestScreenDistance = Mathf.Infinity;

        foreach (Transform target in dynamicTargets)
        {
            if (target == null) continue;

            EnemyHealth enemyHealth = target.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null && enemyHealth.isDead) continue;

            Vector3 targetCenter = GetTargetCenter(target);
            float distance = Vector3.Distance(cameraFollowPos.position, targetCenter);

            if (distance < minLockOnDistance || distance > maxLockOnDistance)
                continue;

            float angle = Vector3.Angle(cam.transform.forward, targetCenter - cam.transform.position);
            if (angle > maxTargetAngle)
                continue;

            if (Physics.Linecast(cameraFollowPos.position, targetCenter, out RaycastHit hit))
            {
                if (hit.transform != target && hit.transform.root != target.root)
                    continue;
            }

            Vector3 screenPoint = cam.WorldToScreenPoint(targetCenter);
            if (screenPoint.z < 0f) continue;

            float distanceToScreenCenter = Vector2.Distance(screenPoint, new Vector2(Screen.width / 2f, Screen.height / 2f));

            if (distanceToScreenCenter < closestScreenDistance)
            {
                closestScreenDistance = distanceToScreenCenter;
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

    Vector3 PredictTargetPosition(Transform target)
    {
        Rigidbody rb = target.GetComponent<Rigidbody>();
        Vector3 velocity = rb ? rb.velocity : Vector3.zero;
        return GetTargetCenter(target) + velocity * predictionTime;
    }

    void UnlockTarget()
    {
        detectedTarget = null;
        if (crosshairRect != null)
            crosshairRect.anchoredPosition = Vector2.zero;

        if (aimStateManager != null)
            aimStateManager.overrideAimRotation = false;
    }

    void UpdateCrosshairFeedback()
    {
        if (crosshairRect == null || cam == null) return;

        Image crosshairImage = crosshairRect.GetComponent<Image>();
        if (crosshairImage == null) return;

        if (detectedTarget != null)
        {
            Vector3 worldPos = PredictTargetPosition(detectedTarget);
            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

            if (screenPos.z > 0f)
            {
                RectTransform canvasRect = crosshairRect.parent as RectTransform;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 localPoint))
                {
                    if (snapToTarget)
                        crosshairRect.anchoredPosition = localPoint;
                    else
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

    public void FireBullet()
    {
        if (bulletPrefab == null || barrelPos == null) return;

        Vector3 shootTarget = GetCurrentAimPoint();
        Vector3 shootDirection = (shootTarget - barrelPos.position).normalized;

        GameObject currentBullet = Instantiate(bulletPrefab, barrelPos.position, Quaternion.LookRotation(shootDirection));
        Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.drag = 0f;
            rb.velocity = shootDirection * bulletVelocity;
        }
    }

    public Vector3 GetCurrentAimPoint()
    {
        if (aimStateManager != null && aimStateManager.overrideAimRotation)
        {
            return aimStateManager.overrideLookPos;
        }
        else if (cam != null)
        {
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                return hit.point;
            else
                return ray.GetPoint(100f);
        }
        else
        {
            return Vector3.zero;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (cameraFollowPos != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(cameraFollowPos.position, detectionRadius);
        }
    }
}
