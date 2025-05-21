using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver2 : MonoBehaviour
{
    public GameObject gameOverPanel; // Reference to the Game Over UI Panel
    public GameObject[] otherCanvases; // Array to store other UI Canvases

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

    public void ShowGameOver()
    {
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

        
    }
}