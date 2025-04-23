using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AimStateManager : MonoBehaviour
{
    [Header("Cinemachine Axis Settings")]
    public Cinemachine.AxisState xAxis = new Cinemachine.AxisState();
    public Cinemachine.AxisState yAxis = new Cinemachine.AxisState();

    [Header("References")]
    public Transform camFollowPos; // Assign this in the Inspector

    void Start()
    {
        // Set the input axis names here
        xAxis.m_InputAxisName = "Mouse X";
        yAxis.m_InputAxisName = "Mouse Y";
    }

    void Update()
    {
        xAxis.Update(Time.deltaTime);
        yAxis.Update(Time.deltaTime);
    }

    void LateUpdate()
    {
        // Apply vertical rotation to the follow target
        camFollowPos.localEulerAngles = new Vector3(
            yAxis.Value,
            camFollowPos.localEulerAngles.y,
            camFollowPos.localEulerAngles.z
        );

        // Apply horizontal rotation to the character
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            xAxis.Value,
            transform.eulerAngles.z
        );
    }
}