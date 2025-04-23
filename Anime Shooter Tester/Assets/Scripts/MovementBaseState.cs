public abstract class MovementBaseState
{
    // Abstract method for entering a state, requires a MovementStateManager parameter
    public abstract void EnterState(MovementStateManager movement);

    // Abstract method for updating a state, requires a MovementStateManager parameter
    public abstract void UpdateState(MovementStateManager movement);
}
