using UnityEngine;

public class AimState : AimBaseState
{
    public override void EnterState(AimStateManager aim)
    {
        Debug.Log("Entered Aim State");
        aim.anim.SetBool("Aiming", true);
        aim.currentFov = aim.adsFov; // ✅ Switch to ADS FOV
    }

    public override void UpdateState(AimStateManager aim)
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            aim.SwitchState(aim.Hip);
        }
    }
}