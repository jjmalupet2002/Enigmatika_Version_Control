using UnityEngine;
using UnityEngine.Rendering; // For ShadowCastingMode
using System.Collections.Generic; // For List<T>

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

    // Static variable to track if any item is currently being inspected
    private static ItemInspectionManager currentlyInspecting;

    void Start()
    {
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

    void InspectItem()
    {
        if (itemsToInspect.Count > 0 && !isInspecting && currentItemIndex >= 0)
        {
            // If another item is currently being inspected, stop its inspection
            if (currentlyInspecting != null && currentlyInspecting != this)
            {
                currentlyInspecting.StopInspection();
            }

            GameObject itemToInspect = itemsToInspect[currentItemIndex]; // Get the current item to inspect

            // Set the position to the inspection point without changing the rotation
            itemToInspect.transform.position = inspectionPoint.position;

            // Keep the original rotation
            itemToInspect.transform.rotation = originalRotations[currentItemIndex];

            // Set parent to the inspection point to manage its hierarchy
            itemToInspect.transform.SetParent(inspectionPoint);

            isInspecting = true;
            currentlyInspecting = this; // Set this instance as the currently inspecting one

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
                meshRenderer.shadowCastingMode = ShadowCastingMode.On;
            }
        }
    }


    void StopInspection()
    {
        if (itemsToInspect.Count > 0 && isInspecting && currentItemIndex >= 0)
        {
            GameObject itemToInspect = itemsToInspect[currentItemIndex]; // Get the current item to reset
            itemToInspect.transform.SetParent(null);
            itemToInspect.transform.position = originalPositions[currentItemIndex]; // Return to original position
            itemToInspect.transform.rotation = originalRotations[currentItemIndex]; // Return to original rotation

            // You may want to leave the current positions and rotations unchanged here
            // if you want the item to maintain its manipulated state until the next inspection

            isInspecting = false;
            currentlyInspecting = null; // Reset the currently inspecting instance

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

    void Update()
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopInspection(); // Stop inspection if it's active
        }

        if (isInspecting)
        {
            if (Input.GetMouseButton(0))
            {
                // Invert the mouse input for rotation
                float rotateX = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime; // Inverted Y axis for dragging down
                float rotateY = -Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime; // Inverted X axis for dragging left

                // Update the target rotation using the inverted input
                targetRotation += new Vector3(rotateX, rotateY, 0f); // Keep the order of rotation the same

                // Use Slerp for smooth rotation and increase responsiveness
                Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
                itemsToInspect[currentItemIndex].transform.rotation = Quaternion.Slerp(itemsToInspect[currentItemIndex].transform.rotation, targetQuaternion, rotateSmoothness);
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
