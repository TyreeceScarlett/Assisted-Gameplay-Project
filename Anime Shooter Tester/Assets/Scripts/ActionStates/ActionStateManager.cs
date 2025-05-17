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

    //start is called before the first frame update
    void Start()
    {
        // Start in Default State
        SwitchState(Default);
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Update current active state
        if (currentState != null)
            currentState.UpdateState(this);
        else
            Debug.LogError("Current state is null in ActionStateManager!");
    }

    public void SwitchState(ActionBaseState state)
    {
        if (state == null)
        {
            Debug.LogError("Trying to switch to a null state!");
            return;
        }

        // Switch state and call EnterState
        currentState = state;
        currentState.EnterState(this);
    }

    public void WeaponReloaded()
    {
        // Ammo reload logic
        if (ammo != null)
            ammo.Reload();

        // Reset Rig weights
        if (rHandAim != null)
            rHandAim.weight = 0;

        if (lHandIK != null)
            lHandIK.weight = 0;

        // Back to default
        SwitchState(Default);
    }

    public void MagOut()
    {
        // Play mag out sound
        if (audioSource != null && ammo != null && ammo.magOutSound != null)
            audioSource.PlayOneShot(ammo.magOutSound);
    }

    public void MagIn()
    {
        // Play mag in sound
        if (audioSource != null && ammo != null && ammo.magInSound != null)
            audioSource.PlayOneShot(ammo.magInSound);
    }

    public void ReleaseSlide()
    {
        // Play slide release sound
        if (audioSource != null && ammo != null && ammo.releaseSlideSound != null)
            audioSource.PlayOneShot(ammo.releaseSlideSound);
    }

    // ✅ Safe check to avoid null crash in WeaponManager
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

        // Set current weapon refs
        currentWeapon = weapon.gameObject;
        audioSource = weapon.audioSource;
        ammo = weapon.ammo;
    }
}
