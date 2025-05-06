using UnityEngine;

public class HipFireState : AimBaseState
{
    public override void EnterState(AimStateManager aim)
    {
        Debug.Log("Entered Hip Fire State");
        aim.anim.SetBool("Aiming", false); // Trigger "hipfire" animation
        aim.currentFov = aim.hipFov;
    }

    public override void UpdateState(AimStateManager aim)
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            aim.SwitchState(aim.Aim);
        }
    }
}