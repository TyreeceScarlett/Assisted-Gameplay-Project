using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour
{
    public AiStateMachine stateMachine;
    public AiStateId initialState;
    public NavMeshAgent navMeshAgent;
    public AiAgentConfig config;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        stateMachine = new AiStateMachine(this);

        // Register all required states
        stateMachine.RegisterState(new AiChasePlayerState());

        // Enter the initial state
        stateMachine.ChangeState(initialState);
    }

    void Update()
    {
        stateMachine.Update();
    }
}
