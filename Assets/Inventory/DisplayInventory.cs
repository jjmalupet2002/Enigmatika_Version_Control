using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DisplayInventory : MonoBehaviour
{
    // Reference to the InventoryManager asset
    public InventoryManager inventoryManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) // Press 'I' to display inventory
        {
            ShowInventory(); // Call the display method
        }
    }

    // Method to display current inventory items in the console
    public void ShowInventory()
    {
        if (inventoryManager != null)
        {
            inventoryManager.DisplayInventory(); // Call the DisplayInventory method from InventoryManager
        }
        else
        {
            UnityEngine.Debug.LogWarning("InventoryManager reference is not assigned in DisplayInventory.");
        }
    }
}