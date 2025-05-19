using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CarterGames.Assets.SaveManager;
using Save;


public class minigame_movement : MonoBehaviour
{
    // Player Controller Settings
    [Header("Player Controller Settings:")]
    public Animator playerAnim; // Reference to Animator
    public Rigidbody playerRigid; // Reference to Rigidbody
    public float moveSpeed = 10f; // Speed of movement
    public Transform cameraTransform; // Reference to the camera transform

    [Header("Save System:")]
    public PlayerPositionSaveObject playerPositionSaveObject; // Reference to your PlayerPositionSaveObject

    private Vector2 movementInput; // Stores joystick input
    private bool isInputEnabled = true; // Flag to control input processing
    private float idleTimer = 0f; // Timer to track idle time
    private const float idleThreshold = 15f; // Time threshold for secondary idle animation

    private void Awake()
    {
        // Automatically assign the main camera transform if not already set
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform; // Assuming the main camera is tagged as "MainCamera"
            if (cameraTransform == null)
            {
                UnityEngine.Debug.LogError("Main Camera not found in the scene!");
            }
        }


    }

    private void SavePlayerPosition()
    {
        playerPositionSaveObject.playerPosition.Value = transform.position;
        SaveManager.Save(playerPositionSaveObject);
    }

    private bool hasLoadedPosition = false;

    // This function is triggered when you manually load the game
    public void ManuallyLoadPlayerPosition()
    {
        if (!hasLoadedPosition)
        {
            LoadPlayerPosition();
        }
    }

    private void LoadPlayerPosition()
    {
        if (playerPositionSaveObject.playerPosition.Value != Vector3.zero)
        {
            // Temporarily disable gravity to prevent unwanted sliding
            playerRigid.useGravity = false;

            // Move the Rigidbody to the saved position
            playerRigid.MovePosition(playerPositionSaveObject.playerPosition.Value);

            // Reset velocity to ensure no unintended forces are applied
            playerRigid.velocity = Vector3.zero;

            hasLoadedPosition = true;

            // Re-enable gravity after setting position
            playerRigid.useGravity = true;
        }
        else
        {
            UnityEngine.Debug.Log("No saved position found.");
        }
    }

    private void FixedUpdate()
    {
        // Removed the call to LoadPlayerPosition() here
        if (isInputEnabled && GameStateManager.Instance.CanPlayerMove())
        {
            MovePlayer();
        }
    }

    private void OnEnable()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Run"].performed += HandleMovement;
        playerInput.actions["Run"].canceled += HandleMovementCanceled;
        // Subscribe to save and load events
        SaveEvents.OnSaveGame += SavePlayerPosition;
        SaveEvents.OnLoadGame += LoadPlayerPosition;
    }

    private void OnDisable()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Run"].performed -= HandleMovement;
        playerInput.actions["Run"].canceled -= HandleMovementCanceled;
        // Unsubscribe to avoid memory leaks
        SaveEvents.OnSaveGame -= SavePlayerPosition;
        SaveEvents.OnLoadGame -= LoadPlayerPosition;
    }

    private void HandleMovement(InputAction.CallbackContext context)
    {
        if (isInputEnabled && GameStateManager.Instance.CanPlayerMove())
        {
            movementInput = context.ReadValue<Vector2>() * -1f;
            idleTimer = 0f; // Reset idle timer on movement
        }
    }

    private void HandleMovementCanceled(InputAction.CallbackContext context)
    {
        if (isInputEnabled)
        {
            movementInput = Vector2.zero;
        }
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        Vector3 worldMoveDirection = cameraForward * moveDirection.z + cameraRight * moveDirection.x;

        worldMoveDirection = -worldMoveDirection;

        playerRigid.velocity = new Vector3(worldMoveDirection.x * moveSpeed, playerRigid.velocity.y, worldMoveDirection.z * moveSpeed); // Maintain vertical velocity

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

            // Reset idle timer when moving
            idleTimer = 0f;
        }
        else
        {
            playerAnim.SetTrigger("Idle");
            playerAnim.ResetTrigger("Run");
            playerAnim.ResetTrigger("Walk");

            // Increment idle timer when not moving
            idleTimer += Time.fixedDeltaTime;
            if (idleTimer >= idleThreshold)
            {
                playerAnim.SetTrigger("IdleLong");
            }
        }
    }

    public bool IsMoving()
    {
        return movementInput.magnitude > 0;
    }

    public void SetInputEnabled(bool enabled)
    {
        isInputEnabled = enabled;
        if (!enabled)
        {
            movementInput = Vector2.zero; // Stop movement when input is disabled

        }
    }
}
