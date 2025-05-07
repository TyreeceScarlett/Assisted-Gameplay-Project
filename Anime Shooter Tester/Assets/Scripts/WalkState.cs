using UnityEngine;

public class WalkState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        // Blend tree will handle walking animation
    }

    public override void UpdateState(MovementStateManager movement)
    {
        movement.moveSpeed = movement.VInput < 0 ? movement.walkBackSpeed : movement.walkSpeed;
    }

    public override void ExitState(MovementStateManager movement)
    {
        // Nothing needed
    }
}