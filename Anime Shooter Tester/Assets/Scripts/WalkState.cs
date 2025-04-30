using UnityEngine;

public class WalkState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Walking", true);
        movement.moveSpeed = 2.5f; // Set walking speed
    }

    public override void UpdateState(MovementStateManager movement)
    {
        // Check for crouch input
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            movement.SwitchState(movement.crouch);
            return;
        }

        // Switch to run if shift is held
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movement.SwitchState(movement.run);
            return;
        }

        // Adjust walking speed based on vInput before switching state to idle
        if (movement.VInput < 0)  // Use the public property VInput
        {
            movement.moveSpeed = movement.walkBackSpeed;  // Use walkBackSpeed when moving backwards
        }
        else
        {
            movement.moveSpeed = movement.walkSpeed;  // Use walkSpeed for normal walking
        }

        // Switch to idle if no movement
        if (movement.dir.magnitude < 0.1f)
        {
            movement.SwitchState(movement.Idle);
            return;
        }
    }

    public override void ExitState(MovementStateManager movement)
    {
        movement.anim.SetBool("Walking", false);
    }
}