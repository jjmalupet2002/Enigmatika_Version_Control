using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using CarterGames.Assets.SaveManager;
using UnityEngine.SceneManagement;
using System.ComponentModel;

public class ChSelect : MonoBehaviour
{

    [Header("Main Menu UI")]
    [SerializeField] private GameObject chSelect;

    [Header("Loading Screens")]
    [SerializeField] private GameObject loadCh1;
    //[SerializeField] private GameObject loadCh2;

    public void playChOne()
    {
        // Hide chapter select then, show loading screen
        chSelect.SetActive(false);
        loadCh1.SetActive(true);

        // Check if we should load a saved game
        if (PlayerPrefs.GetInt("havePlayed", 0) == 1 || PlayerPrefs.HasKey("lastSavedScene"))
        {
            UnityEngine.Debug.Log("Continuing from last save...");
            SaveManager.Load(); // Load saved game state

            // Get last saved scene (default to Chapter 1 if missing)
            int lastScene = PlayerPrefs.GetInt("lastSavedScene", 2);

            // Load the last saved scene manually
            StartCoroutine(LoadLevelAsync(lastScene));
        }
        else
        {
            UnityEngine.Debug.Log("Starting a new game...");
            StartCoroutine(LoadLevelAsync(2)); // Load Chapter 1 normally
        }
    }

    //WIP: Chapter agnostic load code
    //public void playChX(int leveltoLoad)
    //{
    //    // Hide chapter select then, show loading screen
    //    chSelect.SetActive(false);
    //    loadCh1.SetActive(true); // WIP: figure out how to load specific loading screen

    //    // Go to Chapter X
    //    // Ensure index of the said scene in the Scene Manager is 2, else change the value below
    //    StartCoroutine(LoadLevelAsync(leveltoLoad));
    //}

    IEnumerator LoadLevelAsync(int leveltoLoad)
    {
        UnityEngine.AsyncOperation loadOperation = SceneManager.LoadSceneAsync(leveltoLoad);

        while (!loadOperation.isDone)
        {
            yield return null;
        }
    }

    public void mainMenu()
    {
        // Go back to Main Menu
        // Ensure index of the said scene in the Scene Manager is 0, else change the value below
        SceneManager.LoadSceneAsync(0);
    }
}