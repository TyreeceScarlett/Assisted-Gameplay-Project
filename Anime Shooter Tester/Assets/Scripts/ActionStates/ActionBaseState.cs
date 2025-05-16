public abstract class ActionBaseState
{
    public abstract void EnterState(ActionStateManager manager);
    public abstract void UpdateState(ActionStateManager manager);
    public virtual void ExitState(ActionStateManager manager) { }
}
