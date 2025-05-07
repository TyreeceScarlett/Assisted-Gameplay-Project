using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    public float walkSpeed = 3, walkBackSpeed = 2;
    public float runSpeed = 7, runBackSpeed = 5;
    public float crouchSpeed = 2, crouchBackSpeed = 1;

    private MovementBaseState currentState;

    public IdleState Idle = new IdleState();
    public WalkState walk = new WalkState();
    public CrouchingState crouch = new CrouchingState();
    public RunState run = new RunState();

    [HideInInspector] public Animator anim;
    [HideInInspector] public Vector3 dir;
    private float hzInput, vInput;
    private CharacterController controller;

    [SerializeField] private float groundYOffset = 0.2f;
    [SerializeField] private LayerMask groundMask;
    private Vector3 spherePos;

    [SerializeField] private float gravity = -9.81f;
    private Vector3 velocity;

    public float moveSpeed = 3;

    // Public read-only properties for input
    public float HzInput => hzInput; // Public getter for hzInput
    public float VInput => vInput; // Public getter for vInput
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

    public bool isCrouching = false; // New toggle variable for crouching

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
        // Gather inputs
        GetInputs();

        // Handle movement
        GetDirectionAndMove();
        ApplyGravity();

        // Update animator parameters
        anim.SetFloat("hzInput", HzInput); // Accessing the public property
        anim.SetFloat("vInput", VInput);   // Accessing the public property

        // Update state
        currentState.UpdateState(this);

        // Handle state transitions
        HandleStateSwitching();
    }

    void GetInputs()
    {
        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        // Ensure values are valid (in case of unexpected inputs causing NaN)
        if (float.IsNaN(hzInput) || float.IsNaN(vInput))
        {
            hzInput = 0f;
            vInput = 0f;
        }
        dir = transform.forward * vInput + transform.right * hzInput;

        IsRunning = Input.GetKey(KeyCode.LeftShift);

        // Toggle crouch when LeftControl is pressed
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = !isCrouching;

            // Switch to crouch state or return to walking/running/idle
            if (isCrouching)
                SwitchState(crouch);
            else
                SwitchState(GetMovementStateBasedOnInputs());
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
                velocity.y = -2f; // Prevent falling too quickly
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    // Switch to the correct state
    public void SwitchState(MovementBaseState state)
    {
        currentState?.ExitState(this);
        currentState = state;
        currentState.EnterState(this);
    }

    // Handle state switching logic
    void HandleStateSwitching()
    {
        // Prevent state switching if crouching
        if (isCrouching) return;

        // Handle Running State
        if (IsRunning && IsMoving)
        {
            SwitchState(run);
            return;
        }

        // Handle Walking State
        if (!IsRunning && IsMoving)
        {
            SwitchState(walk);
            return;
        }

        // Handle Idle State
        if (!IsMoving)
        {
            SwitchState(Idle);
        }
    }

    // Determine movement state based on input
    MovementBaseState GetMovementStateBasedOnInputs()
    {
        if (IsRunning)
        {
            return run;
        }
        else if (IsMoving)
        {
            return walk;
        }
        else
        {
            return Idle;
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
