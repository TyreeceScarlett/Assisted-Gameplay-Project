using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionStateManager : MonoBehaviour
{
    public ActionBaseState currentState;

    public ReloadState Reload = new ReloadState();
    public DefaultState Default = new DefaultState();

    public GameObject currentWeapon;
    [HideInInspector] public WeaponAmmo ammo;
    AudioSource audioSource;

    [HideInInspector] public Animator anim;

    public MultiAimConstraint rHandAim;
    public TwoBoneIKConstraint lHandIK;

    void Start()
    {
        // Auto-assign rig constraints if not set in inspector
        if (rHandAim == null)
            rHandAim = GetComponentInChildren<MultiAimConstraint>();

        if (lHandIK == null)
            lHandIK = GetComponentInChildren<TwoBoneIKConstraint>();

        SwitchState(Default);

        if (currentWeapon != null)
        {
            ammo = currentWeapon.GetComponent<WeaponAmmo>();
            audioSource = currentWeapon.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogError("Current weapon is not assigned in ActionStateManager!");
        }

        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (currentState != null)
            currentState.UpdateState(this);
        else
            Debug.LogError("Current state is null in ActionStateManager!");
    }

    public void SwitchState(ActionBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }

    public void WeaponReloaded()
    {
        if (ammo != null)
            ammo.Reload();

        if (rHandAim != null)
            rHandAim.weight = 1;

        if (lHandIK != null)
            lHandIK.weight = 1;

        SwitchState(Default);
    }

    public void MagOut()
    {
        if (audioSource != null && ammo != null)
            audioSource.PlayOneShot(ammo.magOutSound);
    }

    public void MagIn()
    {
        if (audioSource != null && ammo != null)
            audioSource.PlayOneShot(ammo.magInSound);
    }

    public void ReleaseSlide()
    {
        if (audioSource != null && ammo != null)
            audioSource.PlayOneShot(ammo.releaseSlideSound);
    }

    // ✅ Safe check to avoid null crash in WeaponManager
    public bool IsReloading()
    {
        return currentState == Reload;
    }
}
