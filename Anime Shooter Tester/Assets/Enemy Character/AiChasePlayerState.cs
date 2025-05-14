using UnityEngine;

public class AiChasePlayerState : AiState
{
    GameObject player;

    public override AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public override void Enter(AiAgent agent)
    {
        player = null;
    }

    public override void Update(AiAgent agent)
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

    public override void Exit(AiAgent agent)
    {
        agent.navMeshAgent.isStopped = true;
    }
}
