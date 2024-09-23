using UnityEngine;
using UnityEngine.UI;

public class InventoryToggle : MonoBehaviour
{
    public Button InventoryButton;
    public Button talkButton;      // Reference to the Talk button
    public Button interactButton;  // Reference to the Interact button
    public AudioSource openSound;  // Assign the AudioSource for opening sound
    public AudioSource closeSound; // Assign the AudioSource for closing sound
    public CanvasGroup joystickCanvasGroup; // Reference to the CanvasGroup for the joystick

    private bool isInventoryOpen = false; // Track the state of the inventory

    private void Start()
    {
        // Set up the inventory button to open the inventory
        if (InventoryButton != null)
            InventoryButton.onClick.AddListener(OpenInventory);

        // Set up the interact button for its primary function
        if (interactButton != null)
            interactButton.onClick.AddListener(OnInteract);
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
        gameObject.SetActive(true);
        SetControlsInteractable(false);

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
        gameObject.SetActive(false);
        SetControlsInteractable(true);

        // Enable player movement and joystick input
        GameStateManager.Instance.SetPlayerMovementState(true);
        var playerJoystick = FindObjectOfType<PlayerJoystickControl>();
        if (playerJoystick != null)
            playerJoystick.SetInputEnabled(true);

        isInventoryOpen = false;
    }

    // Helper method to enable/disable interactivity
    private void SetControlsInteractable(bool isInteractable)
    {
        if (talkButton != null)
            talkButton.interactable = isInteractable;

        if (interactButton != null)
            interactButton.interactable = isInteractable;

        if (InventoryButton != null)
            InventoryButton.interactable = isInteractable;

        // Adjust joystick visibility and interactivity
        if (joystickCanvasGroup != null)
        {
            joystickCanvasGroup.blocksRaycasts = isInteractable;
            joystickCanvasGroup.interactable = isInteractable;
            joystickCanvasGroup.alpha = isInteractable ? 1f : 0.5f; // Adjust alpha as needed
        }
    }
}
