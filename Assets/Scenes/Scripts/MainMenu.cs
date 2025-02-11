using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for Button component
using CarterGames.Assets.SaveManager;

public class Play : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button newGameButton;   // Assign in Inspector
    public Button continueButton;  // Assign in Inspector

    void Start()
    {
        // Assign button click listeners
        if (newGameButton != null)
            newGameButton.onClick.AddListener(PlayGame);

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(ContinueGame);
            UpdateContinueButton();
        }
    }

    void Update()
    {
        // Press 'R' to reset PlayerPrefs (Fresh Start)
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetGameProgress();
        }
    }

    public void PlayGame()
    {
        // Mark as a new game (DO NOT load saved data)
        PlayerPrefs.SetInt("loadGame", 0);
        PlayerPrefs.SetInt("havePlayed", 1);
        PlayerPrefs.Save();

        // Load the first chapter (Scene index 1)
        SceneManager.LoadSceneAsync(1);
    }

    public void ContinueGame()
    {
        // Mark as a continued game (DO load saved data)
        PlayerPrefs.SetInt("loadGame", 1);
        PlayerPrefs.Save();

        // Load last saved scene
        int savedSceneIndex = PlayerPrefs.GetInt("lastSavedScene", 1);
        SceneManager.LoadSceneAsync(savedSceneIndex);
    }

    void ResetGameProgress()
    {
        PlayerPrefs.DeleteKey("lastSavedScene"); // Remove saved scene
        PlayerPrefs.DeleteKey("havePlayed");     // Reset havePlayed status
        PlayerPrefs.DeleteKey("loadGame");       // Reset load flag
        PlayerPrefs.Save();
        UnityEngine.Debug.Log("Game progress reset!");

        // Hide the Continue button
        UpdateContinueButton();
    }

    void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(PlayerPrefs.HasKey("lastSavedScene"));
        }
    }
}
