using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponClassManager : MonoBehaviour
{
    [SerializeField] TwoBoneIKConstraint leftandIK;
    public Transform recoilFollowPos;
    ActionStateManager actions;

    public WeaponManager[] weapons;
    int currentWeaponIndex;

    private void Awake()
    {
        currentWeaponIndex = 0;

        // Activate first weapon, deactivate others
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                if (i == 0) weapons[i].gameObject.SetActive(true);
                else weapons[i].gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"Weapon slot {i} is null in WeaponClassManager!");
            }
        }

        // Set first weapon as current
        if (weapons.Length > 0 && weapons[0] != null)
            SetCurrentWeapon(weapons[0]);
    }

    public void SetCurrentWeapon(WeaponManager weapon)
    {
        if (weapon == null)
        {
            Debug.LogError("WeaponManager is null when setting current weapon!");
            return;
        }

        if (actions == null)
            actions = GetComponent<ActionStateManager>();

        // Update Left Hand IK targets
        if (leftandIK != null)
        {
            leftandIK.data.target = weapon.leftHandTarget;
            leftandIK.data.hint = weapon.leftHandHint;
        }
        else
        {
            Debug.LogWarning("Left Hand IK Constraint not assigned!");
        }

        // Set weapon in actions
        if (actions != null)
            actions.SetWeapon(weapon);
        else
            Debug.LogError("ActionStateManager missing on WeaponClassManager GameObject!");
    }

    public void ChangeWeapon(float direction)
    {
        if (weapons.Length == 0)
        {
            Debug.LogWarning("No weapons assigned in WeaponClassManager!");
            return;
        }

        // Deactivate current weapon
        weapons[currentWeaponIndex].gameObject.SetActive(false);

        // Change index based on scroll direction
        if (direction < 0)
        {
            if (currentWeaponIndex == 0) currentWeaponIndex = weapons.Length - 1;
            else currentWeaponIndex--;
        }
        else
        {
            if (currentWeaponIndex == weapons.Length - 1) currentWeaponIndex = 0;
            else currentWeaponIndex++;
        }

        // Activate new weapon
        weapons[currentWeaponIndex].gameObject.SetActive(true);

        // Set new weapon
        SetCurrentWeapon(weapons[currentWeaponIndex]);
    }

    public void WeaponPutAway()
    {
        ChangeWeapon(actions.Default.scrollDirection);
    }

    public void WeaponPullOut()
    {
        actions.SwitchState(actions.Default);
    }
}
