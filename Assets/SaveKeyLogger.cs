using UnityEngine;
using CarterGames.Assets.SaveManager;
using System.Diagnostics;
using System;


public class SaveKeyLogger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveEvents.SaveGame(); // Trigger the save game event
            UnityEngine.Debug.Log("Game Saved!");
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            SaveEvents.LoadGame(); // Trigger the load game event
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
