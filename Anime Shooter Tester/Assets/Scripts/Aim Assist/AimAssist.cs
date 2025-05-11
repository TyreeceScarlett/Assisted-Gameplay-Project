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

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (aimAssistEnabled)
            AssistAim();
    }

    void AssistAim()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of screen
        Collider[] hits = Physics.OverlapSphere(ray.origin + ray.direction * 20f, assistRange, enemyLayer);

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

            // Smoothly rotate camera a tiny bit toward target
            Vector3 targetDir = (closestTarget.position - cam.transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRot, assistStrength * Time.deltaTime);
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
        assistStrength = strength;
    }
}