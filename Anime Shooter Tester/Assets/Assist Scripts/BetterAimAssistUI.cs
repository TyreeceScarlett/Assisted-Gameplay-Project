using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BetterAimAssistWithUI : MonoBehaviour
{
    [Header("General Settings")]
    public Camera cam;
    public Transform cameraTransform;
    public Transform barrelTransform;
    public string enemyTag = "Enemy";

    [Header("Sticky View")]
    public bool enableStickyView = true;
    public float stickyDetectionRadius = 20f;
    public float stickySlowDownFactor = 0.3f;
    public float stickyMaxAngle = 10f;

    [Header("Assisted Tracking")]
    public bool enableAssistedTracking = true;
    public float trackingDetectionRadius = 20f;
    public float trackingSpeed = 5f;
    public float trackingMaxAngle = 30f;

    [Header("ADS Snapping")]
    public bool enableADSSnapping = true;
    public bool isADS = false;
    public float adsDetectionRadius = 30f;
    public float adsSnapSpeed = 10f;

    [Header("Bullet Magnetism")]
    public bool enableBulletMagnetism = true;
    public float magnetRadius = 2f;
    public float magnetStrength = 1000f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;

    [Header("UI Settings")]
    public RectTransform crosshairMarker; // UI crosshair RawImage RectTransform (assign in Inspector)
    public Vector3 headOffset = new Vector3(0, 0.2f, 0); // Adjust for head height offset

    [Header("UI Blocking")]
    public List<Transform> blockedCanvases = new List<Transform>();

    // Internal states
    public Transform stickyTarget { get; private set; }
    public Transform trackingTarget { get; private set; }
    public Transform adsTarget { get; private set; }
    public Transform bulletMagnetTarget { get; private set; }

    private bool isStickyActive = false;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;
        if (cameraTransform == null && cam != null)
            cameraTransform = cam.transform;

        if (cam == null)
            Debug.LogError("BetterAimAssist: No main camera found!");
        if (barrelTransform == null)
            Debug.LogError("BetterAimAssist: Barrel transform not assigned!");

        if (crosshairMarker != null)
            crosshairMarker.gameObject.SetActive(false); // Hide crosshair initially
    }

    void Update()
    {
        HandleCursor();

        if (enableStickyView)
            UpdateStickyView();

        if (enableAssistedTracking)
            UpdateAssistedTracking();

        if (enableADSSnapping && isADS)
            UpdateADSSnapping();
        else
            adsTarget = null;

        UpdateCrosshairMarker();
    }

    void HandleCursor()
    {
        if (Input.GetMouseButtonDown(2) || IsPointerOverBlockedUI())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (!IsPointerOverBlockedUI() && !Input.GetMouseButton(2))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    bool IsPointerOverBlockedUI()
    {
        if (EventSystem.current == null || blockedCanvases == null || blockedCanvases.Count == 0)
            return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            foreach (var canvasTransform in blockedCanvases)
            {
                if (canvasTransform != null && result.gameObject.transform.IsChildOf(canvasTransform))
                    return true;
            }
        }

        return false;
    }

    #region Sticky View
    public float GetModifiedRotationSpeed(float originalSpeed)
    {
        if (!enableStickyView || !isStickyActive)
            return originalSpeed;
        return originalSpeed * stickySlowDownFactor;
    }

    void UpdateStickyView()
    {
        isStickyActive = false;
        stickyTarget = null;

        Collider[] hits = Physics.OverlapSphere(cameraTransform.position, stickyDetectionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                Vector3 dirToEnemy = (hit.bounds.center - cameraTransform.position).normalized;
                float angle = Vector3.Angle(cameraTransform.forward, dirToEnemy);
                if (angle < stickyMaxAngle)
                {
                    isStickyActive = true;
                    stickyTarget = hit.transform;
                    break;
                }
            }
        }
    }
    #endregion

    #region Assisted Tracking
    void UpdateAssistedTracking()
    {
        trackingTarget = FindClosestTarget(cameraTransform.position, trackingDetectionRadius, trackingMaxAngle);
        if (trackingTarget != null)
        {
            Vector3 dirToTarget = (trackingTarget.position - cameraTransform.position).normalized;
            Quaternion desiredRot = Quaternion.LookRotation(dirToTarget);
            cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, desiredRot, Time.deltaTime * trackingSpeed);
        }
    }
    #endregion

    #region ADS Snapping
    void UpdateADSSnapping()
    {
        adsTarget = FindClosestTarget(cameraTransform.position, adsDetectionRadius, 30f);
        if (adsTarget != null)
        {
            Vector3 targetCenter = GetTargetCenter(adsTarget);
            Vector3 dirToTarget = (targetCenter - cam.transform.position).normalized;
            Quaternion desiredRot = Quaternion.LookRotation(dirToTarget);
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, desiredRot, Time.deltaTime * adsSnapSpeed);
        }
    }
    #endregion

    #region Bullet Magnetism
    public void Fire()
    {
        if (bulletPrefab == null || barrelTransform == null)
        {
            Debug.LogWarning("BetterAimAssist: Missing bullet prefab or barrel transform.");
            return;
        }

        Vector3 aimPoint = GetMagnetizedAimPoint();
        Vector3 dir = (aimPoint - barrelTransform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, Quaternion.LookRotation(dir));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = dir * bulletSpeed;
        }
    }

    Vector3 GetMagnetizedAimPoint()
    {
        bulletMagnetTarget = null;

        if (!enableBulletMagnetism)
            return GetDefaultAimPoint();

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        RaycastHit[] hits = Physics.SphereCastAll(ray, magnetRadius, 100f);

        Transform bestTarget = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag(enemyTag))
            {
                float dist = Vector3.Distance(hit.point, ray.origin + ray.direction * hit.distance);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    bestTarget = hit.transform;
                }
            }
        }

        if (bestTarget != null)
        {
            bulletMagnetTarget = bestTarget;
            Collider col = bestTarget.GetComponent<Collider>();
            return col != null ? col.bounds.center : bestTarget.position;
        }

        return GetDefaultAimPoint();
    }

    Vector3 GetDefaultAimPoint()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            return hit.point;
        else
            return ray.GetPoint(100f);
    }
    #endregion

    #region UI Crosshair Marker Update
    void UpdateCrosshairMarker()
    {
        if (crosshairMarker == null)
            return;

        // Priority: stickyTarget > trackingTarget > adsTarget
        Transform target = stickyTarget ?? trackingTarget ?? adsTarget;

        if (target == null)
        {
            crosshairMarker.gameObject.SetActive(false);
            return;
        }

        Vector3 headWorldPos = target.position + headOffset;

        Vector3 screenPos = cam.WorldToScreenPoint(headWorldPos);

        bool isVisible = screenPos.z > 0 &&
                         screenPos.x >= 0 && screenPos.x <= Screen.width &&
                         screenPos.y >= 0 && screenPos.y <= Screen.height;

        crosshairMarker.gameObject.SetActive(isVisible);

        if (!isVisible)
            return;

        Vector2 anchoredPos;
        RectTransform canvasRect = crosshairMarker.parent as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, cam, out anchoredPos);

        crosshairMarker.anchoredPosition = anchoredPos;

        // Optional: scale crosshair based on distance (closer = bigger)
        float distance = Vector3.Distance(cam.transform.position, target.position);
        float scale = Mathf.Clamp(1f / distance * 10f, 0.5f, 1.5f);
        crosshairMarker.localScale = Vector3.one * scale;
    }
    #endregion

    #region Utility
    Transform FindClosestTarget(Vector3 origin, float radius, float maxAngle)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius);
        Transform bestTarget = null;
        float closestAngle = maxAngle;

        foreach (var hit in hits)
        {
            if (hit.CompareTag(enemyTag))
            {
                Vector3 dirToEnemy = (hit.bounds.center - origin).normalized;
                float angle = Vector3.Angle(cameraTransform.forward, dirToEnemy);
                if (angle < closestAngle)
                {
                    bestTarget = hit.transform;
                    closestAngle = angle;
                }
            }
        }

        return bestTarget;
    }

    Vector3 GetTargetCenter(Transform target)
    {
        Collider col = target.GetComponent<Collider>();
        return col != null ? col.bounds.center : target.position;
    }
    #endregion
}
