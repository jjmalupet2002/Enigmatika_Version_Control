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

    public void playChapterTwo()
    {
        UnityEngine.Debug.Log("Playing Chapter 2...");
        SceneManager.LoadSceneAsync(4); // Loads Chapter 2 (Scene Index 3)
    }

    public void playChapterThree()
    {
        UnityEngine.Debug.Log("Playing Chapter 3...");
        SceneManager.LoadSceneAsync(6); // Loads Chapter 2 (Scene Index 3)
    }

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