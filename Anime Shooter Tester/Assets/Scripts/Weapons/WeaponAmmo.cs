using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    public int clipSize;
    public int extraAmmo;
    [HideInInspector] public int currentAmmo;

    public AudioClip magInSound;
    public AudioClip magOutSound;
    public AudioClip releaseSlideSound;

    private bool infiniteAmmo = false;
    private int defaultExtraAmmo = 300;

    void Start()
    {
        currentAmmo = clipSize;
        extraAmmo = defaultExtraAmmo;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
            infiniteAmmo = !infiniteAmmo;
            extraAmmo = infiniteAmmo ? int.MaxValue : defaultExtraAmmo;
            Debug.Log("Infinite Ammo: " + infiniteAmmo);
        }
    }

    public void Reload()
    {
        if (infiniteAmmo)
        {
            int ammoToReload = clipSize - currentAmmo;
            currentAmmo += ammoToReload;
        }
        else if (extraAmmo >= clipSize)
        {
            int ammoToReload = clipSize - currentAmmo;
            extraAmmo -= ammoToReload;
            currentAmmo += ammoToReload;
        }
        else if (extraAmmo > 0)
        {
            if (extraAmmo + currentAmmo > clipSize)
            {
                int leftOverAmmo = extraAmmo + currentAmmo - clipSize;
                extraAmmo = leftOverAmmo;
                currentAmmo = clipSize;
            }
            else
            {
                currentAmmo += extraAmmo;
                extraAmmo = 0;
            }
        }
    }
}
