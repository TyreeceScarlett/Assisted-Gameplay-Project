using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiLocomotion : MonoBehaviour
{
    public Transform playerTransform;
    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rb;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (agent != null)
        {
            // Let gravity control the Y-axis instead of the NavMeshAgent
            agent.updateUpAxis = false;
            agent.updatePosition = true;
        }

        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        if (animator != null)
        {
            animator.applyRootMotion = false; // We are controlling movement via NavMeshAgent
        }

        if (groundCheck == null)
        {
            Debug.LogWarning("GroundCheck Transform not assigned. Please set it in the inspector.");
        }
    }

    void Update()
    {
        CheckGrounded();

        if (isGrounded && playerTransform != null)
        {
            agent.SetDestination(playerTransform.position);
        }

        SnapToGround(); // Ensure the character sticks to the terrain

        if (animator != null)
        {
            float speed = isGrounded ? agent.velocity.magnitude : 0f;
            animator.SetFloat("Speed", speed);
        }
    }

    void CheckGrounded()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        }
    }

    void SnapToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 2f, groundMask))
        {
            // Match the character's Y-position to the hit point
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
    }
}
