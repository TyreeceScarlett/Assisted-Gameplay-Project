using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] Transform recoilFollowPos;
    [SerializeField] float kickBackAmount = -1f;
    [SerializeField] float kickBackSpeed = 10f, returnSpeed = 20f;
    float currentRecoilPosition, finalRecoilPosition;

    void Update()
    {
        currentRecoilPosition = Mathf.Lerp(currentRecoilPosition, 0, returnSpeed * Time.deltaTime);
        finalRecoilPosition = Mathf.Lerp(finalRecoilPosition, currentRecoilPosition, kickBackSpeed * Time.deltaTime);
        recoilFollowPos.localPosition = new Vector3(0, 0, finalRecoilPosition);
    }

    public void TriggerRecoil() => currentRecoilPosition += kickBackAmount;
}
