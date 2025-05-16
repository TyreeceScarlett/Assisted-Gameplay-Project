using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionStateManager : MonoBehaviour
{
    [HideInInspector] public ActionBaseState currentState;

    // States
    public ReloadState Reload = new ReloadState();
    public DefaultState Default = new DefaultState();
    public SwapState Swap = new SwapState();

    // Current Weapon Info
    [HideInInspector] public GameObject currentWeapon;
    [HideInInspector] public WeaponAmmo ammo;
    AudioSource audioSource;

    [HideInInspector] public Animator anim;

    // Rig Constraints
    public MultiAimConstraint rHandAim;
    public TwoBoneIKConstraint lHandIK;
    public RigBuilder rigBuilder; // ✅ New: reference to RigBuilder

    void Start()
    {
        // Start in Default State
        anim = GetComponent<Animator>();
        if (rigBuilder == null)
            rigBuilder = GetComponent<RigBuilder>();

        SwitchState(Default);
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
        else
        {
            Debug.LogError("Current state is null in ActionStateManager!");
        }
    }

    public void SwitchState(ActionBaseState newState)
    {
        if (newState == null)
        {
            Debug.LogError("Trying to switch to a null state!");
            return;
        }

        if (currentState != null)
            currentState.ExitState(this); // ✅ Ensure current state exits cleanly

        currentState = newState;
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
        if (audioSource != null && ammo != null && ammo.magOutSound != null)
            audioSource.PlayOneShot(ammo.magOutSound);
    }

    public void MagIn()
    {
        if (audioSource != null && ammo != null && ammo.magInSound != null)
            audioSource.PlayOneShot(ammo.magInSound);
    }

    public void ReleaseSlide()
    {
        if (audioSource != null && ammo != null && ammo.releaseSlideSound != null)
            audioSource.PlayOneShot(ammo.releaseSlideSound);
    }

    public bool IsReloading()
    {
        return currentState == Reload;
    }

    public void SetWeapon(WeaponManager weapon)
    {
        if (weapon == null)
        {
            Debug.LogError("WeaponManager reference is null!");
            return;
        }

        currentWeapon = weapon.gameObject;
        audioSource = weapon.audioSource;
        ammo = weapon.ammo;
    }
}
