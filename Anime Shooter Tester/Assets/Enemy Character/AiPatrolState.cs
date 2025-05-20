using UnityEngine;

public class AiPatrolState : AiState
{
    private Vector3 currentTarget;

    public AiStateId GetId()
    {
        return AiStateId.Patrol;
    }

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
        PickNewDestination(agent);
    }

    public void Update(AiAgent agent)
    {
        // Check if reached destination or close enough
        if (!agent.navMeshAgent.pathPending && agent.navMeshAgent.remainingDistance < 0.5f)
        {
            PickNewDestination(agent);
        }

        // Check for player detection
        foreach (var obj in agent.sensor.Objects)
        {
            if (obj.CompareTag("Player"))
            {
                Debug.Log("Chase Player Detected");
                agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
                return;
            }
        }

        // If player is lost and agent is outside patrol radius, return to patrol center
        Vector3 toCenter = agent.patrolCenter - agent.transform.position;
        if (toCenter.magnitude > agent.patrolRadius)
        {
            agent.navMeshAgent.SetDestination(agent.patrolCenter);
        }
    }

    public void Exit(AiAgent agent)
    {
        agent.navMeshAgent.isStopped = true;
    }

    private void PickNewDestination(AiAgent agent)
    {
        currentTarget = agent.GetRandomPatrolPoint();
        agent.navMeshAgent.SetDestination(currentTarget);
    }
}
