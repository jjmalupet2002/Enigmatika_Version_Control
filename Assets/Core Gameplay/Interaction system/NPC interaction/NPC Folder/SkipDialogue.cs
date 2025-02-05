using UnityEngine;
using UnityEngine.UI;

public class SkipDialogue : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject dialoguePanel; // Assign the Dialogue Panel
    public GameObject optionPanel;   // Assign the Option Panel
    public Button skipButton;        // Assign the Skip Button

    void Start()
    {
        // Ensure the button is assigned and add a listener
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(Skip);
        }
    }

    void Skip()
    {
        // Disable both panels
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);
    }
}
