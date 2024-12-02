using UnityEngine;
using UnityEngine.UI;

public class QuitGameHandler : MonoBehaviour
{
    // Public references
    public Button exitButton;
    public GameObject quitAlertUI;
    public Button noButton;

    void Start()
    {
        // Ensure the quit alert UI is initially hidden
        quitAlertUI.SetActive(false);

        // Add listeners for the buttons
        exitButton.onClick.AddListener(OnExitButtonPressed);
        noButton.onClick.AddListener(OnNoButtonPressed);
    }

    // Display the quit alert UI when the exit button is pressed
    void OnExitButtonPressed()
    {
        quitAlertUI.SetActive(true);
    }

    // Hide the quit alert UI when the no button is pressed
    void OnNoButtonPressed()
    {
        quitAlertUI.SetActive(false);
    }
}
