using UnityEngine;

public class IdleState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Walking", false);
        movement.anim.SetBool("Running", false);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        movement.moveSpeed = 0f;
    }

    public override void ExitState(MovementStateManager movement)
    {
        // Nothing needed here
    }
}