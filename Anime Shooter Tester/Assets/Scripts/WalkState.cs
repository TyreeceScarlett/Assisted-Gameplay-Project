using UnityEngine;

public class WalkState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Walking", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        movement.moveSpeed = movement.VInput < 0 ? movement.walkBackSpeed : movement.walkSpeed;
    }

    public override void ExitState(MovementStateManager movement)
    {
        movement.anim.SetBool("Walking", false);
    }
}