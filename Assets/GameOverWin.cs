// GameOver2.cs (Updated)
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWin : MonoBehaviour
{
    public GameObject gameOverPanel; // Reference to the Game Over UI Panel
    public GameObject[] otherCanvases; // Array to store other UI Canvases
    
    // New elements for win state
    [Header("Win State Elements")]
    public Text panelTitleText; // For changing between "Game Over" and "Victory!"
    public Text messageText; // For displaying a win/lose message
    public bool isWinScreen = false; // Flag to determine if showing as win screen

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false); // Ensure it's disabled at start
        }
        else
        {
            Debug.LogError("GameOverPanel is not assigned!");
        }
    }

    public void ShowGameOver(bool asWinScreen = false)
    {
        isWinScreen = asWinScreen;
        
        // Update UI elements if they exist
        if (panelTitleText != null)
        {
            panelTitleText.text = asWinScreen ? "Victory!" : "Game Over";
        }
        
        if (messageText != null)
        {
            messageText.text = asWinScreen ? 
                "Congratulations! You completed all rounds!" : 
                "Better luck next time!";
        }
        
        // Show the game over panel
        gameOverPanel.SetActive(true);

        // Disable all other canvases
        foreach (GameObject canvas in otherCanvases)
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
            }
        }
    }
    
    public void RestartButton()
    {
        SceneManager.LoadScene("minigame2");
    }
    
    public void ExitButton()
    {
        SceneManager.LoadScene("Main Menu");

        // If running in the editor, stop play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
