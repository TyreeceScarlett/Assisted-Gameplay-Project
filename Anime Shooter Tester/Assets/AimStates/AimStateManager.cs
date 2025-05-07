using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AimStateManager : MonoBehaviour
{
    AimBaseState currentState;
    public HipFireState Hip = new HipFireState();
    public AimState Aim = new AimState();

    [SerializeField] float mouseSense = 1;
    [SerializeField] Transform camFollowPos;
    float xAxis, yAxis;

    [HideInInspector] public Animator anim;
    [HideInInspector] public CinemachineVirtualCamera vCam;
    public float adsFov = 30f; // Lowered to make zoom obvious
    [HideInInspector] public float hipFov;
    [HideInInspector] public float currentFov;
    public float fovSmoothSpeed = 10f;

    [SerializeField] Transform aimpos;
    [SerializeField] float aimSmoothSpeed = 20f;
    [SerializeField] LayerMask aimMask;

    [HideInInspector] public MovementStateManager movementManager;  // Reference to the MovementStateManager

    void Start()
    {
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        hipFov = vCam.m_Lens.FieldOfView; // Default camera FOV
        currentFov = hipFov; // Start with hipfire FOV
        anim = GetComponentInChildren<Animator>();
        movementManager = GetComponent<MovementStateManager>(); // Get the MovementStateManager component
        SwitchState(Hip);
    }

    void Update()
    {
        // Mouse look
        xAxis += Input.GetAxisRaw("Mouse X") * mouseSense;
        yAxis -= Input.GetAxisRaw("Mouse Y") * mouseSense;
        yAxis = Mathf.Clamp(yAxis, -80, 80);

        // Smooth FOV zoom
        vCam.m_Lens.FieldOfView = Mathf.Lerp(
            vCam.m_Lens.FieldOfView,
            currentFov,
            fovSmoothSpeed * Time.deltaTime
        );

        // Raycast aiming position
        Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCentre);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            // Snappy aim with smoothing (like Apex/Valorant)
            aimpos.position = Vector3.MoveTowards(
                aimpos.position,
                hit.point,
                aimSmoothSpeed * Time.deltaTime
            );
        }

        // Update state
        currentState.UpdateState(this);

        // Ensure that the movement animations are still playing when aiming (e.g., walking or idle)
        if (movementManager != null)
        {
            // Set animator values based on movement input
            anim.SetFloat("hzInput", movementManager.HzInput); // Accessing the public property HzInput
            anim.SetFloat("vInput", movementManager.VInput);   // Accessing the public property VInput
        }
    }

    void LateUpdate()
    {
        // Apply mouse look rotation
        camFollowPos.localEulerAngles = new Vector3(
            yAxis,
            camFollowPos.localEulerAngles.y,
            camFollowPos.localEulerAngles.z
        );

        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            xAxis,
            transform.eulerAngles.z
        );
    }

    public void SwitchState(AimBaseState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState(this);
        }

        currentState = newState;
        currentState.EnterState(this);
    }
}
