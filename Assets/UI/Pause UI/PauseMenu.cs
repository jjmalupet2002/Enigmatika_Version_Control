using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour

{
    [Header("Word Container")]
    [SerializeField] private GameObject WordContainer; // Assign the Word Container GameObject

    public Button pauseButton;
    public GameObject pauseUI;
    public Button resumeButton;
    public Button quitButton;
    public GameObject quitAlertUI;
    public Button yesButton;
    public Button noButton;
    public SaveUI saveUI;

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

        // Disable the word container during top-down camera
        if (WordContainer != null)
        {
            WordContainer.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Word Container is not assigned!");
        }
    }

    private void ResumeGame()
    {
        pauseUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game

        // enable the word container during top-down camera
        if (WordContainer != null)
        {
            WordContainer.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Word Container is not assigned!");
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
