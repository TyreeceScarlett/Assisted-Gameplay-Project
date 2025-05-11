using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        if (ShouldFire())
        {
            Fire();
            Debug.Log(ammo?.currentAmmo);
        }

        // Always fade light every frame
        if (muzzleFlashLight != null)
            muzzleFlashLight.intensity = Mathf.Lerp(muzzleFlashLight.intensity, 0, lightReturnSpeed * Time.deltaTime);
    }

    bool ShouldFire()
    {
        fireRateTimer += Time.deltaTime;
        if (fireRateTimer < fireRate) return false;
        if (ammo == null || ammo.currentAmmo == 0) return false;
        if (actions != null && actions.IsReloading()) return false;
        if (actions.currentState == actions.Swap) return false;
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
                bulletScript.dir = barrelPos.transform.forward;

                Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.useGravity = false; // Fly straight
                    rb.drag = 0f;          // No slow down
                    rb.velocity = barrelPos.forward * bulletVelocity;
                }
                else
                {
                    Debug.LogWarning("Bullet prefab is missing a Rigidbody component!");
                }

                Destroy(currentBullet, 5f); // Auto cleanup bullet
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
}
