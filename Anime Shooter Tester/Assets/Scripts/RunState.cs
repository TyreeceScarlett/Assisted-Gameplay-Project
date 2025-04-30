using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Running", true);
        movement.moveSpeed = 5.0f; // Adjust the running speed
    }

    public override void UpdateState(MovementStateManager movement)
    {
        // Transition to walk if Shift is released
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            movement.SwitchState(movement.walk);
            return;
        }

        // Transition to idle if no movement input
        if (movement.dir.magnitude < 0.1f)
        {
            movement.SwitchState(movement.Idle);
            return;
        }

        // Optionally, transition back to crouch if 'C' is pressed
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            movement.SwitchState(movement.crouch);
            return;
        }
    }

    public override void ExitState(MovementStateManager movement)
    {
        movement.anim.SetBool("Running", false);
    }
}