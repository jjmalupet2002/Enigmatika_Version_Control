using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using Save;
using CarterGames.Assets.SaveManager;
using System.Diagnostics;

[CreateAssetMenu(fileName = "NewInventoryManager", menuName = "Inventory/Manager")]
public class InventoryManager : ScriptableObject
{
    public static InventoryManager Instance { get; private set; }

    public List<ItemData> inventory = new List<ItemData>(); // List to store item data
    public InventorySystem inventorySystem;

    // Event to notify when the inventory changes
    public delegate void InventoryChanged();
    public event InventoryChanged OnInventoryChanged;

    // Event to notify when an item is equipped/used
    public event Action<ItemData> OnItemUsed;

    // Event to notify when an item is deleted
    public event Action<ItemData> OnItemDeleted;

    // Add event listeners in OnEnable
    private void OnEnable()
    {
        // Ensure only one instance of the InventoryManager exists
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(this); // If another instance exists, destroy this one
        }
        else
        {
            Instance = this; // Assign this instance as the singleton
        }

        // Subscribe to the save and load events
        SaveEvents.OnSaveGame += SaveInventory;
        SaveEvents.OnLoadGame += LoadInventory;
    }

    // Remove event listeners in OnDisable to avoid memory leaks
    private void OnDisable()
    {
        // Unsubscribe from events
        SaveEvents.OnSaveGame -= SaveInventory;
        SaveEvents.OnLoadGame -= LoadInventory;
    }

    // Method to save the inventory data
    public void SaveInventory()
    {
        inventorySystem.itemNames.Clear(); // Clear the list before adding new data
        inventorySystem.itemIcons.Clear();
        inventorySystem.itemDescriptions.Clear();
        inventorySystem.isClueItems.Clear();
        inventorySystem.isGeneralItems.Clear();
        inventorySystem.isUsableItems.Clear();
        inventorySystem.isUsingItems.Clear();
        inventorySystem.keyIds.Clear();
        inventorySystem.isNotes.Clear();
        inventorySystem.noteUIs.Clear();
        inventorySystem.itemInspectionStatus.Clear();

        foreach (ItemData item in inventory)
        {
            inventorySystem.itemNames.Add(item.itemName); // Save item name
            inventorySystem.itemIcons.Add(item.itemIcon); // Save item icon
            inventorySystem.itemDescriptions.Add(item.itemDescription); // Save item description
            inventorySystem.isClueItems.Add(item.isClueItem); // Save clue item flag
            inventorySystem.isGeneralItems.Add(item.isGeneralItem); // Save general item flag
            inventorySystem.isUsableItems.Add(item.isUsable); // Save usability flag
            inventorySystem.isUsingItems.Add(item.isUsingItem); // Save usage status
            inventorySystem.keyIds.Add(item.keyId); // Save key ID
            inventorySystem.isNotes.Add(item.isNote); // Save note flag
            inventorySystem.noteUIs.Add(item.noteUI); // Save note UI object
            inventorySystem.itemInspectionStatus.Add(item.hasBeenInspected); // Save inspection status
        }

        // Call SaveManager to save the data
        SaveManager.Save(inventorySystem);
        UnityEngine.Debug.Log("Inventory Saved!");
    }

    // Method to load the inventory data
    public void LoadInventory()
    {
        // Load the InventorySystem asset from Resources folder
        InventorySystem loadedInventorySystem = Resources.Load<InventorySystem>("Save System/Enigmatika Save Game Classes/Inventory/InventorySystem");

        // Check if the loaded InventorySystem is valid
        if (loadedInventorySystem != null)
        {
            inventorySystem = loadedInventorySystem; // Assign it only if valid
            UnityEngine.Debug.Log("InventorySystem loaded successfully.");

            // Clear inventory before loading new data
            inventory.Clear();

            for (int i = 0; i < inventorySystem.itemNames.Count; i++)
            {
                string itemName = inventorySystem.itemNames[i];
                Sprite itemIcon = inventorySystem.itemIcons[i];
                string itemDescription = inventorySystem.itemDescriptions[i];
                bool isClueItem = inventorySystem.isClueItems[i];
                bool isGeneralItem = inventorySystem.isGeneralItems[i];
                bool isUsable = inventorySystem.isUsableItems[i];
                bool isUsingItem = inventorySystem.isUsingItems[i];
                string keyId = inventorySystem.keyIds[i];
                bool isNote = inventorySystem.isNotes[i];
                GameObject noteUI = inventorySystem.noteUIs[i];
                bool hasBeenInspected = inventorySystem.itemInspectionStatus[i];

                // Check if this item already exists and has been inspected
                bool alreadyInInventory = inventory.Any(item => item.itemName == itemName && item.hasBeenInspected);

                if (!alreadyInInventory)
                {
                    // Create a new item based on the loaded data
                    ItemData item = new ItemData(itemName, itemIcon, itemDescription, isClueItem, isGeneralItem, isUsable, isUsingItem, isNote, hasBeenInspected, noteUI, keyId);

                    // Add the item to the inventory
                    inventory.Add(item);

                    // If the item was inspected previously, mark it as inspected
                    if (hasBeenInspected)
                    {
                        InspectItem(item); // Mark the item as inspected in the inventory system
                    }
                }
                else
                {
                    UnityEngine.Debug.Log($"Skipped adding {itemName} as it was already inspected.");
                }
            }

            // Notify the system that inventory has changed
            OnInventoryChanged?.Invoke();
            UnityEngine.Debug.Log("Inventory Loaded!");
        }
        else
        {
            UnityEngine.Debug.LogWarning("Failed to load InventorySystem.");
        }
    }

    public void InspectItem(ItemData item)
    {
        item.hasBeenInspected = true; // Mark the item as inspected globally
        UnityEngine.Debug.Log($"Item {item.itemName} has been inspected.");
        OnInventoryChanged?.Invoke(); // Notify inventory change
    }

    // Add an item to the inventory
    public void AddItem(ItemData item)
    {
        inventory.Add(item);
        OnInventoryChanged?.Invoke(); // Notify that the inventory changed
    }

    // Equip/Use an item from the inventory (but not delete it)
    public void UseItem(ItemData item)
    {
        if (item.isUsable && !item.isUsingItem) // Check if the item is usable and not currently in use
        {
            // If another item is already being used, restore it first
            ItemData currentItem = inventory.FirstOrDefault(i => i.isUsingItem);
            if (currentItem != null)
            {
                RestoreItem(currentItem); // Restore the previously equipped item
            }

            item.isUsingItem = true; // Mark the item as in use (equipped)
            UnityEngine.Debug.Log($"Equipped item: {item.itemName}");

            // Trigger inventory update
            OnInventoryChanged?.Invoke();

            // If the item is a key (has a keyID), notify the listeners
            if (!string.IsNullOrEmpty(item.keyId)) // Check if the item has a keyID
            {
                UnityEngine.Debug.Log($"Item {item.itemName} is a key with ID: {item.keyId}");
                // Trigger the OnItemUsed event and pass the keyID to listeners (e.g., DoorObjectHandler)
                OnItemUsed?.Invoke(item);
            }
            else
            {
                // Handle non-key items as usual
                OnItemUsed?.Invoke(item);
            }
        }
        else if (item.isUsingItem)
        {
            UnityEngine.Debug.LogWarning($"Item {item.itemName} is already in use.");
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Item {item.itemName} cannot be used.");
        }
    }

    // Restore an item back to inventory (remove it from being used)
    public void RestoreItem(ItemData item)
    {
        if (item.isUsingItem)
        {
            item.isUsingItem = false; // Mark the item as no longer in use
            UnityEngine.Debug.Log($"Unequipped: {item.itemName}");
            OnInventoryChanged?.Invoke(); // Notify that the inventory changed
        }
        else
        {
            UnityEngine.Debug.LogWarning($"Item {item.itemName} is not currently being used.");
        }
    }

    // Delete an item from the inventory (after it is used up or consumed)
    public void DeleteItem(ItemData item)
    {
        // You can either delete used items or handle it differently
        inventory.Remove(item); // Remove the item from the inventory
        UnityEngine.Debug.Log($"Deleted item: {item.itemName}");

        // Trigger the inventory update after item deletion
        OnInventoryChanged?.Invoke();

        // Trigger the OnItemDeleted event to notify other systems
        OnItemDeleted?.Invoke(item);
    }

    // Display the current inventory
    public void DisplayInventory()
    {
        UnityEngine.Debug.Log("Current Inventory Items:");
        foreach (var item in inventory)
        {

            // Check if the NoteUI is stored in the inventory
            if (item.isNote && item.noteUI != null)
            {
                UnityEngine.Debug.Log($"Item {item.itemName} has a NoteUI stored.");
            }
            else if (item.isNote && item.noteUI == null)
            {
                UnityEngine.Debug.Log($"Item {item.itemName} is a note but has no NoteUI stored.");
            }
        }
    }
}
