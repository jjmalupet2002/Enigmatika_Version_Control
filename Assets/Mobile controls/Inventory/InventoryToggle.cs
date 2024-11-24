using UnityEngine;
using UnityEngine.UI;

public class InventoryToggle : MonoBehaviour
{
    public Button InventoryButton;
    public Button TalkButton;  // Add TalkButton as a public variable
    public Button InteractButton;  // Add InteractButton as a public variable
    public AudioSource openSound;  // Assign the AudioSource for opening sound
    public AudioSource closeSound; // Assign the AudioSource for closing sound
    public GameObject joystickCanvasGroup; // Reference to the GameObject for the joystick canvas group
    public GameObject inventoryUI; // Reference to the actual Inventory UI
    public GameObject CriteriaText;
    public GameObject ObjectiveText;

    private bool isInventoryOpen = false; // Track the state of the inventory
    private TalkandInteract talkandInteract; // Reference to TalkandInteract script
    private Button activeButton; // Track the currently active button (Talk or Interact)

    private void Start()
    {
        // Set up the inventory button to open the inventory
        if (InventoryButton != null)
            InventoryButton.onClick.AddListener(OpenInventory);

        // Find the TalkandInteract script in the scene
        talkandInteract = FindObjectOfType<TalkandInteract>();
    }

    private void Update()
    {
        // Optional: Disable the inventory button if necessary
        // InventoryButton.interactable = true;
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
        CriteriaText.gameObject.SetActive(true);
        ObjectiveText.gameObject.SetActive(true);

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
        if (joystickCanvasGroup != null)
            joystickCanvasGroup.SetActive(isActive);

        if (TalkButton != null && InteractButton != null)
        {
            if (isActive)
            {
                // Enable the previously active button when exiting inventory
                if (activeButton != null)
                {
                    activeButton.gameObject.SetActive(true);
                   
                }
            }
            else
            {
                // Track which button is active before opening the inventory
                if (TalkButton.gameObject.activeSelf)
                {
                    activeButton = TalkButton;
                }
                else if (InteractButton.gameObject.activeSelf)
                {
                    activeButton = InteractButton;
                }

                TalkButton.gameObject.SetActive(false);
                InteractButton.gameObject.SetActive(false);
                CriteriaText.gameObject.SetActive(false);
                ObjectiveText.gameObject.SetActive(false);
            }
        }

        if (InventoryButton != null)
            InventoryButton.gameObject.SetActive(isActive);
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