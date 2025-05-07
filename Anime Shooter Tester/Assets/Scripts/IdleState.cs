using UnityEngine;

public class IdleState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        // No bools needed now — blend tree handles movement animations
    }

    public override void UpdateState(MovementStateManager movement)
    {
        movement.moveSpeed = 0f;
    }

    public override void ExitState(MovementStateManager movement)
    {
        // Nothing needed
    }
}