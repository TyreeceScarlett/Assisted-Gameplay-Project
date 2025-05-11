using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponClassManager : MonoBehaviour
{
    [SerializeField] TwoBoneIKConstraint leftandIK;
    public Transform recoilFollowPos;
    ActionStateManager actions;

    public void SetCurrentWeapon(WeaponManager weapon)
    {
        if (weapon == null)
        {
            Debug.LogError("WeaponManager is null when setting current weapon!");
            return;
        }

        if (actions == null)
            actions = GetComponent<ActionStateManager>();

        if (leftandIK != null)
        {
            leftandIK.data.target = weapon.leftHandTarget;
            leftandIK.data.hint = weapon.leftHandHint;
        }
        else
        {
            Debug.LogWarning("Left Hand IK Constraint not assigned!");
        }

        if (actions != null)
            actions.SetWeapon(weapon);
        else
            Debug.LogError("ActionStateManager missing on WeaponClassManager GameObject!");
    }
}
