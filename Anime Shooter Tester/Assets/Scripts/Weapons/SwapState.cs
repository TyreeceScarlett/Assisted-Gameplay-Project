public class SwapState : ActionBaseState
{
    public override void EnterState(ActionStateManager manager)
    {
        // Disable rig builder to prevent IK interference during swap
        if (manager.rigBuilder != null)
            manager.rigBuilder.enabled = false;

        // ✅ Use the correct parameter to play the swap animation
        manager.anim.SetTrigger("SwapWeapon");
    }

    public override void UpdateState(ActionStateManager manager)
    {
        // Optional: Add logic to transition back to Default state when animation ends
    }

    public override void ExitState(ActionStateManager manager)
    {
        // Re-enable rig builder after swap
        if (manager.rigBuilder != null)
            manager.rigBuilder.enabled = true;
    }
}
