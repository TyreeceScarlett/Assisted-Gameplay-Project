using UnityEngine;

public class SwapState : ActionBaseState
{
    float swapDuration = 6f; // Adjust based on animation length
    float timer;

    public override void EnterState(ActionStateManager actions)
    {
        actions.anim.SetTrigger("SwapWeapon");
        actions.lHandIK.weight = 0;
        actions.rHandAim.weight = 0;
        timer = 0;
    }

    public override void UpdateState(ActionStateManager actions)
    {
        timer += Time.deltaTime;

        if (timer >= swapDuration)
        {
            actions.rHandAim.weight = 1; // Restore aim weight after swap
            actions.SwitchState(actions.Default);
        }
    }
}
