using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class AnimePlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private Vector2 movementInput = Vector2.zero;
    private bool jumped = false;

    [Header("Multiplayer Settings")]
    public int playerIndex = 0;  // Set externally (e.g., SplitScreenManager)

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Wrapper methods for UnityEvent compatibility (these WILL show in Inspector)
    public void HandleMove(InputAction.CallbackContext context)
    {
        OnMove(context);
    }

    public void HandleJump(InputAction.CallbackContext context)
    {
        OnJump(context);
    }

    // Internal logic
    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsCorrectPlayer(context))
        {
            movementInput = context.ReadValue<Vector2>();
            // Debug.Log($"Player {playerIndex} Move: {movementInput}");
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (IsCorrectPlayer(context))
        {
            if (context.started || context.performed)
            {
                jumped = true;
                // Debug.Log($"Player {playerIndex} Jump Pressed");
            }
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
        return false;
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

    private void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y);
        move = Vector3.ClampMagnitude(move, 1f);

        if (move != Vector3.zero)
        {
            transform.forward = move;
        }

        if (jumped && groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        Vector3 finalMove = (move * playerSpeed) + (playerVelocity.y * Vector3.up);
        controller.Move(finalMove * Time.deltaTime);

        jumped = false; // reset after update
    }
}
