using System.Collections; // Needed for using IEnumerator
using UnityEngine;
using UnityEngine.UI; // Needed for UI elements

public class ItemInventoryObjectHandler : MonoBehaviour
{
    [Header("Item Attributes")]
    public string itemName;
    public Sprite itemIcon;
    [TextArea] public string itemDescription;

    [Header("Other Settings")]
    public bool isClueItem;
    public bool isGeneralItem;
    public bool isUsable;

    [Header("UI Notification")]
    public Text notificationText; // Reference to the Text UI object

    private Vector3 originalPosition; // Store the original position of the object
    private bool isInspecting = false; // Track whether the item is being inspected
    private bool hasLoggedAlreadyInspecting = false; // Flag to track if the log has been printed

    void Start()
    {
        // Store the original position of the 3D object
        originalPosition = transform.position;

        // Ensure the notification text is hidden at the start
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Check if the object has moved from its original position
        if (Vector3.Distance(transform.position, originalPosition) > 0.1f) // Adjust the threshold as needed
        {
            InspectItem(); // Call the InspectItem method if the object has moved
        }
        else if (isInspecting) // If the object returns to its original position while inspecting
        {
            StopInspection(); // Call StopInspection method
        }
    }

    public void InspectItem() // Call this method when an item is inspected
    {
        // Check if an item is already being inspected
        if (isInspecting)
        {
            if (!hasLoggedAlreadyInspecting) // Check if we have already logged this message
            {
                UnityEngine.Debug.Log("Item is already being inspected.");
                hasLoggedAlreadyInspecting = true; // Set the flag to true after logging
            }
            return; // Prevent re-inspection if already active
        }

        // Logic for inspecting the item (show UI, etc.)
        UnityEngine.Debug.Log($"Inspecting item: {itemName}");

        isInspecting = true; // Set inspecting state to true
        StartCoroutine(NotifyPickupAfterDelay(1f)); // Notify pickup after a delay
    }

    public void StopInspection()
    {
        if (!isInspecting) // Only stop inspection if it is active
        {
            UnityEngine.Debug.Log("No inspection to stop.");
            return;
        }

        // Disable the item's mesh renderer or hide it in the scene
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false; // Disable the mesh renderer
            // Log that the inspection has stopped for this item
            UnityEngine.Debug.Log($"Inspection stopped for item: {itemName}");
        }

        // Disable the game object after a short delay
        StartCoroutine(DisableGameObjectAfterDelay(2f)); // Adjust the delay as needed

        isInspecting = false; // Reset inspecting state
    }

    private IEnumerator DisableGameObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        gameObject.SetActive(false); // Disable the item in the scene
    }

    private IEnumerator NotifyPickupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NotifyPickup(); // Notify after the delay
    }

    public void NotifyPickup()
    {
        // Logic for notifying the inventory manager to add this item
        UnityEngine.Debug.Log($"New item added: {itemName}");

        // Display the notification
        if (notificationText != null)
        {
            notificationText.text = "A new item has been added, check your inventory!";
            notificationText.gameObject.SetActive(true); // Show the notification
            StartCoroutine(HideNotificationAfterDelay(2f)); // Hide the notification after 1 second
        }

        // Any additional logic for notifying pickup can be added here
    }

    private IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false); // Hide the notification
        }
    }
}
