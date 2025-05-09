using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionStateManager : MonoBehaviour
{
    ActionBaseState currentState;

    public ReloadState Reload = new ReloadState();
    public DefaultState Default = new DefaultState();

    public GameObject currentWeapon;
    [HideInInspector]public WeaponAmmo ammo;

    [HideInInspector] public Animator anim;

    public MultiAimConstraint rHandAim;
    public TwoBoneIKConstraint lhandIK;

    // Start is called before the first frame update
    void Start()
    {
        SwitchState(Default);
        ammo = currentWeapon.GetComponent<WeaponAmmo>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(ActionBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public void WeaponReloaded()
    {
        ammo.Reload();
        SwitchState(Default);
    }
}
