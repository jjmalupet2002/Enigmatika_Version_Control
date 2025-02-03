using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventoryObjectHandler : MonoBehaviour
{
    [Header("Item Attributes")]
    public string itemName;
    public Sprite itemIcon;
    [TextArea] public string itemDescription;

    [Header("If Item is a key")]
    public string keyId = ""; // KeyID for key items (empty if not a key)

    [Header("Inventory Settings")]
    public bool isClueItem;
    public bool isGeneralItem;
    public bool isUsable;

    [Header("Item Settings")]
    public bool isNote; // Flag for notes
    public bool is3dObject; // Flag for 3D objects

    [Header("UI Notification")]
    public GameObject notificationText; // Reference to Text UI
    public GameObject noteUI; // Reference to Note UI

    [Header("Inventory Manager reference")]
    public InventoryManager inventoryManager; // Reference to your InventoryManager


    private Vector3 originalPosition; // Original position of the object
    private bool isInspecting = false; // Inspection state
    private bool hasLoggedAlreadyInspecting = false; // Log flag for 3D items


    void Start()
    {
        if (is3dObject)
        {
            originalPosition = transform.position;
        }

        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Check for 3D object inspection
        if (is3dObject)
        {
            if (Vector3.Distance(transform.position, originalPosition) > 0.1f)
            {
                Inspect3DItem(); // Call InspectItem if moved
            }
            else if (isInspecting)
            {
                Stop3DInspect(); // Stop inspection if back at original position
            }
        }

        // Check the active state of the note UI
        if (noteUI != null)
        {
            if (noteUI.activeSelf)
            {
                NoteInspect(); // Call NoteInspect if active
            }
            else if (isInspecting)
            {
                StopNoteInspection(); // Stop inspection if inactive

            }
        }
    }

    public void Inspect3DItem()
    {
        if (isNote)
        {
            UnityEngine.Debug.Log("Cannot inspect a note as a 3D object.");
            return; // Prevent inspecting a note
        }

        if (isInspecting)
        {
            if (!hasLoggedAlreadyInspecting)
            {

                hasLoggedAlreadyInspecting = true; // Log flag
            }
            return; // Prevent re-inspection
        }

        isInspecting = true; // Set inspecting state
        StartCoroutine(NotifyPickupAfterDelay(1f)); // Notify after a delay
    }

    public void Stop3DInspect()
    {
        if (!isInspecting)
        {
            UnityEngine.Debug.Log("No inspection to stop.");
            return;
        }

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false; // Disable mesh renderer
        }

        // Disable colliders (box or sphere)
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false; // Disable collider

        }

        StartCoroutine(DisableGameObjectAfterDelay(2f)); // Disable game object after a delay
        isInspecting = false; // Reset state
        hasLoggedAlreadyInspecting = false; // Reset log flag
    }

    private bool hasBeenInspected = false; // Flag to track if the note has been inspected

    public void NoteInspect()
    {
        if (noteUI != null && noteUI.activeSelf)
        {
            if (isInspecting || hasBeenInspected)
            {
                return; // Prevent re-inspection if already inspecting or has been inspected
            }

            isInspecting = true; // Set inspecting state
            hasBeenInspected = true; // Set the flag to indicate it has been inspected
            StartCoroutine(NotifyPickupAfterDelay(1f)); // Notify after a delay
        }
    }

    public void StopNoteInspection()
    {
        if (!isInspecting)
        {
            UnityEngine.Debug.Log("No inspection to stop for the note."); // Log if there was no inspection to stop
            return;
        }

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false; // Disable mesh renderer
        }

        // Disable colliders (box or sphere)
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false; // Disable collider

        }

        isInspecting = false; // Reset state
        hasLoggedAlreadyInspecting = false; // Reset log flag for note inspection
    }


    private IEnumerator DisableGameObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false); // Disable the item

    }

    private IEnumerator NotifyPickupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NotifyPickup(); // Notify after delay

    }

    public void NotifyPickup()
    {
        ItemData newItemData = new ItemData(
            itemName,
            itemIcon,
            itemDescription,
            isClueItem,
            isGeneralItem,
            isUsable,
            false,
            isNote,
            hasBeenInspected, // Ensure this reflects inspection status
            noteUI,
            keyId
        );

        InventoryManager.Instance.AddItem(newItemData);

        // If the item has been inspected before pickup, update it in InventoryManager
        if (hasBeenInspected)
        {
            InventoryManager.Instance.InspectItem(newItemData);
        }

        // Show notification
        if (notificationText != null)
        {
            notificationText.SetActive(true);
            StartCoroutine(HideNotificationAfterDelay(2f));
        }
        else
        {
            UnityEngine.Debug.LogWarning("Notification text GameObject is not assigned.");
        }
    }


    private IEnumerator HideNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (notificationText != null)
        {
            notificationText.SetActive(false); // Hide notification

        }
    }
}