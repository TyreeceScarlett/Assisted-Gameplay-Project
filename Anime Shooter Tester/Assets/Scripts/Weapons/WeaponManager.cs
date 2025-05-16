using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WeaponManager : MonoBehaviour
{
    [Header("Fire Rate")]
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private bool semiAuto = true;
    private float fireRateTimer;

    [Header("Bullet Properties")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform barrelPos;
    [SerializeField] private float bulletVelocity = 20f;
    [SerializeField] private int bulletsPerShot = 1;
    public float damage = 20;

    AimStateManager aim;
    [SerializeField] AudioClip gunShot;
    [HideInInspector] public AudioSource audioSource;
    [HideInInspector] public WeaponAmmo ammo;
    WeaponBloom bloom;
    ActionStateManager actions;
    WeaponRecoil recoil;

    Light muzzleFlashLight;
    ParticleSystem muzzleFlashParticles;
    float lightIntensity;
    [SerializeField] float lightReturnSpeed = 20;

    public float enemyKickbackForce = 100;

    public Transform leftHandTarget, leftHandHint;
    WeaponClassManager weaponClass;

    [Header("UI Blocking")]
    [Tooltip("Add any canvases that should block firing when the mouse is over them")]
    public List<Transform> blockedCanvases = new List<Transform>(); // 🔹 Drag multiple canvases here

    void Start()
    {
        aim = GetComponentInParent<AimStateManager>();
        bloom = GetComponent<WeaponBloom>();
        actions = GetComponentInParent<ActionStateManager>();

        muzzleFlashLight = GetComponentInChildren<Light>();
        if (muzzleFlashLight != null)
        {
            lightIntensity = muzzleFlashLight.intensity;
            muzzleFlashLight.intensity = 0f;
        }
        else
        {
            Debug.LogWarning("Muzzle flash light not found!");
        }

        muzzleFlashParticles = GetComponentInChildren<ParticleSystem>();
        fireRateTimer = fireRate;

        if (aim == null)
        {
            Debug.LogError("AimStateManager not found in parent!");
        }
    }

    private void OnEnable()
    {
        if (weaponClass == null)
        {
            weaponClass = GetComponentInParent<WeaponClassManager>();
            ammo = GetComponent<WeaponAmmo>();
            if (ammo == null) Debug.LogError("WeaponAmmo script missing on weapon!");

            audioSource = GetComponent<AudioSource>();
            recoil = GetComponent<WeaponRecoil>();

            if (recoil != null && weaponClass != null)
                recoil.recoilFollowPos = weaponClass.recoilFollowPos;
        }

        if (weaponClass != null)
            weaponClass.SetCurrentWeapon(this);
    }

    void Update()
    {
        if (ShouldFire())
        {
            Fire();
            Debug.Log(ammo?.currentAmmo);
        }

        if (muzzleFlashLight != null)
            muzzleFlashLight.intensity = Mathf.Lerp(muzzleFlashLight.intensity, 0, lightReturnSpeed * Time.deltaTime);
    }

    bool ShouldFire()
    {
        fireRateTimer += Time.deltaTime;
        if (fireRateTimer < fireRate) return false;
        if (ammo == null || ammo.currentAmmo == 0) return false;
        if (actions != null && actions.IsReloading()) return false;
        if(actions.currentState == actions.Swap) return false;
        if (IsPointerOverBlockedUI()) return false; // 🔹 Updated method

        if (semiAuto && Input.GetKeyDown(KeyCode.Mouse0)) return true;
        if (!semiAuto && Input.GetKey(KeyCode.Mouse0)) return true;

        return false;
    }

    void Fire()
    {
        fireRateTimer = 0f;

        if (aim != null && aim.aimpos != null)
        {
            barrelPos.LookAt(aim.aimpos.position);
            barrelPos.localEulerAngles = bloom.BloomAngle(barrelPos);

            if (audioSource != null)
                audioSource.PlayOneShot(gunShot);

            if (recoil != null)
                recoil.TriggerRecoil();

            if (ammo != null)
                ammo.currentAmmo--;

            for (int i = 0; i < bulletsPerShot; i++)
            {
                GameObject currentBullet = Instantiate(bullet, barrelPos.position, barrelPos.rotation);

                Bullet bulletScript = currentBullet.GetComponent<Bullet>();
                bulletScript.weapon = this;
                bulletScript.dir = barrelPos.forward;

                Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false;
                    rb.drag = 0f;
                    rb.velocity = barrelPos.forward * bulletVelocity;
                }
                else
                {
                    Debug.LogWarning("Bullet prefab is missing a Rigidbody component!");
                }

                Destroy(currentBullet, 5f);
            }

            TriggerMuzzleFlash();
        }
    }

    void TriggerMuzzleFlash()
    {
        if (muzzleFlashParticles != null)
            muzzleFlashParticles.Play();

        if (muzzleFlashLight != null)
            muzzleFlashLight.intensity = lightIntensity;
    }

    bool IsPointerOverBlockedUI()
    {
        if (blockedCanvases == null || blockedCanvases.Count == 0)
            return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            foreach (var canvasTransform in blockedCanvases)
            {
                if (canvasTransform != null && result.gameObject.transform.IsChildOf(canvasTransform))
                    return true;
            }
        }

        return false;
    }
}
