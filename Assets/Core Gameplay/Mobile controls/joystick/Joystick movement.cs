using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using CarterGames.Assets.SaveManager;
using Save;


public class PlayerJoystickControl : MonoBehaviour
{
    // Player Controller Settings
    [Header("Player Controller Settings:")]
    public Animator playerAnim; // Reference to Animator
    public Rigidbody playerRigid; // Reference to Rigidbody
    public float moveSpeed = 5f; // Speed of movement
    public Transform cameraTransform; // Reference to the camera transform

    // Stair Climb Settings
    [Header("Stair Climb Settings:")]
    [SerializeField] GameObject stepRayUpper; // Upper ray for stair detection
    [SerializeField] GameObject stepRayLower; // Lower ray for stair detection
    [SerializeField] float stepHeight = 0.3f; // Height of the stairs
    [SerializeField] float stepSmooth = 2f; // Smoothing for climbing

    [Header("Save System:")]
    public PlayerPositionSaveObject playerPositionSaveObject; // Reference to your PlayerPositionSaveObject

    [Header("Movement Inversion:")]
    public bool invertMovement = false; // Toggle to invert movement
    public GameObject cloakObject;

    private Vector2 movementInput; // Stores joystick input
    private bool isInputEnabled = true; // Flag to control input processing
    private bool isOnStairs = false; // Flag to check if the player is on stairs
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

        // Initialize the position of the upper ray
        stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
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
            ClimbStairs();
            ApplyGravity();
            UpdateInvertMovementBasedOnCloak();
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
            Vector2 rawInput = context.ReadValue<Vector2>();
            movementInput = invertMovement ? rawInput : -rawInput;
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

    private void UpdateInvertMovementBasedOnCloak()
    {
        if (cloakObject != null)
        {
            invertMovement = cloakObject.activeInHierarchy;
        }
    }

    private Vector2 GetProcessedMovementInput(Vector2 rawInput)
    {
        return invertMovement ? -rawInput : rawInput;
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

    private void ClimbStairs()
    {
        bool isClimbing = false;

        // Check for forward climb
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
            {
                playerRigid.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                isClimbing = true;
            }
        }
        else
        {
            // Check for diagonal climbs (45 degrees)
            if (CheckClimb(Vector3.forward + Vector3.right) || CheckClimb(Vector3.forward + Vector3.left))
            {
                isClimbing = true;
            }
        }

        // Check for downward climb
        if (Physics.Raycast(stepRayUpper.transform.position, -transform.up, out RaycastHit hitUpperCheck, 0.2f))
        {
            isOnStairs = true; // Set the flag indicating player is on stairs
            AdjustMovementSpeed(true); // Climbing down, reduced speed
        }
        else
        {
            isOnStairs = false; // Reset the flag
            if (!isClimbing)
            {
                AdjustMovementSpeed(false); // Normal speed
            }
        }
    }

    private bool CheckClimb(Vector3 direction)
    {
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(direction), out RaycastHit hitLower45, 0.1f))
        {
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(direction), out RaycastHit hitUpper45, 0.2f))
            {
                playerRigid.position -= new Vector3(0f, -stepSmooth * Time.deltaTime, 0f);
                return true;
            }
        }
        return false;
    }

    private void AdjustMovementSpeed(bool isDescending)
    {
        if (isDescending)
        {
            moveSpeed = 3f; // Reduced speed when climbing down
        }
        else
        {
            moveSpeed = 4f; // Normal speed when not climbing down
        }
    }

    private void ApplyGravity()
    {
        if (!isOnStairs)
        {
            playerRigid.AddForce(Physics.gravity, ForceMode.Acceleration);
        }
        else
        {
            // Only keep the vertical velocity zero when climbing stairs, not when idle
            if (IsMoving())
            {
                playerRigid.velocity = new Vector3(playerRigid.velocity.x, 0, playerRigid.velocity.z);
            }
            else
            {
                // Disable gravity when idle on stairs
                playerRigid.useGravity = false;
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
