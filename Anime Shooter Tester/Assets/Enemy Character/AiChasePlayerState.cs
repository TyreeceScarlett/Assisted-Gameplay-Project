using UnityEngine;

public class AiChasePlayerState : AiState
{
    private GameObject player;

    public AiStateId GetId() => AiStateId.ChasePlayer;

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Update(AiAgent agent)
    {
        if (player == null)
        {
            agent.stateMachine.ChangeState(AiStateId.Patrol);
            return;
        }

        // Chase the player
        agent.navMeshAgent.SetDestination(player.transform.position);

        // Lost sight of the player
        if (agent.sensor.Objects.Count == 0)
        {
            Debug.Log("Player lost — returning to patrol");
            Vector3 fallbackPoint = agent.GetRandomPatrolPoint();
            agent.navMeshAgent.SetDestination(fallbackPoint);
            agent.stateMachine.ChangeState(AiStateId.Patrol);
            return;
        }

        // Rotate toward movement direction
        Vector3 velocity = agent.navMeshAgent.velocity;
        if (velocity.sqrMagnitude > 0.01f)
        {
            Quaternion rotation = Quaternion.LookRotation(velocity.normalized);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, rotation, Time.deltaTime * 5f);
        }

        // Animate
        agent.animator.SetFloat("vInput", 1f);
        agent.animator.SetFloat("hzInput", 0f);
    }

    public void Exit(AiAgent agent)
    {
        agent.animator.SetFloat("vInput", 0f);
    }
}
