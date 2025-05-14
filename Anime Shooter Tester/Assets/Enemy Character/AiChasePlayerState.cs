using UnityEngine;

public class AiChasePlayerState : MonoBehaviour, AiState
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

            // Play walk animation
            if (animator) animator.SetBool("IsWalking", true);

            // Sensor color red = can see player
            agent.sensor.meshColor = Color.red;
        }
        else
        {
            // No player in sight
            agent.navMeshAgent.isStopped = true;

            // Stop walk animation
            if (animator) animator.SetBool("IsWalking", false);

            // Sensor color yellow = can't see player
            agent.sensor.meshColor = Color.yellow;
        }
    }

    public void Exit(AiAgent agent)
    {
        agent.navMeshAgent.isStopped = true;
    }
}