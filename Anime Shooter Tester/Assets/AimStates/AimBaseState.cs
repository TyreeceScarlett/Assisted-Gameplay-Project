using UnityEngine;

public abstract class AimBaseState
{
    public abstract void EnterState(AimStateManager aim);
    public abstract void UpdateState(AimStateManager aim);
    public abstract void ExitState(AimStateManager aim);
}