using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewInventoryManager", menuName = "Inventory/Manager")]
public class InventoryManager : ScriptableObject
{
    public static InventoryManager Instance { get; private set; }

    public List<ItemData> inventory = new List<ItemData>(); // List to store item data

    // Event to notify when the inventory changes
    public delegate void InventoryChanged();
    public event InventoryChanged OnInventoryChanged;

    // Event to notify when an item is equipped/used
    public event Action<ItemData> OnItemUsed;

    // Event to notify when an item is deleted
    public event Action<ItemData> OnItemDeleted;

    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(this);
        }
        else
        {
            Instance = this;
        }
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

            // Trigger the OnItemUsed event
            OnItemUsed?.Invoke(item);
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
        if (item.isUsingItem) // Prevent deletion if the item is currently in use
        {
            UnityEngine.Debug.LogWarning($"Item {item.itemName} cannot be deleted because it is in use.");
            return; // Don't delete if it's in use
        }

        inventory.Remove(item); // Remove the item from the inventory
        UnityEngine.Debug.Log($"Deleted item: {item.itemName}");

        // Notify that the inventory changed
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
            UnityEngine.Debug.Log($"Name: {item.itemName}, Description: {item.itemDescription}, Icon: {item.itemIcon}\nClue Item: {item.isClueItem}, General Item: {item.isGeneralItem}, Usable: {item.isUsable}, Using: {item.isUsingItem}");
        }
    }
}
