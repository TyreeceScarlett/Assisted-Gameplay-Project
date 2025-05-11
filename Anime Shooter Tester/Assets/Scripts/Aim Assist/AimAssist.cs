using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssist : MonoBehaviour
{
    [Header("Assist Settings")]
    public bool aimAssistEnabled = true;
    public float assistRange = 5f; // How wide the assist cone is
    public float assistStrength = 5f; // How strong the assist snaps toward target
    public LayerMask enemyLayer;

    Camera cam;
    float detectionDistance = 20f; // How far ahead to search for targets

    void Start()
    {
        // ✅ Cache main camera
        if (Camera.main != null)
            cam = Camera.main;
        else
            Debug.LogError("No main camera found! Assign a camera to the 'MainCamera' tag.");
    }

    void Update()
    {
        if (aimAssistEnabled)
            AssistAim();
    }

    void AssistAim()
    {
        if (cam == null) return; // ✅ Safety check

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of screen
        Collider[] hits = Physics.OverlapSphere(ray.origin + ray.direction * detectionDistance, assistRange, enemyLayer);

        if (hits.Length > 0)
        {
            Transform closestTarget = hits[0].transform;
            float closestAngle = Vector3.Angle(cam.transform.forward, closestTarget.position - cam.transform.position);

            foreach (Collider hit in hits)
            {
                float angle = Vector3.Angle(cam.transform.forward, hit.transform.position - cam.transform.position);
                if (angle < closestAngle)
                {
                    closestAngle = angle;
                    closestTarget = hit.transform;
                }
            }

            // ✅ Smoothly rotate camera a tiny bit toward target
            Vector3 targetDir = (closestTarget.position - cam.transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(targetDir);

            // ✅ Smoothed rotation (so it feels soft)
            float smoothFactor = Mathf.Clamp01(assistStrength * Time.deltaTime); // Clamp to avoid overshoot
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRot, smoothFactor);
        }
    }

    // These public methods can be called from UI sliders/buttons!
    public void SetAssistEnabled(bool enabled)
    {
        aimAssistEnabled = enabled;
    }

    public void SetAssistRange(float range)
    {
        assistRange = range;
    }

    public void SetAssistStrength(float strength)
    {
        assistStrength = Mathf.Clamp(strength, 0f, 10f); // ✅ Clamp to prevent crazy values
    }
}
