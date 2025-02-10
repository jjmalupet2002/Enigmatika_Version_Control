using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    public GameObject continueButton; // Assign in Inspector
    public static bool havePlayed = false;

    void Start()
    {
        // Check if there's a saved scene
        if (PlayerPrefs.HasKey("lastSavedScene"))
        {
            havePlayed = true;
            continueButton.SetActive(true); // Show continue button
        }
        else
        {
            continueButton.SetActive(false); // Hide if no save exists
        }
    }

    public void PlayGame()
    {
        // Set havePlayed to true
        havePlayed = true;
        PlayerPrefs.SetInt("havePlayed", 1);
        PlayerPrefs.Save();

        // Start a new game at Scene Index 1
        SceneManager.LoadSceneAsync(1);
    }

    public void ContinueGame()
    {
        // Load last saved scene
        int savedSceneIndex = PlayerPrefs.GetInt("lastSavedScene", 1); // Default to scene 1 if missing
        SceneManager.LoadSceneAsync(savedSceneIndex);
    }
}
