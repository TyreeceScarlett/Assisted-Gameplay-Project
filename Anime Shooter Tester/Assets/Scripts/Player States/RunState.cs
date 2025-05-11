using UnityEngine;

public class RunState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Running", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        // Set running speed depending on direction
        movement.moveSpeed = movement.VInput < 0 ? movement.runBackSpeed : movement.runSpeed;
    }

    public override void ExitState(MovementStateManager movement)
    {
        movement.anim.SetBool("Running", false);
    }
}