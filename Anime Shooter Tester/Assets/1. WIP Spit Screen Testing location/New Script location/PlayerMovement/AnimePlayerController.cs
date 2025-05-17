using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(CharacterController))]
public class AnimePlayerController : MovementStateManager
{
    [Header("Input System")]
    private Vector2 movementInput = Vector2.zero;
    private bool jumpRequested = false;

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1.0f;

    private Vector3 playerVelocity;

    [Header("Multiplayer Settings")]
    public int playerIndex = 0; // For identifying Gamepad users

    // Override GetInputs to use InputSystem instead of old Input
    protected override void GetInputs()
    {
        // Update horizontal/vertical input from Input System
        hzInput = movementInput.x;
        vInput = movementInput.y;

        dir = transform.forward * vInput + transform.right * hzInput;

        // Check running keys/buttons for keyboard and gamepad
        IsRunning = (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed);
        if (Gamepad.all.Count > playerIndex)
        {
            IsRunning |= Gamepad.all[playerIndex].leftStickButton.isPressed;
        }

        // Toggle crouch on Left Ctrl (keyboard only here, expand as needed)
        if (Keyboard.current != null && Keyboard.current.leftCtrlKey.wasPressedThisFrame)
        {
            isCrouching = !isCrouching;
            if (isCrouching)
                SwitchState(crouch);
            else
                SwitchState(IsMoving ? (IsRunning ? run : walk) : Idle);
        }
    }

    void Update()
    {
        // Set animation params
        anim.SetFloat("hzInput", hzInput);
        anim.SetFloat("vInput", vInput);

        currentState.UpdateState(this);
        HandleStateSwitching();

        MoveCharacter();
    }

    void MoveCharacter()
    {
        // Gravity + jumping
        if (IsGrounded)
        {
            if (playerVelocity.y < 0)
                playerVelocity.y = -2f;

            if (jumpRequested)
            {
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpRequested = false;
            }
        }
        else
        {
            playerVelocity.y += gravity * Time.deltaTime;
        }

        Vector3 move = dir.normalized * moveSpeed;
        controller.Move((move + playerVelocity) * Time.deltaTime);
    }

    // Input System callbacks (hook these in your Input Actions)
    public void HandleMove(InputAction.CallbackContext context)
    {
        if (IsCorrectPlayer(context))
        {
            movementInput = context.ReadValue<Vector2>();
        }
    }

    public void HandleJump(InputAction.CallbackContext context)
    {
        if (IsCorrectPlayer(context) && (context.started || context.performed))
        {
            jumpRequested = true;
        }
    }

    private bool IsCorrectPlayer(InputAction.CallbackContext context)
    {
        var device = context.control.device;
        if (device is Gamepad gamepad)
        {
            int index = GetGamepadIndex(gamepad);
            return index == playerIndex;
        }
        // Assume keyboard is always correct player for simplicity
        return true;
    }

    private int GetGamepadIndex(Gamepad gamepad)
    {
        for (int i = 0; i < Gamepad.all.Count; i++)
        {
            if (Gamepad.all[i] == gamepad)
                return i;
        }
        return -1;
    }
}
