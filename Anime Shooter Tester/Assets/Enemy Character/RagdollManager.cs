using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollManager : MonoBehaviour
{
    private Rigidbody[] rbs;
    private Animator animator;

    void Awake()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();

        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }

    public void TriggerRagdoll()
    {
        if (animator != null) animator.enabled = false;

        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = false;
        }
    }
}
