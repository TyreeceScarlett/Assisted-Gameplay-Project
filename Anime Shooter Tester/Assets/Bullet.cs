using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 60f; // 1 minute lifetime
    Rigidbody rb;

    void Start()
    {
        Destroy(gameObject, timeToDestroy);
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false; // Make bullet fly straight, no drop
            rb.drag = 0f;          // No slowdown over time
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}