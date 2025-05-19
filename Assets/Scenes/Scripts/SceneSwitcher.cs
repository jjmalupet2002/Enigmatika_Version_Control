using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Name of the scene to load
    public string testSceneName = "test scene"; // Ensure this matches the exact scene name

    void Update()
    {
        // Check for Ctrl + Shift + L key press
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.L))
                {
                    LoadTestScene();
                }
            }
        }
    }

    // Method to load the scene
    void LoadTestScene()
    {
        UnityEngine.Debug.Log("Loading scene: " + testSceneName);
        SceneManager.LoadScene(testSceneName);
    }
}
