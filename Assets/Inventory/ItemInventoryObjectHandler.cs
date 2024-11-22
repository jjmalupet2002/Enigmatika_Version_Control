using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ItemRewardGiver : MonoBehaviour
{
    [Header("Item Attributes")]
    private string itemName;
    private Sprite itemIcon;
    [TextArea] private string itemDescription;

    [Header("Inventory Settings")]
    private bool isClueItem;
    private bool isGeneralItem;
    private bool isUsable;

    [Header("Item Settings")]
    private bool isNote;
    private bool is3dObject;

    [Header("UI Notification")]
    private GameObject notificationText;
    private GameObject noteUI;

    [Header("Inventory Manager reference")]
    public InventoryManager inventoryManager;

    [Header("Item Events")]
    public UnityEvent item1Event;  // Event for the first item
    public UnityEvent item2Event;  // Event for the second item
    public UnityEvent item3Event;  // Event for the third item
    public UnityEvent item4Event;  // Event for the fourth item

    private List<UnityEvent> itemEvents = new List<UnityEvent>();  // List to store item events
    public List<ItemData> itemList = new List<ItemData>();  // List to store item data
    private bool[] hasBeenStored = new bool[4];  // Track if each item has been stored

    void Start()
    {
        // Initialize the item events list with the four item events
        itemEvents.Add(item1Event);
        itemEvents.Add(item2Event);
        itemEvents.Add(item3Event);
        itemEvents.Add(item4Event);

        // Initialize itemList with default ItemData objects for 4 slots
        for (int i = 0; i < 4; i++)
        {
            itemList.Add(new ItemData(
                    "",                    // Default name
                    null,                   // Default icon (null for now)
                    "",                    // Default description
                    false,                  // Default isClueItem
                    false,                  // Default isGeneralItem
                    false,                  // Default isUsable
                    false,                  // Default isStored
                    false,                  // Default isNote
                    null,                   // Default noteUI (null for now)
                    ""                      // Default additionalInfo
            ));
        }

        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }

    // Add items to the list (you can add them via Inspector or dynamically at runtime)
    public void AddItemToList(ItemData newItem, int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= 4) return;  // Ensure the index is valid
        itemList[itemIndex] = newItem;  // Replace the item data at the given index
        hasBeenStored[itemIndex] = false;  // Reset the stored flag
    }

    // Handle item events when triggered
    public void HandleItemEvent(int itemIndex)
    {
        // Ensure itemIndex is within bounds
        if (itemIndex < 0 || itemIndex >= 4)
        {
            UnityEngine.Debug.LogError("Item index out of bounds!");
            return;
        }

        // Notify inventory system if the item hasn't been stored
        if (!hasBeenStored[itemIndex])
        {
            NotifyPickup(itemIndex);
            hasBeenStored[itemIndex] = true;
        }

        // Invoke the event for the item
        itemEvents[itemIndex]?.Invoke();
    }

    private void NotifyPickup(int itemIndex)
    {
        // Prevent adding the item again if already stored
        if (hasBeenStored[itemIndex])
        {
            return;
        }

        // Create an instance of ItemData for the item
        ItemData newItemData = new ItemData(
            itemList[itemIndex].itemName,                // The item name
            itemList[itemIndex].itemIcon,                // The item icon
            itemList[itemIndex].itemDescription,         // The item description
            itemList[itemIndex].isClueItem,             // Whether it's a clue item
            itemList[itemIndex].isGeneralItem,          // Whether it's a general item
            itemList[itemIndex].isUsable,               // Whether it's usable
            false,                                       // isStored, initially false as it hasn't been stored yet
            itemList[itemIndex].isNote,                 // Whether it's a note
            itemList[itemIndex].noteUI,                 // The note UI (if applicable)
            ""                                           // Placeholder for any additional information (if needed)
        );

        // Notify the Inventory Manager to add this item
        inventoryManager.AddItem(newItemData);

        // Show notification
        if (notificationText != null)
        {
            notificationText.SetActive(true); // Show notification
            StartCoroutine(HideNotificationAfterDelay(2f)); // Hide after 2 seconds
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