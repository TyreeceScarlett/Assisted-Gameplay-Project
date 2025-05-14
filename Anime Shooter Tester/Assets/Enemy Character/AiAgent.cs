using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AiSensor))] // Ensure AiSensor is required
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
        sensor = GetComponent<AiSensor>();

        stateMachine = new AiStateMachine(this);

        // Register states
        stateMachine.RegisterState(new AiChasePlayerState());

        // Set initial state
        stateMachine.ChangeState(initialState);
    }

    void Update()
    {
        stateMachine.Update();

        // Keep enemy at ground level (Y = 0)
        var pos = transform.position;
        pos.y = 0f;
        transform.position = pos;
    }
}
