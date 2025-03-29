using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionComplete : MonoBehaviour
{
    public GameObject MissionPanel; // Reference to the Mission UI Panel
    public GameObject[] otherCanvases; // Array to store other UI Canvases

    void Start()
    {
        if (MissionPanel != null)
        {
            MissionPanel.SetActive(false); // Ensure it's disabled at start
        }
        else
        {
            Debug.LogError("MissionPanel is not assigned!");
        }
    }

    public void ShowMissionComplete()
    {
        // Show the game over panel
        MissionPanel.SetActive(true);

        // Disable all other canvases
        foreach (GameObject canvas in otherCanvases)
        {
            if (canvas != null)
            {
                canvas.SetActive(false);
            }
        }
    }
    public void MenuButton()
    {
        SceneManager.LoadScene("Main Menu");
    }

    
}
