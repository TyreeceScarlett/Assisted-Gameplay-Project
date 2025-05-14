using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiChasePlayerState : AiState
{
    public Transform playerTransform;
    float timer = 0.0f;
    int playerLayerMask;
    Animator animator;
    Renderer detectionIndicator;  // To change color based on vision
    float groundedThreshold = 0.1f; // Threshold to determine if the enemy is grounded
    float originalYPosition; // The initial Y position for checking floating

    public AiStateId GetId() => AiStateId.ChasePlayer;

    public void Enter(AiAgent agent)
    {
        playerLayerMask = LayerMask.GetMask("Player");

        // Get player by layer
        Collider[] hits = Physics.OverlapSphere(agent.transform.position, 1000f, playerLayerMask);
        if (hits.Length > 0)
        {
            playerTransform = hits[0].transform;
        }
        else
        {
            Debug.LogWarning("No object found on the 'Player' layer.");
        }

        animator = agent.GetComponent<Animator>();
        detectionIndicator = agent.GetComponentInChildren<Renderer>(); // Assuming the indicator is part of the enemy's mesh

        // Save the initial Y position
        originalYPosition = agent.transform.position.y;
    }

    public void Update(AiAgent agent)
    {
        if (!agent.enabled || playerTransform == null)
            return;

        // Check if grounded (Y position close to 0)
        if (Mathf.Abs(agent.transform.position.y - originalYPosition) > groundedThreshold)
        {
            Debug.LogWarning("Enemy is floating! Correcting Y position.");
            // Correct the Y position or log a warning
            agent.transform.position = new Vector3(agent.transform.position.x, originalYPosition, agent.transform.position.z);
        }

        // Check visibility with raycast
        Vector3 origin = agent.transform.position + Vector3.up * 1.5f; // eye height
        Vector3 dirToPlayer = (playerTransform.position - origin).normalized;
        float distanceToPlayer = Vector3.Distance(origin, playerTransform.position);

        bool canSeePlayer = false;

        if (Physics.Raycast(origin, dirToPlayer, out RaycastHit hit, distanceToPlayer + 1f))
        {
            if (hit.transform == playerTransform)
            {
                canSeePlayer = true;
            }
        }

        // Update the detection indicator color based on vision
        if (detectionIndicator != null)
        {
            Color indicatorColor = canSeePlayer ? Color.red : Color.yellow;
            detectionIndicator.material.color = indicatorColor;
        }

        if (!canSeePlayer)
        {
            // Player not visible, stop movement and animation
            animator?.SetFloat("Speed", 0f);
            return;
        }

        // Continue chasing
        timer -= Time.deltaTime;

        if (!agent.navMeshAgent.hasPath)
        {
            agent.navMeshAgent.destination = playerTransform.position;
        }

        if (timer < 0.0f)
        {
            Vector3 direction = playerTransform.position - agent.navMeshAgent.destination;
            direction.y = 0;

            if (direction.sqrMagnitude > agent.config.maxDistance * agent.config.maxDistance)
            {
                if (agent.navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial)
                {
                    agent.navMeshAgent.destination = playerTransform.position;
                }
                timer = agent.config.maxTime;
            }
        }

        // Update walk animation
        float speed = agent.navMeshAgent.velocity.magnitude;
        animator?.SetFloat("Speed", speed);
    }

    public void Exit(AiAgent agent)
    {
        animator?.SetFloat("Speed", 0f);
    }
}
