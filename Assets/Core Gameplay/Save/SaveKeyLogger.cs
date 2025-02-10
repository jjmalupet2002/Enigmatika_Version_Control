using UnityEngine;
using CarterGames.Assets.SaveManager;
using System;
using System.Diagnostics;

public class SaveKeyLogger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            int currentSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;

            // Save the current scene index
            PlayerPrefs.SetInt("lastSavedScene", currentSceneIndex);
            PlayerPrefs.Save();

            SaveEvents.SaveGame(); // Trigger save event
            UnityEngine.Debug.Log("Game Saved at Scene Index: " + currentSceneIndex);
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            SaveEvents.LoadGame(); // Trigger load event
            UnityEngine.Debug.Log("Game Loaded!");
        }
    }
}

public static class SaveEvents
{
    public static event Action OnSaveGame;
    public static event Action OnLoadGame;

    public static void SaveGame() => OnSaveGame?.Invoke();
    public static void LoadGame() => OnLoadGame?.Invoke();
}
