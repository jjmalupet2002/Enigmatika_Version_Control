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
    public Button QuestButton;

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

        // Adjust joystick interactivity and alpha based on panel state
        SetJoystickInteractable(!isPanelActive);

        // Adjust button interactability and alpha based on panel state
        SetControlsInteractable(!isPanelActive);
    }

    private void SetJoystickInteractable(bool isInteractable)
    {
        if (joystickCanvasGroup != null)
        {
            joystickCanvasGroup.blocksRaycasts = isInteractable;
            joystickCanvasGroup.interactable = isInteractable;
            joystickCanvasGroup.alpha = isInteractable ? 1f : 0f; // Set alpha to 0 when inactive
        }
    }

    private void SetControlsInteractable(bool isInteractable)
    {
        // Adjust interactability and alpha for each button
        if (talkButton != null)
        {
            talkButton.interactable = isInteractable;
            SetButtonAlpha(talkButton, isInteractable ? 1f : 0f);
        }

        if (interactButton != null)
        {
            interactButton.interactable = isInteractable;
            SetButtonAlpha(interactButton, isInteractable ? 1f : 0f);
        }

        if (inventoryButton != null)
        {
            inventoryButton.interactable = isInteractable;
            SetButtonAlpha(inventoryButton, isInteractable ? 1f : 0f);
        }

        if (QuestButton != null)
        {
            QuestButton.interactable = isInteractable;
            SetButtonAlpha(QuestButton, isInteractable ? 1f : 0f);
        }
    }

    private void SetButtonAlpha(Button button, float alpha)
    {
        // Set the button's color to adjust its alpha
        Color color = button.GetComponent<UnityEngine.UI.Image>().color; // Specify UnityEngine.UI.Image
        color.a = alpha;
        button.GetComponent<UnityEngine.UI.Image>().color = color;
    }
}
