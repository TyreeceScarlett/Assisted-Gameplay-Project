using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AimStateManager : MonoBehaviour
{
    public AimBaseState currentState;
    public HipFireState Hip = new HipFireState();
    public AimState Aim = new AimState();

    [SerializeField] float mouseSense = 1;
    [SerializeField] Transform camFollowPos;
    float xAxis, yAxis;

    [HideInInspector] public Animator anim;
    [HideInInspector] public CinemachineVirtualCamera vCam;
    public float adsFov = 30f; // ✅ Lowered to make zoom obvious
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

    // ✅ New fields for proper shoulder swapping
    [SerializeField] float shoulderOffset = 0.5f;
    bool isRightShoulder = true;

    // Start is called before the first frame update
    void Start()
    {
        moving = GetComponent<MovementStateManager>();
        xFollowPos = camFollowPos.localPosition.x;
        ogYPos = camFollowPos.localPosition.y;
        yFollowPos = ogYPos;
        vCam = GetComponentInChildren<CinemachineVirtualCamera>();
        hipFov = vCam.m_Lens.FieldOfView; // default camera FOV
        currentFov = hipFov; // start with hipfire FOV
        anim = GetComponent<Animator>();
        SwitchState(Hip);
    }

    //update is called once per frame
    void Update()
    {
        // Mouse look
        xAxis += Input.GetAxisRaw("Mouse X") * mouseSense;
        yAxis -= Input.GetAxisRaw("Mouse Y") * mouseSense;
        yAxis = Mathf.Clamp(yAxis, -80, 80);

        // ✅ Smooth FOV zoom
        vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);

        // Raycast aiming position
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

        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            xAxis,
            transform.eulerAngles.z
        );
    }

    public void SwitchState(AimBaseState newState)
    {
        currentState = newState;
        currentState.EnterState(this);
    }

    void MoveCamera()
    {
        // ✅ Proper shoulder swapping using fixed offset
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            isRightShoulder = !isRightShoulder;

        xFollowPos = isRightShoulder ? shoulderOffset : -shoulderOffset;

        // Adjust camera height when crouching
        if (moving.currentState == moving.crouch) yFollowPos = crouchCamHeight;
        else yFollowPos = ogYPos;

        Vector3 newFollowPos = new Vector3(xFollowPos, yFollowPos, camFollowPos.localPosition.z);
        camFollowPos.localPosition = Vector3.Lerp(camFollowPos.localPosition, newFollowPos, shoulderSwapSpeed * Time.deltaTime);
    }
}
