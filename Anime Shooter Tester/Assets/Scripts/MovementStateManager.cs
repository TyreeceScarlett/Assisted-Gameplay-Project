using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    public float walkSpeed = 3, walkBackSpeed = 2;
    public float runSpeed = 7, runBackSpeed = 5;
    public float crouchSpeed = 2, crouchBackSpeed = 1;

    MovementBaseState currentState;

    public IdleState Idle = new IdleState();
    public WalkState walk = new WalkState();
    public CrouchingState crouch = new CrouchingState();
    public RunState run = new RunState();

    [HideInInspector] public Animator anim;
    [HideInInspector] public Vector3 dir;
    private float hzInput, vInput;
    CharacterController controller;

    [SerializeField] float groundYOffset = 0.2f;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;

    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;

    public float moveSpeed = 3;

    // Public read-only property for input
    public float VInput => vInput;
    public bool IsRunning { get; private set; }
    public bool IsMoving => dir.magnitude > 0.1f;
    public bool IsGrounded
    {
        get
        {
            spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
            return Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask);
        }
    }

    public bool isCrouching = false; // 🔥 New toggle variable

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<CharacterController>();
        SwitchState(Idle);

        if (controller != null)
        {
            controller.center = new Vector3(0, controller.height / 2, 0);
        }
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

        // Toggle crouch when LeftControl is pressed once
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

    public void SwitchState(MovementBaseState state)
    {
        currentState?.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }

    void HandleStateSwitching()
    {
        // Prevent state switching if crouching
        if (isCrouching)
            return;

        // Running
        if (IsRunning && IsMoving)
        {
            if (currentState != run)
                SwitchState(run);
            return;
        }

        // Walking
        if (!IsRunning && IsMoving)
        {
            if (currentState != walk)
                SwitchState(walk);
            return;
        }

        // Idle
        if (!IsMoving)
        {
            if (currentState != Idle)
                SwitchState(Idle);
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