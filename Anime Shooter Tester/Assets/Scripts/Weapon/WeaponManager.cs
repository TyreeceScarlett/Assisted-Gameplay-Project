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
    AimStateManager aim;

    [SerializeField] AudioClip gunShot;
    AudioSource audioSource;
    WeaponAmmo ammo;

    // Start is called before this first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        aim = GetComponentInParent<AimStateManager>();
        ammo = GetComponent<WeaponAmmo>();
        fireRateTimer = fireRate;

        if (aim == null)
        {
            Debug.LogError("AimStateManager not found in parent!");
        }
    }

    void Update()
    {
        if (ShouldFire())
        {
            Fire();
            Debug.Log(ammo.currentAmmo);
        }
    }

    bool ShouldFire()
    {
        fireRateTimer += Time.deltaTime;
        if (fireRateTimer < fireRate) return false;
        if (ammo.currentAmmo == 0) return false;
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
            audioSource.PlayOneShot(gunShot);
            ammo.currentAmmo--;
            for (int i = 0; i < bulletsPerShot; i++)
            {
                GameObject currentBullet = Instantiate(bullet, barrelPos.position, barrelPos.rotation);
                Rigidbody rb = currentBullet.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddForce(barrelPos.forward * bulletVelocity, ForceMode.Impulse);
                }
                else
                {
                    Debug.LogWarning("Bullet prefab is missing a Rigidbody component!");
                }
            }
        }
    }
}