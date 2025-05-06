using UnityEngine;

public class HipFireState : AimBaseState
{
    public override void EnterState(AimStateManager aim)
    {
        Debug.Log("Entered Hip Fire State");
        aim.anim.SetBool("Aiming", false);
        aim.currentFov = aim.hipFov; // ✅ Switch back to hip FOV
    }

    public override void UpdateState(AimStateManager aim)
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            aim.SwitchState(aim.Aim);
        }
    }
}