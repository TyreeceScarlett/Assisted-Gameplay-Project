using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    public float moveSpeed = 3;
    [HideInInspector] public Vector3 dir;
    float hzInput, vInput;
    CharacterController controller;

    [SerializeField] float groundYOffset = 0.2f;  // Adjust for accurate ground detection
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;

    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (controller != null)
        {
            // Adjust the center of the capsule collider to make sure it's aligned at the bottom
            controller.center = new Vector3(0, controller.height / 2, 0);
        }
    }

    void Update()
    {
        GetDirectionAndMove();
        Gravity();
    }

    void GetDirectionAndMove()
    {
        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        dir = transform.forward * vInput + transform.right * hzInput;

        controller.Move(dir.normalized * moveSpeed * Time.deltaTime);
    }

    bool IsGrounded()
    {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        return Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask);
    }

    void Gravity()
    {
        if (IsGrounded())
        {
            // Reset vertical velocity when grounded
            if (velocity.y < 0)
                velocity.y = -2f;

            // Ensure the Y position is exactly 0 when grounded (only on the Y axis)
            transform.position = new Vector3(transform.position.x, Mathf.Max(transform.position.y, 0f), transform.position.z);
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Apply gravity and movement
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spherePos, controller.radius - 0.05f);
    }
}