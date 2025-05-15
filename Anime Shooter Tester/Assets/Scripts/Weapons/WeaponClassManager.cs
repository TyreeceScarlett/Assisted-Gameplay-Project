using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponClassManager : MonoBehaviour
{
    [SerializeField] TwoBoneIKConstraint leftandIK;
    public Transform recoilFollowPos;
    public WeaponManager[] weapons;
    int currentWeaponIndex;

    ActionStateManager actions;

    private void Awake()
    {
        actions = GetComponent<ActionStateManager>();
        currentWeaponIndex = 0;

        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].gameObject.SetActive(i == 0);
            }
            else
            {
                Debug.LogWarning($"Weapon slot {i} is null in WeaponClassManager!");
            }
        }

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

        StartCoroutine(SwapWeaponCoroutine(weapon));
    }

    private IEnumerator SwapWeaponCoroutine(WeaponManager weapon)
    {
        if (leftandIK != null)
            leftandIK.weight = 0f;

        if (actions != null)
            actions.SetWeapon(weapon);

        yield return new WaitForSeconds(0.5f);

        if (leftandIK != null)
        {
            leftandIK.data.target = weapon.leftHandTarget;
            leftandIK.data.hint = weapon.leftHandHint;
            leftandIK.weight = 1f;
        }
    }

    public void ChangeWeapon(float direction)
    {
        if (weapons.Length == 0)
        {
            Debug.LogWarning("No weapons assigned in WeaponClassManager!");
            return;
        }

        if (weapons[currentWeaponIndex] != null)
            weapons[currentWeaponIndex].gameObject.SetActive(false);

        if (direction < 0)
        {
            currentWeaponIndex = (currentWeaponIndex == 0) ? weapons.Length - 1 : currentWeaponIndex - 1;
        }
        else
        {
            currentWeaponIndex = (currentWeaponIndex == weapons.Length - 1) ? 0 : currentWeaponIndex + 1;
        }

        if (weapons[currentWeaponIndex] != null)
        {
            weapons[currentWeaponIndex].gameObject.SetActive(true);
            SetCurrentWeapon(weapons[currentWeaponIndex]);
            Debug.Log($"Switched to weapon: {weapons[currentWeaponIndex].name}");
        }
        else
        {
            Debug.LogWarning($"Weapon at index {currentWeaponIndex} is null!");
        }
    }

    public void WeaponPutAway(float scrollDirection)
    {
        ChangeWeapon(scrollDirection);
    }

    public void WeaponPullOut()
    {
        if (actions != null)
        {
            actions.SwitchState(actions.Default);
        }
    }
}
