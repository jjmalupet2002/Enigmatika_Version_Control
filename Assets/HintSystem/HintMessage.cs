using UnityEngine;
using UnityEngine.UI;

public class HintMessageUIManager : MonoBehaviour
{
    public Button hintButton; // Reference to the hint button
    public Button exitButton; // Reference to the exit button
    public GameObject hintMessageUI; // Reference to the hint message UI game object

    private void OnEnable()
    {
        // Add listeners to buttons
        hintButton.onClick.AddListener(DisplayHintMessage);
        exitButton.onClick.AddListener(HideHintMessage);
    }

    private void OnDisable()
    {
        // Remove listeners when the object is disabled
        hintButton.onClick.RemoveListener(DisplayHintMessage);
        exitButton.onClick.RemoveListener(HideHintMessage);
    }

    // Method to display the hint message UI
    private void DisplayHintMessage()
    {
        hintMessageUI.SetActive(true); // Activate the hint message UI
    }

    // Method to hide the hint message UI
    private void HideHintMessage()
    {
        hintMessageUI.SetActive(false); // Deactivate the hint message UI
    }
}
