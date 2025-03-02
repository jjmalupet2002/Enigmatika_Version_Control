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

    public void playChOne()
    {
        UnityEngine.Debug.Log("Playing Chapter 1...");

        // **Always load Level 1 (Scene Index 2)**
        SceneManager.LoadSceneAsync(2);
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