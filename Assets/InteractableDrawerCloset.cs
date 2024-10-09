using System.Collections.Generic; // For using List<T>
using UnityEngine;

public class InteractableDrawerCloset : MonoBehaviour
{
    private bool isOpen = false;

    // Boolean to check if it's a closet or a drawer
    public bool isCloset = false; // Set to true if this object is a closet
    public bool isDrawer = false; // Set to true if this object is a drawer

    // Header for Drawer Settings
    [Header("Drawer Settings")]
    public float openDistance = 0.5f; // Distance the drawer should move forward when opened
    public float openCloseSpeedDrawer = 2f; // Speed of the drawer opening/closing

    // Header for Closet Settings
    [Header("Closet Settings")]
    public float closedYRotation = 0f; // Y rotation when closed
    public float openYRotation = 90f;  // Y rotation when open
    public float openCloseSpeedCloset = 2f; // Speed of the closet opening/closing

    // Used to track if the drawer or closet is currently moving
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

        // Move the drawer or closet based on its open state if not currently moving
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

        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject && !isMoving) // Ensure it's not moving
            {
                ToggleDrawerOrCloset();
            }
        }
    }

    public void ToggleDrawerOrCloset()
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
                Open();
            }
        }
    }

    private void Open()
    {
        isOpen = true; // Set the state to open
        isMoving = true; // Set moving to true
        UnityEngine.Debug.Log((isCloset ? "Closet" : "Drawer") + " is now: Open"); // Debug log
    }

    private void Close()
    {
        isOpen = false; // Set the state to closed
        isMoving = true; // Set moving to true
        UnityEngine.Debug.Log((isCloset ? "Closet" : "Drawer") + " is now: Closed"); // Debug log
    }
}
