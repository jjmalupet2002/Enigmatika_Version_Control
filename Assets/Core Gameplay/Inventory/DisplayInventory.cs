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

    [Header("Inspect Item UI References")]
    public GameObject InspectItemUI; // Reference to the black background UI for inspections
    public Button ExitInspectButton; // Reference to the UseItem button



    private ItemData currentSelectedItem; // To store the currently selected item

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
        inspectItemButton.onClick.AddListener(InspectItem);
        ExitInspectButton.onClick.AddListener(ExitInspectItem);

        // Ensure the UseItem background is disabled at start
        useItemBackground.SetActive(false);

        // Subscribe to the OnItemDeleted event
        InventoryManager.Instance.OnItemDeleted += OnItemDeleted;
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
                var button = clueSlots[clueIndex].GetComponentInChildren<Button>();
                var image = clueSlots[clueIndex].GetComponentInChildren<Image>();
                image.sprite = item.itemIcon; // Assign the item icon

                if (item.isUsingItem)
                {
                    button.interactable = false; // Disable the button if the item is in use
                    image.color = Color.grey; // Grey out the icon
                }
                else
                {
                    button.interactable = true; // Enable the button if the item is not in use
                    image.color = Color.white; // Set the icon to white
                    button.onClick.AddListener(() => SelectItem(item));
                }

                // Increment the clue index for the next clue item
                clueIndex++;
            }
            else if (item.isGeneralItem && generalIndex < generalSlots.Length)
            {
                // Place the item in the corresponding general slot
                generalSlots[generalIndex].SetActive(true);
                var button = generalSlots[generalIndex].GetComponentInChildren<Button>();
                var image = generalSlots[generalIndex].GetComponentInChildren<Image>();
                image.sprite = item.itemIcon; // Assign the item icon

                if (item.isUsingItem)
                {
                    button.interactable = false; // Disable the button if the item is in use
                    image.color = Color.grey; // Grey out the icon
                }
                else
                {
                    button.interactable = true; // Enable the button if the item is not in use
                    image.color = Color.white; // Set the icon to white
                    button.onClick.AddListener(() => SelectItem(item)); // Ensure the SelectItem method is called
                }

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
        // Store the selected item in the currentSelectedItem field
        currentSelectedItem = item;

        // Display the item's details in the view panel
        itemIcon.sprite = item.itemIcon; // Set the item icon
        itemIcon.gameObject.SetActive(true); // Enable the item icon GameObject
        itemName.text = item.itemName; // Set the item name
        itemDescription.text = item.itemDescription; // Set the item description

        // Update button states based on item usability
        useItemButton.interactable = item.isUsable && !item.isUsingItem; // Enable or disable the use button
        inspectItemButton.interactable = true; // Enable inspect button

        // If the item is usable and not already in use, update the used item UI
        if (item.isUsable && !item.isUsingItem)
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

            // Grey out the item slot for the used item
            GreyOutItemSlot(item);

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

    private void GreyOutItemSlot(ItemData item)
    {
        // Find the corresponding slot and grey it out
        foreach (GameObject slot in clueSlots)
        {
            if (slot.activeSelf && slot.GetComponentInChildren<Image>().sprite == item.itemIcon)
            {
                var button = slot.GetComponentInChildren<Button>();
                var image = slot.GetComponentInChildren<Image>();
                button.interactable = false; // Disable the button
                image.color = Color.grey; // Grey out the icon
                return; // Exit the method once the item is found
            }
        }

        foreach (GameObject slot in generalSlots)
        {
            if (slot.activeSelf && slot.GetComponentInChildren<Image>().sprite == item.itemIcon)
            {
                var button = slot.GetComponentInChildren<Button>();
                var image = slot.GetComponentInChildren<Image>();
                button.interactable = false; // Disable the button
                image.color = Color.grey; // Grey out the icon
                return; // Exit the method once the item is found
            }
        }
    }

    private void RestoreItem()
    {
        // Logic to restore the used item
        ItemData currentItem = null;
        foreach (ItemData item in inventoryManager.inventory)
        {
            if (item.isUsingItem)
            {
                currentItem = item; // Found the currently used item
                break; // Exit the loop once we find it
            }
        }

        if (currentItem != null)
        {
            inventoryManager.RestoreItem(currentItem); // Restore the item via the inventory manager
            useItemButton.interactable = true; // Enable the use button if the item is restored

            // Hide the used item UI
            useItemIcon.gameObject.SetActive(false); // Hide the used item icon
            useItemText.text = ""; // Clear the used item name
            restoreItemButton.gameObject.SetActive(false); // Hide the restore button
            useItemBackground.SetActive(false); // Hide the UseItem background

            // Refresh the inventory display
            DisplayInventoryItems();
        }
    }

    // Method to handle item deletion
    private void OnItemDeleted(ItemData item)
    {
        // Hide the UI for the item that was deleted
        if (item.isUsingItem)
        {
            HideUseItemUI(); // A method to hide the UI when the item is deleted
        }

        // Reset the view panel UI after the item is deleted
        ResetItemViewPanel();
    }

    // Method to reset the item view panel
    private void ResetItemViewPanel()
    {
        // Clear the item icon, name, and description
        itemIcon.sprite = null; // Clear the item icon
        itemIcon.gameObject.SetActive(false); // Disable the item icon GameObject
        itemName.text = ""; // Clear the item name
        itemDescription.text = ""; // Clear the item description

        // Disable buttons
        useItemButton.interactable = false; // Disable the use button
        inspectItemButton.interactable = true; // Disable the inspect button
    }

    // Method to hide the used item UI
    private void HideUseItemUI()
    {
        useItemIcon.gameObject.SetActive(false); // Hide the used item icon
        useItemText.text = ""; // Clear the used item name
        restoreItemButton.gameObject.SetActive(false); // Hide the restore button
        useItemBackground.SetActive(false); // Hide the UseItem background
    }


    // Display the black background UI and the note UI for the selected item
    public void InspectItem()
    {
        if (currentSelectedItem != null && currentSelectedItem.noteUI != null)
        {
            // Hide any previously shown note UI first (if needed)
            foreach (var item in inventoryManager.inventory)
            {
                if (item.noteUI != null)
                    item.noteUI.SetActive(false); // Hide other noteUIs if necessary
            }

            // Show the specific note UI for the selected item
            currentSelectedItem.noteUI.SetActive(true);
            InspectItemUI.SetActive(true); // Show the black background

        }
    }

        // Method to exit the note UI when the Back button is pressed
        public void ExitInspectItem()
        {
            if (currentSelectedItem != null && currentSelectedItem.noteUI != null)
            {
                currentSelectedItem.noteUI.SetActive(false); // Hide the note UI
                InspectItemUI.SetActive(false); // Hide the black background
            }
        }
    }