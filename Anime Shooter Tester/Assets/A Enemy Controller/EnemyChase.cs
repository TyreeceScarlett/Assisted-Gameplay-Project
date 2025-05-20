using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;
    private EnemyHealth enemyHealth;

    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    private bool isGrounded;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>(); // Get the health component
    }

    void Update()
    {
        CheckGrounded();

        // Make sure the enemy is alive, grounded, and player exists
        if (player != null && isGrounded && enemyHealth != null && !enemyHealth.isDead)
        {
            agent.SetDestination(player.position);

            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }
        else
        {
            // Stop moving
            if (agent.enabled)
                agent.SetDestination(transform.position);

            animator.SetFloat("Speed", 0f);
        }
    }

    void CheckGrounded()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f; // Slightly above the base
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundLayer);

        // Debug visualization (optional)
        Debug.DrawRay(rayOrigin, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }
}
