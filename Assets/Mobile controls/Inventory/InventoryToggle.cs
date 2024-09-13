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

    private void Start()
    {
        // Set up the interact button (back button) to play the close sound
        if (interactButton != null)
            interactButton.onClick.AddListener(CloseInventory);

        // Set up the inventory button to open the inventory
        if (InventoryButton != null)
            InventoryButton.onClick.AddListener(OpenInventory);
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

        Debug.Log("Inventory opened. Player movement disabled.");
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

        Debug.Log("Inventory closed. Player movement enabled.");
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
