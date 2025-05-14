using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AiAgent : MonoBehaviour
{
    public AiAgentConfig config;
    public AiStateMachine stateMachine;
    public NavMeshAgent navMeshAgent;
    public AiStateId initialState;
    public AiSensor sensor;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        stateMachine = new AiStateMachine(this);
        sensor = GetComponent<AiSensor>();

        // Register states
        stateMachine.RegisterState(new AiChasePlayerState());

        // Set initial state
        stateMachine.ChangeState(initialState);
    }

    void Update()
    {
        stateMachine.Update();
    }
}
