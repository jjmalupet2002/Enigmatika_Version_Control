using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEventHandler : MonoBehaviour
{
    [Header("Item Attributes")]
    public List<ItemData> items = new List<ItemData>(4);  // This will now be editable in the Inspector

    [Header("Inventory Manager reference")]
    public InventoryManager inventoryManager; // Reference to your InventoryManager

    [Header("Notification UI reference")]
    public GameObject notificationTextUI; // Reference to the notification text UI GameObject

    // Events for each item
    public delegate void ItemEvent();
    public event ItemEvent item1Event;
    public event ItemEvent item2Event;
    public event ItemEvent item3Event;
    public event ItemEvent item4Event;

    void Start()
    {
        // Ensure that the items list is properly initialized and filled with default ItemData objects
        if (items.Count < 4)
        {
            // Automatically fill with empty ItemData (if necessary).
            while (items.Count < 4)
            {
                items.Add(new ItemData("", null, "", false, false, false, false, false, null, ""));
            }
        }

        // Hide the notification UI initially
        if (notificationTextUI != null)
        {
            notificationTextUI.SetActive(false);
        }
    }

    // Method to handle item events
    public void HandleItemEvent(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= items.Count)
        {
            UnityEngine.Debug.LogError("Invalid item index.");
            return;
        }

        // Perform actions based on itemIndex
        switch (itemIndex)
        {
            case 0:
                item1Event?.Invoke();
                break;
            case 1:
                item2Event?.Invoke();
                break;
            case 2:
                item3Event?.Invoke();
                break;
            case 3:
                item4Event?.Invoke();
                break;
        }
    }

    // Method to notify the inventory system that an item is ready to be stored
    public void NotifyPickup(int itemIndex)
    {
        if (itemIndex < 0 || itemIndex >= items.Count)
        {
            UnityEngine.Debug.LogError("Invalid item index.");
            return;
        }

        ItemData newItemData = items[itemIndex];

        // Notify the Inventory Manager to add this item
        inventoryManager.AddItem(newItemData); // Ensure you have a reference to the Inventory Manager

        // Show the notification text UI for 1 second
        if (notificationTextUI != null)
        {
            StartCoroutine(ShowNotification());
        }

        // Perform any additional actions here if necessary
    }

    // Coroutine to show and hide the notification UI
    private IEnumerator ShowNotification()
    {
        notificationTextUI.SetActive(true);
        yield return new WaitForSeconds(1f);
        notificationTextUI.SetActive(false);
    }

    // Example of invoking an event from another script
    public void TriggerItemEvent(int itemIndex)
    {
        HandleItemEvent(itemIndex);
    }
}
