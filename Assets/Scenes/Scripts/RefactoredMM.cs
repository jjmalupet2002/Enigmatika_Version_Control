using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public bool havePlayed = false; // Boolean variable
    public GameObject playIndicator; // Assign the GameObject in the Inspector

    void Update()
    {
        if (havePlayed)
        {
            playIndicator.SetActive(true); // Make the GameObject visible
        }
    }

    [Header("Main Menu UI")]
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject chSelect;

    [Header("Loading Screens")]
    [SerializeField] private GameObject loadCh1;
    //[SerializeField] private GameObject loadCh2;

    public void ChapterSelect()
    {
        // Set havePlayed to true when the game starts. Just a test. In actual implementation, modify to actually detect for a save file.
        havePlayed = true;

        // Disable title screen and enable chapter select
        titleScreen.SetActive(false);
        chSelect.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

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
        // Hide chapter select then, show main menu
        chSelect.SetActive(false);
        titleScreen.SetActive(true);
    }
}
