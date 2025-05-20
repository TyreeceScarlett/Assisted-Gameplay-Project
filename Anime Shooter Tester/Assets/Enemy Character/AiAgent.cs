using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AiSensor))]
[RequireComponent(typeof(Animator))]
public class AiAgent : MonoBehaviour
{
    public AiAgentConfig config;

    [HideInInspector] public AiStateMachine stateMachine;
    [HideInInspector] public NavMeshAgent navMeshAgent;
    [HideInInspector] public AiSensor sensor;
    [HideInInspector] public Animator animator;

    public AiStateId initialState = AiStateId.Patrol;

    public Vector3 patrolCenter;
    public float patrolRadius = 5f;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        sensor = GetComponent<AiSensor>();
        animator = GetComponent<Animator>();

        patrolCenter = transform.position;

        stateMachine = new AiStateMachine(this);

        stateMachine.RegisterState(new AiPatrolState());
        stateMachine.RegisterState(new AiChasePlayerState());

        stateMachine.ChangeState(initialState);
    }

    void Update()
    {
        stateMachine.Update();

        // Keep grounded
        Vector3 pos = transform.position;
        pos.y = 0f;
        transform.position = pos;

        UpdateAnimator();
    }

    public Vector3 GetRandomPatrolPoint()
    {
        Vector2 randomPoint = Random.insideUnitCircle * patrolRadius;
        return patrolCenter + new Vector3(randomPoint.x, 0f, randomPoint.y);
    }

    private void UpdateAnimator()
    {
        // Instead of velocity, use fixed input values to animate walking forward
        animator.SetFloat("vInput", 1f);    // forward movement input
        animator.SetFloat("hzInput", 0f);   // horizontal input zero for no strafing
    }
}
