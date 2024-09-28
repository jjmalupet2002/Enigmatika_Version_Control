using UnityEngine;
using UnityEngine.UI;

public class InventoryToggle : MonoBehaviour
{
    public Button InventoryButton;
    public Button interactButton;  // Reference to the Interact button
    public AudioSource openSound;  // Assign the AudioSource for opening sound
    public AudioSource closeSound; // Assign the AudioSource for closing sound
    public GameObject joystickCanvasGroup; // Reference to the GameObject for the joystick canvas group
    public GameObject inventoryUI; // Reference to the actual Inventory UI

    private bool isInventoryOpen = false; // Track the state of the inventory
    private ItemInspectionManager itemInspectionManager;

    private void Start()
    {
        // Set up the inventory button to open the inventory
        if (InventoryButton != null)
            InventoryButton.onClick.AddListener(OpenInventory);

        // Set up the interact button for its primary function
        if (interactButton != null)
            interactButton.onClick.AddListener(OnInteract);

        // Find the ItemInspectionManager in the scene
        itemInspectionManager = FindObjectOfType<ItemInspectionManager>();
    }

    private void Update()
    {
        // Disable the inventory button if an item is being inspected
        if (itemInspectionManager != null)
        {
            InventoryButton.interactable = !itemInspectionManager.IsInspecting();
        }
    }


    private void OnInteract()
    {
        if (isInventoryOpen)
        {
            CloseInventory();
        }
        else
        {
            // Implement other interact button functionality here
        }
    }

    public void OpenInventory()
    {
        if (openSound != null)
            openSound.Play();
        inventoryUI.SetActive(true); // Show the inventory UI
        SetControlsActive(false);

        // Disable player movement and joystick input
        GameStateManager.Instance.SetPlayerMovementState(false);
        var playerJoystick = FindObjectOfType<PlayerJoystickControl>();
        if (playerJoystick != null)
            playerJoystick.SetInputEnabled(false);

        isInventoryOpen = true;
    }

    public void CloseInventory()
    {
        if (closeSound != null)
            closeSound.Play();

        Invoke("DeactivateInventory", closeSound.clip.length);
    }

    private void DeactivateInventory()
    {
        inventoryUI.SetActive(false); // Hide the inventory UI
        SetControlsActive(true);

        // Enable player movement and joystick input only if we're not in close-up view
        if (Camera.main != null && Camera.main.enabled)
        {
            GameStateManager.Instance.SetPlayerMovementState(true);
            var playerJoystick = FindObjectOfType<PlayerJoystickControl>();
            if (playerJoystick != null)
                playerJoystick.SetInputEnabled(true);
        }

        isInventoryOpen = false;
    }

    // Helper method to enable/disable controls
    private void SetControlsActive(bool isActive)
    {
        if (interactButton != null)
            interactButton.gameObject.SetActive(isActive);

        if (InventoryButton != null)
            InventoryButton.gameObject.SetActive(isActive);

        // Adjust joystick visibility
        if (joystickCanvasGroup != null)
            joystickCanvasGroup.SetActive(isActive);
    }

    // New method to enable or disable the inventory button
    public void SetInventoryButtonActive(bool isActive)
    {
        if (InventoryButton != null)
        {
            InventoryButton.gameObject.SetActive(isActive);
        }
    }

    // New method to check if the inventory is open
    public bool IsInventoryOpen()
    {
        return isInventoryOpen;
    }
}
