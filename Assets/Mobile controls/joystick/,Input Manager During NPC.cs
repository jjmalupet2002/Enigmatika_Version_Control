using UnityEngine;
using UnityEngine.UI;

public class InputManagerDuringNPC : MonoBehaviour
{
    public GameObject dialoguePanel; // Reference to the dialogue panel
    public GameObject inventoryPanel; // Reference to the inventory panel
    public CanvasGroup joystickCanvasGroup; // Reference to the CanvasGroup component for the joystick

    public Button talkButton;      // Reference to the Talk button
    public Button interactButton;  // Reference to the Interact button
    public Button inventoryButton; // Reference to the Inventory button

    private PlayerJoystickControl playerJoystickControl;

    private void Start()
    {
        // Find the PlayerJoystickControl component on the player
        playerJoystickControl = FindObjectOfType<PlayerJoystickControl>();
    }

    private void Update()
    {
        // Check if either the dialogue panel or the inventory panel is active
        bool isPanelActive = dialoguePanel.activeInHierarchy || inventoryPanel.activeInHierarchy;

        // Enable or disable joystick input based on panel state
        if (playerJoystickControl != null)
        {
            playerJoystickControl.SetInputEnabled(!isPanelActive);
        }

        // Adjust joystick interactivity based on panel state
        SetJoystickInteractable(!isPanelActive);

        // Adjust button interactability based on panel state
        SetControlsInteractable(!isPanelActive);
    }

    private void SetJoystickInteractable(bool isInteractable)
    {
        if (joystickCanvasGroup != null)
        {
            joystickCanvasGroup.blocksRaycasts = isInteractable;
            joystickCanvasGroup.interactable = isInteractable;
            joystickCanvasGroup.alpha = isInteractable ? 1f : 0.5f; // Adjust alpha as needed
        }
    }

    private void SetControlsInteractable(bool isInteractable)
    {
        if (talkButton != null)
            talkButton.interactable = isInteractable;

        if (interactButton != null)
            interactButton.interactable = isInteractable;

        if (inventoryButton != null)
            inventoryButton.interactable = isInteractable;
    }
}