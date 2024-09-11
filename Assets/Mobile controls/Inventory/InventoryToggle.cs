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
    }

    // Method to open the inventory
    public void OpenInventory()
    {
        if (openSound != null)
            openSound.Play();
        gameObject.SetActive(true);
        SetControlsInteractable(false);

        // Disable player movement
        GameStateManager.Instance.SetPlayerMovementState(false);
    }

    // Method to close the inventory
    public void CloseInventory()
    {
        // Play the close sound
        if (closeSound != null)
            closeSound.Play();

        // Invoke the deactivation after a slight delay to ensure the sound plays
        Invoke("DeactivateInventory", closeSound.clip.length);
    }

    // Helper method to deactivate the inventory UI
    private void DeactivateInventory()
    {
        gameObject.SetActive(false);
        SetControlsInteractable(true);

        // Enable player movement
        GameStateManager.Instance.SetPlayerMovementState(true);
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
