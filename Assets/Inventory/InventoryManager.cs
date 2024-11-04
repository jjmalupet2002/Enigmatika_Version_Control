using System.Collections.Generic;
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
        UnityEngine.Debug.Log($"Added item to inventory: {item.itemName}");

        // Trigger the inventory changed event
        OnInventoryChanged?.Invoke();
    }

    // Method to display current inventory items in the console
    public void DisplayInventory()
    {
        UnityEngine.Debug.Log("Current Inventory Items:");
        foreach (var item in inventory)
        {
            UnityEngine.Debug.Log($"Name: {item.itemName}, Description: {item.itemDescription}, Icon: {item.itemIcon}\nClue Item: {item.isClueItem}, General Item: {item.isGeneralItem}, Usable: {item.isUsable}");
        }
    }
}
