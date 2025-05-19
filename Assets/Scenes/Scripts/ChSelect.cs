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
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject chSelect;

    [Header("Loading Screens")]
    [SerializeField] private GameObject[] loadingScreens; // Index 0 = Ch1, 1 = Ch2, etc.

    // Map chapter index (starting at 0) to scene index
    [SerializeField] private int[] sceneIndices = { 1, 3, 5 };

    public void PlayChapter(int chapter)
    {
        if (chapter < 0 || chapter >= loadingScreens.Length || chapter >= sceneIndices.Length)
        {
            UnityEngine.Debug.LogWarning("Invalid chapter selected.");
            return;
        }

        UnityEngine.Debug.Log($"Playing Chapter {chapter + 1}...");
        chSelect.SetActive(false);
        loadingScreens[chapter].SetActive(true);
        SceneManager.LoadSceneAsync(sceneIndices[chapter]);
    }

    public void MainMenu()
    {
        chSelect.SetActive(false);
        titleScreen.SetActive(true);
    }
}