using System.Collections.Generic; // For using List<T>
using UnityEngine;
using UnityEngine.Events; // For UnityEvent
using System.Collections; // For IEnumerator

public class InteractableDrawerClosetChest : MonoBehaviour
{
    private bool isOpen = false;

    // Booleans to check the type of interactable object
    public bool isCloset = false; // Set to true if this object is a closet
    public bool isDrawer = false; // Set to true if this object is a drawer
    public bool isChest = false;  // Set to true if this object is a chest

    // Header for Drawer Settings
    [Header("Drawer Settings")]
    public float openDistance = 0.5f; // Distance the drawer should move forward when opened
    public float openCloseSpeedDrawer = 2f; // Speed of the drawer opening/closing
    public bool isDrawerLocked = false; // If true, the drawer cannot be opened
    public GameObject drawerLockedUI; // Reference to the Drawer Locked UI
    public UnityEvent onUnlockDrawer; // Event to unlock the drawer

    // Header for Closet Settings
    [Header("Closet Settings")]
    public float closedYRotation = 0f; // Y rotation when closed
    public float openYRotation = 90f;  // Y rotation when open
    public float openCloseSpeedCloset = 2f; // Speed of the closet opening/closing

    // Header for Chest Settings
    [Header("Chest Settings")]
    public float closedXRotation = 0f; // X rotation when closed
    public float openXRotation = -90f; // X rotation when open
    public float openCloseSpeedChest = 2f; // Speed of the chest opening/closing
    public bool isChestLocked = false; // If true, the chest cannot be opened
    public GameObject chestLockedUI; // Reference to the Chest Locked UI
    public UnityEvent onUnlockChest; // Event to unlock the chest

    // Used to track if the drawer, closet, or chest is currently moving
    private bool isMoving = false;

    // Used to store the initial position of the drawer
    private Vector3 closedPosition; // To store the initial position of the drawer

    void Start()
    {
        // Set the initial position or rotation based on the type
        if (isCloset)
        {
            transform.localRotation = Quaternion.Euler(0f, closedYRotation, 0f); // Set initial rotation for closet
        }
        else if (isDrawer)
        {
            closedPosition = transform.localPosition; // Store initial position for drawer
        }
        else if (isChest)
        {
            transform.localRotation = Quaternion.Euler(closedXRotation, 0f, 0f); // Set initial rotation for chest
        }
    }

    void Update()
    {
        // Check for mouse click input
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput(Input.mousePosition);
        }

        // Check for touch input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleInput(Input.GetTouch(0).position);
        }

        // Move the drawer, closet, or chest based on its open state if not currently moving
        if (isMoving)
        {
            if (isCloset)
            {
                // Rotate for closet
                float targetY = isOpen ? openYRotation : closedYRotation;
                Quaternion targetRotation = Quaternion.Euler(0f, targetY, 0f);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * openCloseSpeedCloset);

                // Check if the closet has reached the target rotation
                if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
                {
                    transform.localRotation = targetRotation; // Snap to target rotation
                    isMoving = false; // Stop moving
                    UnityEngine.Debug.Log("Closet has finished moving to: " + (isOpen ? "Open" : "Closed")); // Debug log
                }
            }
            else if (isDrawer)
            {
                // Move for drawer based on its local forward direction
                Vector3 targetPosition = closedPosition + transform.localRotation * Vector3.forward * (isOpen ? openDistance : 0);
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * openCloseSpeedDrawer);

                // Check if the drawer has reached the target position
                if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
                {
                    transform.localPosition = targetPosition; // Snap to target position
                    isMoving = false; // Stop moving
                    UnityEngine.Debug.Log("Drawer has finished moving to: " + (isOpen ? "Open" : "Closed")); // Debug log
                }
            }
            else if (isChest)
            {
                // Rotate for chest
                float targetX = isOpen ? openXRotation : closedXRotation;
                Quaternion targetRotation = Quaternion.Euler(targetX, 0f, 0f);
                transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * openCloseSpeedChest);

                // Check if the chest has reached the target rotation
                if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
                {
                    transform.localRotation = targetRotation; // Snap to target rotation
                    isMoving = false; // Stop moving
                    UnityEngine.Debug.Log("Chest has finished moving to: " + (isOpen ? "Open" : "Closed")); // Debug log
                }
            }
        }
    }

    void HandleInput(Vector2 inputPosition)
    {
        // Check if the note UI is active
        NoteInspectionManager noteManager = FindObjectOfType<NoteInspectionManager>();
        if (noteManager != null && noteManager.isNoteUIActive)
        {
            return; // Skip interaction if the note UI is active
        }

        // Check if any instance of ItemInspectionManager has item inspection active
        ItemInspectionManager[] itemInspectionManagers = FindObjectsOfType<ItemInspectionManager>();
        foreach (var itemInspectionManager in itemInspectionManagers)
        {
            if (itemInspectionManager.IsInspecting())
            {
                return; // Skip interaction if any item inspection is active
            }
        }

        // Check if the close-up camera is active
        if (!IsCloseUpCameraActive())
        {
            return; // Skip interaction if close-up camera is not active
        }

        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject && !isMoving) // Ensure it's not moving
            {
                ToggleDrawerClosetOrChest();
            }
        }
    }

    public void ToggleDrawerClosetOrChest()
    {
        // Only toggle if not currently moving
        if (!isMoving)
        {
            if (isOpen) // If currently open, prepare to close
            {
                Close();
            }
            else // If currently closed, prepare to open
            {
                if (isDrawer && isDrawerLocked)
                {
                    drawerLockedUI.SetActive(true); // Show drawer locked UI
                    StartCoroutine(HideLockedUIAfterDelay(drawerLockedUI)); // Hide UI after delay
                    return;
                }

                if (isChest && isChestLocked)
                {
                    chestLockedUI.SetActive(true); // Show chest locked UI
                    StartCoroutine(HideLockedUIAfterDelay(chestLockedUI)); // Hide UI after delay
                    return;
                }

                Open();
            }
        }
    }

    private IEnumerator HideLockedUIAfterDelay(GameObject lockedUI)
    {
        yield return new WaitForSeconds(1.5f);
        lockedUI.SetActive(false);
    }

    private bool IsCloseUpCameraActive()
    {
        // Get all instances of SwitchCamera
        var switchCameras = FindObjectsOfType<SwitchCamera>();
        // Check if any instance has the CloseUp camera active
        foreach (var switchCamera in switchCameras)
        {
            if (switchCamera.currentCameraState == CameraState.CloseUp)
            {
                return true; // Return true if any close-up camera is active
            }
        }
        return false; // No close-up camera is active
    }

    private void Open()
    {
        isOpen = true; // Set the state to open
        isMoving = true; // Set moving to true
        UnityEngine.Debug.Log((isCloset ? "Closet" : isDrawer ? "Drawer" : "Chest") + " is now: Open"); // Debug log
    }

    private void Close()
    {
        isOpen = false; // Set the state to closed
        isMoving = true; // Set moving to true
        UnityEngine.Debug.Log((isCloset ? "Closet" : isDrawer ? "Drawer" : "Chest") + " is now: Closed"); // Debug log
    }
}