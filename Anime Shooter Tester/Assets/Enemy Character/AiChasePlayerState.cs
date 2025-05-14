using UnityEngine;

public class AiChasePlayerState : AiState
{
    GameObject player;

    public AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public void Enter(AiAgent agent)
    {
        player = null;
    }

    public void Update(AiAgent agent)
    {
        GameObject[] buffer = new GameObject[10];
        int count = agent.sensor.Filter(buffer, "Player");

        Animator animator = agent.GetComponent<Animator>();

        if (count > 0)
        {
            player = buffer[0];

            // Move toward player
            agent.navMeshAgent.isStopped = false;
            agent.navMeshAgent.SetDestination(player.transform.position);

            // Calculate local movement direction
            Vector3 direction = (player.transform.position - agent.transform.position).normalized;
            Vector3 localDirection = agent.transform.InverseTransformDirection(direction);

            float hz = Mathf.Clamp(localDirection.x, -1f, 1f);
            float v = Mathf.Clamp(localDirection.z, -1f, 1f);

            // Update blend tree parameters
            if (animator)
            {
                animator.SetFloat("hzInput", hz, 0.1f, Time.deltaTime);
                animator.SetFloat("vInput", v, 0.1f, Time.deltaTime);
            }

            // Change sensor color to red (player detected)
            agent.sensor.meshColor = Color.red;
        }
        else
        {
            // Stop moving
            agent.navMeshAgent.isStopped = true;

            // Reset blend tree inputs to idle
            if (animator)
            {
                animator.SetFloat("hzInput", 0f, 0.1f, Time.deltaTime);
                animator.SetFloat("vInput", 0f, 0.1f, Time.deltaTime);
            }

            // Change sensor color to yellow (no player)
            agent.sensor.meshColor = Color.yellow;
        }
    }

    public void Exit(AiAgent agent)
    {
        agent.navMeshAgent.isStopped = true;
    }
}
