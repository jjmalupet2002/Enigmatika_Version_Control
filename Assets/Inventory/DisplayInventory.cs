using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Make sure to include this for UI elements

public class DisplayInventory : MonoBehaviour
{
    [Header("InventoryManager asset")]
    public InventoryManager inventoryManager;

    [Header("Item slot references")]
    public GameObject[] clueSlots; // Array for ClueSlots
    public GameObject[] generalSlots; // Array for GeneralSlots

    [Header("View panel UI references")]
    public Image itemIcon; // Reference to the ItemIcon image component
    public Text itemName; // Reference to the ItemName text component
    public Text itemDescription; // Reference to the ItemDescription text component

    [Header("Buttons and Capacity text")]
    public Button useItemButton; // Reference to the UseItem button
    public Button inspectItemButton; // Reference to the InspectItem button
    public Text capacityText; // Reference to the capacity text component

    private void OnEnable()
    {
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged += DisplayInventoryItems; // Subscribe to the event
        }
    }

    private void OnDisable()
    {
        if (inventoryManager != null)
        {
            inventoryManager.OnInventoryChanged -= DisplayInventoryItems; // Unsubscribe to avoid memory leaks
        }
    }

    private void Start()
    {
        // Initialize the inventory display on start
        DisplayInventoryItems(); // Automatically show the inventory items
    }

    // Method to display current inventory items in the UI
    public void DisplayInventoryItems()
    {
        // Clear previous item slots
        ClearItemSlots();

        // Update the capacity text
        capacityText.text = $"{inventoryManager.inventory.Count}"; // Only show the current count

        // Initialize counters for clue and general items
        int clueIndex = 0;
        int generalIndex = 0;

        // Iterate through the inventory and fill the item slots
        for (int i = 0; i < inventoryManager.inventory.Count; i++)
        {
            ItemData item = inventoryManager.inventory[i];

            // Determine where to place the item based on its type
            if (item.isClueItem && clueIndex < clueSlots.Length)
            {
                // Place the item in the corresponding clue slot
                clueSlots[clueIndex].SetActive(true);
                clueSlots[clueIndex].GetComponentInChildren<Image>().sprite = item.itemIcon; // Assign the item icon
                clueSlots[clueIndex].GetComponentInChildren<Button>().onClick.AddListener(() => SelectItem(item));

                // Increment the clue index for the next clue item
                clueIndex++;
            }
            else if (item.isGeneralItem && generalIndex < generalSlots.Length)
            {
                // Place the item in the corresponding general slot
                generalSlots[generalIndex].SetActive(true);
                generalSlots[generalIndex].GetComponentInChildren<Image>().sprite = item.itemIcon; // Assign the item icon
                generalSlots[generalIndex].GetComponentInChildren<Button>().onClick.AddListener(() => SelectItem(item));

                // Increment the general index for the next general item
                generalIndex++;
            }
        }
    }


    // Method to clear item slots before updating
    private void ClearItemSlots()
    {
        foreach (GameObject slot in clueSlots)
        {
            slot.SetActive(false);
            var image = slot.GetComponentInChildren<Image>();
            image.sprite = null; // Clear the icon
            image.color = Color.white; // Reset the color to white
        }

        foreach (GameObject slot in generalSlots)
        {
            slot.SetActive(false);
            var image = slot.GetComponentInChildren<Image>();
            image.sprite = null; // Clear the icon
            image.color = Color.white; // Reset the color to white
        }
    }


    // Method to select an item and display its details
    private void SelectItem(ItemData item)
    {
        // Display the item's details in the view panel
        itemIcon.sprite = item.itemIcon; // Set the item icon
        itemIcon.gameObject.SetActive(true); // Enable the item icon GameObject
        itemName.text = item.itemName; // Set the item name
        itemDescription.text = item.itemDescription; // Set the item description

        // Update button states based on item usability
        useItemButton.interactable = item.isUsable; // Enable or disable the use button
        inspectItemButton.interactable = true; // Enable inspect button
    }

    // Optional Debug Method: Call to display the inventory via InventoryManager
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
