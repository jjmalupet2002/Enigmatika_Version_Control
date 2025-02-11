using UnityEngine;
using CarterGames.Assets.SaveManager;
using System;
using System.Diagnostics;

public class SaveKeyLogger : MonoBehaviour
{
    private bool isLoading = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (!isLoading) // Prevent saving while loading
            {
                int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

                // Save the current scene index
                PlayerPrefs.SetInt("lastSavedScene", currentSceneIndex);
                PlayerPrefs.Save();
                SaveEvents.SaveGame();           
            }
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            isLoading = true; // Set flag to prevent saving
            SaveEvents.LoadGame();     

            // Reset after delay
            Invoke(nameof(ResetLoadingFlag), 2f);
        }
    }

    void ResetLoadingFlag()
    {
        isLoading = false;
    }
}

public static class SaveEvents
{
    public static event Action OnSaveGame;
    public static event Action OnLoadGame;

    public static void SaveGame()
    {
        UnityEngine.Debug.Log($"Game Saved: {new StackTrace().GetFrame(1).GetMethod().DeclaringType}");
        OnSaveGame?.Invoke();
    }

    public static void LoadGame()
    {
        UnityEngine.Debug.Log("Game Loaded!");
        OnLoadGame?.Invoke();
    }
}