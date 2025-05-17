using UnityEngine;
using UnityEngine.InputSystem;

public class SplitScreenManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;

    public Camera camera1;
    public Camera camera2;

    private bool player2Joined = false;  // <-- This MUST be here, as a member variable

    void Start()
    {
        player2.SetActive(false);
        camera2.gameObject.SetActive(false);

        camera1.rect = new Rect(0f, 0f, 1f, 1f);

        var p1Controller = player1.GetComponent<AnimePlayerController>();
        if (p1Controller != null) p1Controller.playerIndex = 0;

        var p2Controller = player2.GetComponent<AnimePlayerController>();
        if (p2Controller != null) p2Controller.playerIndex = 1;
    }

    void Update()
    {
        if (!player2Joined)
        {
            if (Gamepad.all.Count > 1)
            {
                var gamepad2 = Gamepad.all[1];
                if (IsAnyButtonPressed(gamepad2))
                {
                    player2Joined = true;

                    player2.SetActive(true);
                    camera2.gameObject.SetActive(true);

                    camera1.rect = new Rect(0f, 0.5f, 1f, 0.5f);
                    camera2.rect = new Rect(0f, 0f, 1f, 0.5f);
                }
            }
        }
    }

    private bool IsAnyButtonPressed(Gamepad gamepad)
    {
        if (gamepad == null) return false;

        if (gamepad.buttonSouth.wasPressedThisFrame) return true;
        if (gamepad.buttonNorth.wasPressedThisFrame) return true;
        if (gamepad.buttonWest.wasPressedThisFrame) return true;
        if (gamepad.buttonEast.wasPressedThisFrame) return true;
        if (gamepad.leftShoulder.wasPressedThisFrame) return true;
        if (gamepad.rightShoulder.wasPressedThisFrame) return true;
        if (gamepad.leftStickButton.wasPressedThisFrame) return true;
        if (gamepad.rightStickButton.wasPressedThisFrame) return true;
        if (gamepad.startButton.wasPressedThisFrame) return true;
        if (gamepad.selectButton.wasPressedThisFrame) return true;
        if (gamepad.dpad.up.wasPressedThisFrame) return true;
        if (gamepad.dpad.down.wasPressedThisFrame) return true;
        if (gamepad.dpad.left.wasPressedThisFrame) return true;
        if (gamepad.dpad.right.wasPressedThisFrame) return true;

        return false;
    }
}
