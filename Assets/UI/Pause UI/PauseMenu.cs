using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button pauseButton;
    public GameObject pauseUI;
    public Button resumeButton;
    public Button quitButton;
    public GameObject quitAlertUI;
    public Button yesButton;
    public Button noButton;
    public SaveUI saveUI;

    // Array to hold game objects to hide
    public GameObject[] gameObjectsToHide;

    private void Start()
    {
        // Initially deactivate Pause UI and Quit Alert UI
        pauseUI.SetActive(false);
        quitAlertUI.SetActive(false);

        // Add listeners to buttons
        pauseButton.onClick.AddListener(PauseGame);
        resumeButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(ShowQuitAlert);
        yesButton.onClick.AddListener(QuitToMainMenu);
        noButton.onClick.AddListener(CancelQuit);
    }

    private void PauseGame()
    {
        pauseUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game
        saveUI.OnPauseMenuOpened();

        // Hide specified game objects
        foreach (GameObject obj in gameObjectsToHide)
        {
            obj.SetActive(false);
        }
    }

    private void ResumeGame()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game

        // Show the hidden game objects again
        foreach (GameObject obj in gameObjectsToHide)
        {
            obj.SetActive(true);
        }
    }

    private void ShowQuitAlert()
    {
        quitAlertUI.SetActive(true);
    }

    private void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Ensure the game is not paused when loading the main menu
        SceneManager.LoadScene("Main Menu");
    }

    private void CancelQuit()
    {
        quitAlertUI.SetActive(false);
    }
}