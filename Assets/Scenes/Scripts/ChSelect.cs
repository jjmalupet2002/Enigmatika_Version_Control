using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChSelect : MonoBehaviour
{
    [Header("Chapter Select")]
    [SerializeField] private GameObject chSelect;

    [Header("Loading Screens")]
    [SerializeField] private GameObject loadCh1;
    //[SerializeField] private GameObject loadCh2;

    public void playChOne()
    {
        // Hide chapter select then, show loading screen
        chSelect.SetActive(false);
        loadCh1.SetActive(true);

        // Go to Chapter 1
        // Ensure index of the said scene in the Scene Manager is 2, else change the value below
        StartCoroutine(LoadLevelAsync(2));
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
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(leveltoLoad);

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
