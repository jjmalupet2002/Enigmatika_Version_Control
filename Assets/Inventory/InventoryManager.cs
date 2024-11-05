using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "NewInventoryManager", menuName = "Inventory/Manager")]
public class InventoryManager : ScriptableObject
{
    public static InventoryManager Instance { get; private set; }

    public List<ItemData> inventory = new List<ItemData>(); // List to store item data

    // Define an event to notify when the inventory changes
    public delegate void InventoryChanged();
    public event InventoryChanged OnInventoryChanged;

    private void OnEnable()
    {
        // Ensure only one instance of the InventoryManager exists
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Method to add an item to the inventory
    public void AddItem(ItemData item)
    {
        inventory.Add(item);

        // Trigger the inventory changed event
        OnInventoryChanged?.Invoke();
    }

    // Method to use an item from the inventory
    public void UseItem(ItemData item)
    {
        if (item.isUsable && !item.isUsingItem) // Check if the item is usable and not currently in use
        {
            // Find the currently used item, if any
            ItemData currentItem = inventory.FirstOrDefault(i => i.isUsingItem);
            if (currentItem != null)
            {
                RestoreItem(currentItem); // Restore the currently used item before using the new one
            }

            item.isUsingItem = true; // Set the new item as currently being used
            UnityEngine.Debug.Log($"Using item: {item.itemName}");

            // Trigger the inventory changed event
            OnInventoryChanged?.Invoke();
        }
        else if (item.isUsingItem)
        {
            UnityEngine.Debug.LogWarning($"Item {item.itemName} is already in use and cannot be used again.");
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Item {item.itemName} cannot be used.");
        }
    }

    // Method to restore an item back to inventory
    public void RestoreItem(ItemData item)
    {
        if (item.isUsingItem) // Check if the item is currently in use
        {
            item.isUsingItem = false; // Set the item as not being used
            UnityEngine.Debug.Log($"Restored item: {item.itemName}");

            // Trigger the inventory changed event
            OnInventoryChanged?.Invoke();
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Item {item.itemName} is not currently being used.");
        }
    }





    // Method to display current inventory items in the console
    public void DisplayInventory()
    {
        UnityEngine.Debug.Log("Current Inventory Items:");
        foreach (var item in inventory)
        {
            UnityEngine.Debug.Log($"Name: {item.itemName}, Description: {item.itemDescription}, Icon: {item.itemIcon}\nClue Item: {item.isClueItem}, General Item: {item.isGeneralItem}, Usable: {item.isUsable}, Using: {item.isUsingItem}");
        }
    }
}
