public class RunState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        // Blend tree handles animation
    }

    public override void UpdateState(MovementStateManager movement)
    {
        movement.moveSpeed = movement.VInput < 0 ? movement.runBackSpeed : movement.runSpeed;
    }

    public override void ExitState(MovementStateManager movement)
    {
        // Nothing needed
    }
}