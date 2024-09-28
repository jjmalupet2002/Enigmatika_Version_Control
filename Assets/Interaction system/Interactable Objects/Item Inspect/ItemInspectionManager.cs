using UnityEngine;
using UnityEngine.Rendering; // For ShadowCastingMode
using System.Collections.Generic; // For List<T>
using UnityEngine.InputSystem; // Import for the new Input System
using UnityEngine.InputSystem.Controls; // Needed for Input Control types

public class ItemInspectionManager : MonoBehaviour
{
    public List<GameObject> itemsToInspect; // List of items to inspect
    private Transform inspectionPoint;
    private bool isInspecting = false;
    private float rotationSpeed = 1000f;
    private float rotateSmoothness = 0.1f; // Adjust the smoothness of rotation

    private Vector3 targetRotation;

    // Store original positions, rotations, and shadow casting modes for each item
    private List<Vector3> originalPositions = new List<Vector3>();
    private List<Quaternion> originalRotations = new List<Quaternion>();
    private List<ShadowCastingMode> originalShadowCastingModes = new List<ShadowCastingMode>();

    // Store current positions and rotations for manipulation
    private List<Vector3> currentPositions = new List<Vector3>();
    private List<Quaternion> currentRotations = new List<Quaternion>();

    private int currentItemIndex = -1; // To track the current item being inspected

    // Reference to SwitchCamera script
    public SwitchCamera switchCamera;

    // Reference to the Input Action Asset
    public InputActionAsset inputActions; // Drag your Input Action Asset here in the inspector
    private InputAction rotateItemAction;

    public CloseUpViewUIController closeUpViewUIController; // Reference to the UI controller

    // Reference to the BackButtonHandler script
    [SerializeField] private BackButtonHandler backButtonHandler; // Serialized reference

    void Start()
    {
        // Initialize the input action for rotation
        rotateItemAction = inputActions.FindAction("RotateItem"); // Make sure "RotateItem" matches your action name
        rotateItemAction.Enable();

        inspectionPoint = transform;
        targetRotation = Vector3.zero;

        // Store the original position, rotation, and shadow casting mode for each item
        foreach (GameObject item in itemsToInspect)
        {
            if (item != null)
            {
                originalPositions.Add(item.transform.position);
                originalRotations.Add(item.transform.rotation);

                // Initialize current positions and rotations
                currentPositions.Add(item.transform.position);
                currentRotations.Add(item.transform.rotation);

                MeshRenderer meshRenderer = item.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    originalShadowCastingModes.Add(meshRenderer.shadowCastingMode);
                }
                else
                {
                    originalShadowCastingModes.Add(ShadowCastingMode.Off); // Default value if no MeshRenderer
                }
            }
        }
    }

    public bool IsInspecting()
    {
        return isInspecting;
    }

    public void StopInspection()
    {
        // Disable the UI elements when exiting inspection mode
        if (closeUpViewUIController != null)
        {
            closeUpViewUIController.SetUIActive(false); // Disable UI
        }

        if (itemsToInspect.Count > 0 && isInspecting && currentItemIndex >= 0)
        {
            GameObject itemToInspect = itemsToInspect[currentItemIndex]; // Get the current item to reset
            itemToInspect.transform.SetParent(null);
            itemToInspect.transform.position = originalPositions[currentItemIndex]; // Return to original position
            itemToInspect.transform.rotation = originalRotations[currentItemIndex]; // Return to original rotation

            isInspecting = false;

            // Reset shadow casting to original value
            MeshRenderer meshRenderer = itemToInspect.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.shadowCastingMode = originalShadowCastingModes[currentItemIndex];
            }

            // Re-enable colliders for all items
            EnableAllItemColliders();
        }
    }

    void InspectItem()
    {
        // Enable the UI elements when entering inspection mode
        if (closeUpViewUIController != null)
        {
            closeUpViewUIController.SetUIActive(true); // Enable UI
        }

        if (itemsToInspect.Count > 0 && !isInspecting && currentItemIndex >= 0)
        {
            GameObject itemToInspect = itemsToInspect[currentItemIndex]; // Get the current item to inspect

            // Set the position to the inspection point without changing the rotation
            itemToInspect.transform.position = inspectionPoint.position;

            // Keep the original rotation
            itemToInspect.transform.rotation = originalRotations[currentItemIndex];

            // Set parent to the inspection point to manage its hierarchy
            itemToInspect.transform.SetParent(inspectionPoint);

            isInspecting = true;

            // Reset target rotation to original rotation
            targetRotation = originalRotations[currentItemIndex].eulerAngles; // Use original rotation

            // Reset current positions and rotations to original values
            currentPositions[currentItemIndex] = originalPositions[currentItemIndex];
            currentRotations[currentItemIndex] = originalRotations[currentItemIndex];

            // Disable colliders for all other items
            DisableOtherItemColliders(itemToInspect);

            // Enable shadow casting while inspecting
            MeshRenderer meshRenderer = itemToInspect.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            }
        }
    }

    // Add a sensitivity factor for touch controls
    private float touchSensitivity = 0.03f; // Adjust this value to change sensitivity for touch input

    void Update()
    {
        // Check if the current camera state is CloseUp before allowing inspection
        if (switchCamera != null && switchCamera.currentCameraState == CameraState.CloseUp)
        {
            // Handle mouse click for inspection
            if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // Check if any of the items are clicked
                for (int i = 0; i < itemsToInspect.Count; i++)
                {
                    if (Physics.Raycast(ray, out hit) && hit.transform.gameObject == itemsToInspect[i]) // Check if clicked object is one of the items
                    {
                        currentItemIndex = i; // Set current item index
                        InspectItem();
                        break; // Exit the loop after the first hit
                    }
                }
            }
        }

        if (isInspecting)
        {
            // Handle rotation with mouse
            if (Input.GetMouseButton(0))
            {
                float rotateX = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime; // Inverted Y axis for dragging down
                float rotateY = -Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime; // Inverted X axis for dragging left

                targetRotation += new Vector3(rotateX, rotateY, 0f); // Update target rotation

                // Smoothly rotate the item
                Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
                itemsToInspect[currentItemIndex].transform.rotation = Quaternion.Slerp(itemsToInspect[currentItemIndex].transform.rotation, targetQuaternion, rotateSmoothness);
            }

            // Handle rotation with touch input using Touchscreen
            var touchscreen = Touchscreen.current; // Access the current touchscreen input
            if (touchscreen != null && touchscreen.primaryTouch.press.isPressed) // Check if the primary touch is pressed
            {
                var touch = touchscreen.primaryTouch; // Access the primary touch
                if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved) // Check if the touch has moved
                {
                    float rotateX = -touch.delta.ReadValue().y * rotationSpeed * touchSensitivity * Time.deltaTime; // Inverted Y axis
                    float rotateY = -touch.delta.ReadValue().x * rotationSpeed * touchSensitivity * Time.deltaTime; // Inverted X axis

                    targetRotation += new Vector3(rotateX, rotateY, 0f); // Update target rotation

                    // Smoothly rotate the item
                    Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
                    itemsToInspect[currentItemIndex].transform.rotation = Quaternion.Slerp(itemsToInspect[currentItemIndex].transform.rotation, targetQuaternion, rotateSmoothness);
                }
            }
        }
    }

    // Function to disable colliders for all items except the currently inspected one
    private void DisableOtherItemColliders(GameObject inspectedItem)
    {
        foreach (GameObject item in itemsToInspect)
        {
            if (item != inspectedItem)
            {
                Collider collider = item.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = false; // Disable the collider
                }
            }
        }
    }

    // Function to re-enable colliders for all items
    private void EnableAllItemColliders()
    {
        foreach (GameObject item in itemsToInspect)
        {
            Collider collider = item.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true; // Re-enable the collider
            }
        }
    }
}

