using UnityEngine;

public class AiChasePlayerState : AiState
{
    public AiStateId GetId() => AiStateId.ChasePlayer;

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
    }

    public void Update(AiAgent agent)
    {
        if (agent.target == null)
        {
            agent.stateMachine.ChangeState(AiStateId.Patrol);
            return;
        }

        agent.navMeshAgent.SetDestination(agent.target.position);

        // Smooth rotation toward movement
        if (agent.navMeshAgent.desiredVelocity != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(agent.navMeshAgent.desiredVelocity.normalized);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.target.position);
        if (distanceToPlayer > agent.sensor.distance)
        {
            agent.stateMachine.ChangeState(AiStateId.Patrol);
        }

        // Animation
        agent.animator.SetFloat("vInput", 1);
        agent.animator.SetFloat("hzInput", 0);
    }

    public void Exit(AiAgent agent)
    {
        agent.navMeshAgent.isStopped = true;
    }
}
