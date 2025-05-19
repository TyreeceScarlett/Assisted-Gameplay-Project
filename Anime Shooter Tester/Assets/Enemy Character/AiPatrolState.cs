using UnityEngine;

public class AiPatrolState : AiState
{
    private Vector3 pointA;
    private Vector3 pointB;
    private Vector3 currentTarget;

    public AiPatrolState(Vector3 pointA, Vector3 pointB)
    {
        this.pointA = pointA;
        this.pointB = pointB;
        this.currentTarget = pointA;
    }

    public AiStateId GetId()
    {
        return AiStateId.Patrol;
    }

    public void Enter(AiAgent agent)
    {
        agent.navMeshAgent.isStopped = false;
        agent.navMeshAgent.SetDestination(currentTarget);
    }

    public void Update(AiAgent agent)
    {
        // Smooth rotation
        if (agent.navMeshAgent.desiredVelocity != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(agent.navMeshAgent.desiredVelocity.normalized);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        // Arrived at patrol point
        if (!agent.navMeshAgent.pathPending && agent.navMeshAgent.remainingDistance < 0.5f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
            agent.navMeshAgent.SetDestination(currentTarget);
        }

        // Player detection
        foreach (var obj in agent.sensor.Objects)
        {
            if (obj.CompareTag("Player") && agent.sensor.IsInSight(obj))
            {
                Debug.Log("Chase player"); // ✅ Debug message
                agent.target = obj.transform;
                agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
                break;
            }
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
