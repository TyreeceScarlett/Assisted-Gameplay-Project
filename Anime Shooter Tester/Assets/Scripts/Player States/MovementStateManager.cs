using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    [Header("Speed Settings")]
    public float walkSpeed = 3f;
    public float walkBackSpeed = 2f;
    public float runSpeed = 7f;
    public float runBackSpeed = 5f;
    public float crouchSpeed = 2f;
    public float crouchBackSpeed = 1f;

    [Header("State System")]
    public MovementBaseState currentState;
    public IdleState Idle = new IdleState();
    public WalkState walk = new WalkState();
    public CrouchingState crouch = new CrouchingState();
    public RunState run = new RunState();

    [Header("Movement and Gravity")]
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float groundYOffset = 0.2f;
    [SerializeField] LayerMask groundMask;
    [HideInInspector] public Vector3 dir;
    Vector3 velocity;
    Vector3 spherePos;

    [Header("References")]
    [HideInInspector] public Animator anim;
    CharacterController controller;

    [Header("State Info")]
    public bool isCrouching = false;
    public float moveSpeed = 3f;
    public float VInput => vInput;
    public bool IsRunning { get; private set; }
    public bool IsMoving => dir.magnitude > 0.1f;

    private float hzInput, vInput;

    public bool IsGrounded
    {
        get
        {
            spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
            return Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask);
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        if (controller != null)
            controller.center = new Vector3(0, controller.height / 2, 0);

        SwitchState(Idle);
    }

    void Update()
    {
        GetInputs();
        GetDirectionAndMove();
        ApplyGravity();

        anim.SetFloat("hzInput", hzInput);
        anim.SetFloat("vInput", vInput);

        currentState.UpdateState(this);
        HandleStateSwitching();
    }

    void GetInputs()
    {
        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        dir = transform.forward * vInput + transform.right * hzInput;
        IsRunning = Input.GetKey(KeyCode.LeftShift);

        // Toggle crouch on Left Ctrl
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;

            if (isCrouching)
                SwitchState(crouch);
            else
                SwitchState(IsMoving ? (IsRunning ? run : walk) : Idle);
        }
    }

    void GetDirectionAndMove()
    {
        controller.Move(dir.normalized * moveSpeed * Time.deltaTime);
    }

    void ApplyGravity()
    {
        if (IsGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleStateSwitching()
    {
        if (isCrouching) return;

        if (IsRunning && IsMoving)
        {
            if (currentState != run)
                SwitchState(run);
            return;
        }

        if (!IsRunning && IsMoving)
        {
            if (currentState != walk)
                SwitchState(walk);
            return;
        }

        if (!IsMoving && currentState != Idle)
        {
            SwitchState(Idle);
        }
    }

    public void SwitchState(MovementBaseState state)
    {
        currentState?.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }

    public void RotateToward(Vector3 worldPosition)
    {
        Vector3 directionToTarget = worldPosition - transform.position;
        directionToTarget.y = 0f;

        if (directionToTarget.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void OnDrawGizmos()
    {
        if (controller != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spherePos, controller.radius - 0.05f);
        }
    }
}
