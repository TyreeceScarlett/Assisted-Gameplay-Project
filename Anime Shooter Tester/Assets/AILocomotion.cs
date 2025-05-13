using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiLocomotion : MonoBehaviour
{
    public Transform playerTransform;
    private NavMeshAgent agent;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            animator.applyRootMotion = false; // Disable root motion
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            agent.destination = playerTransform.position;
            Debug.Log("Destination set to: " + agent.destination);
        }

        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
            Debug.Log("Agent velocity: " + speed);
        }
    }
}
