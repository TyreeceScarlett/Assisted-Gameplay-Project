using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Animations.Rigging;

public class AimStateManager : MonoBehaviour
{
    public AimBaseState currentState;
    public HipFireState Hip = new HipFireState();
    [HideInInspector] public AimState Aim = new AimState();

    [SerializeField] float mouseSense = 1;
    [SerializeField] Transform camFollowPos;
    float xAxis, yAxis;

    [HideInInspector] public Animator anim;

    MultiAimConstraint[] multiAims;
    WeightedTransform aimPosweightedTransform;

    [HideInInspector] public CinemachineVirtualCamera vCam;
    public float adsFov = 30f;
    [HideInInspector] public float hipFov;
    [HideInInspector] public float currentFov;
    public float fovSmoothSpeed = 10f;

    public Transform aimpos;
    [HideInInspector] public Vector3 actualAimPos;
    [SerializeField] float aimSmoothSpeed = 20f;
    [SerializeField] LayerMask aimMask;

    float xFollowPos;
    float yFollowPos, ogYPos;
    [SerializeField] float crouchCamHeight = 0.6f;
    [SerializeField] float shoulderSwapSpeed = 10;
    MovementStateManager moving;

    [SerializeField] float shoulderOffset = 0.5f;
    bool isRightShoulder = true;

    // New fields for aim assist rotation override
    [HideInInspector] public bool overrideAimRotation = false;
    [HideInInspector] public Vector3 overrideLookPos;

    private void Awake()
    {
        // Create aim position object
        aimpos = new GameObject("AimPosition").transform;

        // Initialize WeightedTransform
        aimPosweightedTransform = new WeightedTransform
        {
            transform = aimpos,
            weight = 1f
        };

        // Get all MultiAimConstraints in children
        multiAims = GetComponentsInChildren<MultiAimConstraint>();

        // Assign aim position to all constraints
        foreach (MultiAimConstraint constraint in multiAims)
        {
            var data = constraint.data.sourceObjects;
            data.Clear();
            data.Add(aimPosweightedTransform);
            constraint.data.sourceObjects = data;
        }
    }

    void Start()
    {
        moving = GetComponent<MovementStateManager>();
        xFollowPos = camFollowPos.localPosition.x;
        ogYPos = camFollowPos.localPosition.y;
        yFollowPos = ogYPos;
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        hipFov = vCam.m_Lens.FieldOfView;
        currentFov = hipFov;
        anim = GetComponent<Animator>();
        SwitchState(Hip);
    }

    void Update()
    {
        // Mouse look
        xAxis += Input.GetAxisRaw("Mouse X") * mouseSense;
        yAxis -= Input.GetAxisRaw("Mouse Y") * mouseSense;
        yAxis = Mathf.Clamp(yAxis, -80, 80);

        // Smooth FOV transition
        vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);

        // Raycast to get aim position
        Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCentre);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            aimpos.position = Vector3.Lerp(aimpos.position, hit.point, aimSmoothSpeed * Time.deltaTime);
        }

        MoveCamera();
        currentState.UpdateState(this);
    }

    void LateUpdate()
    {
        camFollowPos.localEulerAngles = new Vector3(
            yAxis,
            camFollowPos.localEulerAngles.y,
            camFollowPos.localEulerAngles.z
        );

        if (overrideAimRotation)
        {
            Vector3 lookDir = overrideLookPos - transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
                xAxis = transform.eulerAngles.y; // Update mouse axis to match assist
            }
        }
        else
        {
            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                xAxis,
                transform.eulerAngles.z
            );
        }
    }

    public void SwitchState(AimBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    void MoveCamera()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            isRightShoulder = !isRightShoulder;

        xFollowPos = isRightShoulder ? shoulderOffset : -shoulderOffset;

        yFollowPos = (moving.currentState == moving.crouch) ? crouchCamHeight : ogYPos;

        Vector3 newFollowPos = new Vector3(xFollowPos, yFollowPos, camFollowPos.localPosition.z);
        camFollowPos.localPosition = Vector3.Lerp(camFollowPos.localPosition, newFollowPos, shoulderSwapSpeed * Time.deltaTime);
    }
}
