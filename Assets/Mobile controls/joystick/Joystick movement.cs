using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoystickControl : MonoBehaviour
{
    public Animator playerAnim; // Reference to Animator
    public Rigidbody playerRigid; // Reference to Rigidbody
    public float moveSpeed = 7f; // Speed of movement
    public Transform cameraTransform; // Reference to the camera transform

    private Vector2 movementInput; // Stores joystick input
    private bool isInputEnabled = true; // Flag to control input processing

    private void OnEnable()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Run"].performed += HandleMovement;
        playerInput.actions["Run"].canceled += HandleMovementCanceled;
    }

    private void OnDisable()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Run"].performed -= HandleMovement;
        playerInput.actions["Run"].canceled -= HandleMovementCanceled;
    }

    private void HandleMovement(InputAction.CallbackContext context)
    {
        if (isInputEnabled)
        {
            movementInput = context.ReadValue<Vector2>() * -1f;
        }
    }

    private void HandleMovementCanceled(InputAction.CallbackContext context)
    {
        if (isInputEnabled)
        {
            movementInput = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (isInputEnabled)
        {
            Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            Vector3 worldMoveDirection = cameraForward * moveDirection.z + cameraRight * moveDirection.x;

            worldMoveDirection = -worldMoveDirection;

            playerRigid.velocity = worldMoveDirection * moveSpeed;

            if (worldMoveDirection.magnitude > 0)
            {
                playerRigid.MoveRotation(Quaternion.LookRotation(worldMoveDirection));

                float movementMagnitude = movementInput.magnitude;

                if (movementMagnitude >= 1.0f)
                {
                    playerAnim.SetTrigger("Run");
                    playerAnim.ResetTrigger("Walk");
                }
                else
                {
                    playerAnim.SetTrigger("Walk");
                    playerAnim.ResetTrigger("Run");
                }
            }
            else
            {
                playerAnim.SetTrigger("Idle");
                playerAnim.ResetTrigger("Run");
                playerAnim.ResetTrigger("Walk");
            }
        }
    }

    public bool IsMoving()
    {
        return movementInput.magnitude > 0;
    }

    // Method to enable or disable joystick input
    public void SetInputEnabled(bool enabled)
    {
        isInputEnabled = enabled;
    }
}