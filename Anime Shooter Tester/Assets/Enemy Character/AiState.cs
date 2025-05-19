using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum representing AI state identifiers
public enum AiStateId
{
    Patrol,
    ChasePlayer
}

// Interface for AI states
public interface AiState
{
    AiStateId GetId();
    void Enter(AiAgent agent);
    void Update(AiAgent agent);
    void Exit(AiAgent agent);
}