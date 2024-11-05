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

    [Header("Used Item UI References")]
    public Image useItemIcon; // Reference to the Image for the used item icon
    public Text useItemText; // Reference to the Text for the used item name
    public Button restoreItemButton; // Reference to the Restore Item button
    public GameObject useItemBackground; // Reference to the UseItem background

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
        restoreItemButton.onClick.AddListener(RestoreItem);

        // Ensure the UseItem background is disabled at start
        useItemBackground.SetActive(false);
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

        // If the item is usable, update the used item UI
        if (item.isUsable)
        {
            useItemButton.onClick.AddListener(() => UseItem(item));
        }
    }

    private void UseItem(ItemData item)
    {
        // Check if the item can be used and is not already in use
        if (item.isUsable && !item.isUsingItem) // Ensure the item is usable and not already in use
        {
            // Restore any currently used item before using the new one
            ItemData currentItem = null;
            foreach (ItemData invItem in inventoryManager.inventory)
            {
                if (invItem.isUsingItem)
                {
                    currentItem = invItem; // Found the currently used item
                    break; // Exit the loop once we find it
                }
            }

            if (currentItem != null)
            {
                inventoryManager.RestoreItem(currentItem); // Restore the currently used item
            }

            // Use the new item via the inventory manager
            inventoryManager.UseItem(item);
            useItemIcon.sprite = item.itemIcon; // Set the used item icon
            useItemText.text = item.itemName; // Set the used item name
            useItemIcon.gameObject.SetActive(true); // Show the used item icon
            restoreItemButton.gameObject.SetActive(true); // Show the restore button
            useItemButton.interactable = false; // Disable the use button for the current item
            useItemBackground.SetActive(true); // Show the UseItem background

            // Close the inventory
            InventoryToggle inventoryToggle = FindObjectOfType<InventoryToggle>(); // Find the InventoryToggle script instance
            if (inventoryToggle != null)
            {
                inventoryToggle.CloseInventory(); // Call the method to close the inventory
            }
        }
        else if (item.isUsingItem)
        {
            // If the item is already in use, disable the use button
            useItemButton.interactable = false; // Disable the use button if the item is already used
        }

        // Ensure the inspect button is always enabled
        inspectItemButton.interactable = true; // Enable inspect button
    }



    public void RestoreItem()
    {
        // Assuming the inventory manager handles restoring the item
        if (useItemText.text != "") // Check if there's an item being used
        {
            ItemData usedItem = inventoryManager.inventory.Find(item => item.itemName == useItemText.text);
            if (usedItem != null)
            {
                inventoryManager.RestoreItem(usedItem); // Restore the item in the inventory
                useItemIcon.gameObject.SetActive(false); // Hide the used item icon
                restoreItemButton.gameObject.SetActive(false); // Hide the restore button
                useItemButton.interactable = true; // Re-enable the use button
                useItemBackground.SetActive(false); // Hide the UseItem background after restoring                                   
                useItemText.text = ""; // Set the used item text to an empty string
            }
        }
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
