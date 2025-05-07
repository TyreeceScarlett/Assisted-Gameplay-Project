using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimState : AimBaseState
{
    public override void EnterState(AimStateManager aim)
    {
        // Switch to aiming FOV
        aim.currentFov = aim.adsFov;
        aim.anim.SetBool("isAiming", true);
    }

    public override void UpdateState(AimStateManager aim)
    {
        // If right mouse button released, switch back to hipfire
        if (Input.GetMouseButtonUp(1))
        {
            aim.SwitchState(aim.Hip);
        }
    }

    public override void ExitState(AimStateManager aim)
    {
        aim.anim.SetBool("isAiming", false);
    }
}