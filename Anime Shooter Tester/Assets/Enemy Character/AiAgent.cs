using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AiSensor))]
[RequireComponent(typeof(Animator))]
public class AiAgent : MonoBehaviour
{
    public AiAgentConfig config;
    public AiStateMachine stateMachine;
    public NavMeshAgent navMeshAgent;
    public AiSensor sensor;
    public Animator animator;

    public AiStateId initialState = AiStateId.Patrol;
    public Vector3 patrolPointOffsetA = new Vector3(-5, 0, 0);
    public Vector3 patrolPointOffsetB = new Vector3(5, 0, 0);

    [HideInInspector]
    public Transform target;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        sensor = GetComponent<AiSensor>();
        animator = GetComponent<Animator>();

        stateMachine = new AiStateMachine(this);

        Vector3 patrolPointA = transform.position + patrolPointOffsetA;
        Vector3 patrolPointB = transform.position + patrolPointOffsetB;

        stateMachine.RegisterState(new AiPatrolState(patrolPointA, patrolPointB));
        stateMachine.RegisterState(new AiChasePlayerState());

        stateMachine.ChangeState(initialState);
    }

    void Update()
    {
        stateMachine.Update();

        // Keep enemy at ground level
        Vector3 pos = transform.position;
        pos.y = 0f;
        transform.position = pos;
    }
}
