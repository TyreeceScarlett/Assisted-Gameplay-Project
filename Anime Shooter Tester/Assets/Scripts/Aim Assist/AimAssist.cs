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

    private Camera cam;
    private float detectionDistance = 20f;

    [Header("Head Lock-On Settings")]
    public Transform enemyHeadTarget;
    public float headLockOnRange = 2f;
    public float headLockOnBonusStrength = 10f;

    private bool isLockedOn = false;

    [Header("Crosshair UI")]
    public Image crosshairImage;
    public Color normalColor = Color.white;
    public Color lockedColor = Color.red;
    public float colorLerpSpeed = 10f;

    [Header("Crosshair Bump Settings")]
    public AnimationCurve bumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float bumpScale = 1.5f;
    public float bumpDuration = 0.25f;

    private RectTransform crosshairRect;
    private Vector3 originalScale;
    private float bumpTimer = 0f;
    private bool isBumping = false;

    [Header("Optional Lock-On Sound")]
    public AudioSource lockOnAudio;

    private bool wasLockedLastFrame = false;

    void Start()
    {
        if (Camera.main != null)
            cam = Camera.main;
        else
            Debug.LogError("No main camera tagged as MainCamera!");

        if (crosshairImage != null)
        {
            crosshairRect = crosshairImage.GetComponent<RectTransform>();
            originalScale = crosshairRect.localScale;
            crosshairImage.color = normalColor;
        }
    }

    void Update()
    {
        if (!aimAssistEnabled || cam == null)
            return;

        HandleAimLogic();
        UpdateCrosshairVisuals();

        if (Input.GetMouseButtonDown(2)) // Middle click = manual unlock
        {
            UnlockHead();
        }

        wasLockedLastFrame = isLockedOn;
    }

    void HandleAimLogic()
    {
        if (enemyHeadTarget == null)
            return;

        Vector3 headPos = enemyHeadTarget.position;
        float dist = Vector3.Distance(cam.transform.position, headPos);

        bool shouldLockOn = false;

        if (dist <= detectionDistance)
        {
            Vector3 viewportPoint = cam.WorldToViewportPoint(headPos);
            float centerDist = Vector2.Distance(new Vector2(viewportPoint.x, viewportPoint.y), new Vector2(0.5f, 0.5f));

            if (centerDist <= headLockOnRange / 10f)
                shouldLockOn = true;
        }

        if (shouldLockOn)
        {
            if (!wasLockedLastFrame)
            {
                TriggerCrosshairBump();
                if (lockOnAudio != null) lockOnAudio.Play();
            }

            isLockedOn = true;
            MaintainLockOn();
        }
        else
        {
            isLockedOn = false;
        }

        // Soft aim (optional)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Collider[] hits = Physics.OverlapSphere(ray.origin + ray.direction * detectionDistance, assistRange, enemyLayer);

        Transform closest = null;
        float closestAngle = float.MaxValue;

        foreach (Collider hit in hits)
        {
            float angle = Vector3.Angle(cam.transform.forward, hit.transform.position - cam.transform.position);
            if (angle < closestAngle)
            {
                closestAngle = angle;
                closest = hit.transform;
            }
        }

        if (closest != null)
        {
            Vector3 dir = (closest.position - cam.transform.position).normalized;
            Quaternion rot = Quaternion.LookRotation(dir);
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, rot, assistStrength * Time.deltaTime);
        }
    }

    void MaintainLockOn()
    {
        Vector3 dir = (enemyHeadTarget.position - cam.transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, rot, headLockOnBonusStrength * Time.deltaTime);
    }

    void UnlockHead()
    {
        isLockedOn = false;
    }

    void UpdateCrosshairVisuals()
    {
        if (crosshairImage == null || crosshairRect == null)
            return;

        // Color fade
        Color target = isLockedOn ? lockedColor : normalColor;
        crosshairImage.color = Color.Lerp(crosshairImage.color, target, Time.deltaTime * colorLerpSpeed);

        // Elastic bump using AnimationCurve
        if (isBumping)
        {
            bumpTimer += Time.deltaTime;
            float t = Mathf.Clamp01(bumpTimer / bumpDuration);
            float curveScale = bumpCurve.Evaluate(t);
            crosshairRect.localScale = originalScale * Mathf.Lerp(1f, bumpScale, curveScale);

            if (t >= 1f)
            {
                crosshairRect.localScale = originalScale;
                isBumping = false;
            }
        }
    }

    void TriggerCrosshairBump()
    {
        isBumping = true;
        bumpTimer = 0f;
    }

    // Optional UI setters
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
