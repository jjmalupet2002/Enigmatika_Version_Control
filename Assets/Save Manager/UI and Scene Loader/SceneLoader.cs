using System.Collections;
using UnityEngine;
using CarterGames.Assets.SaveManager;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;

    void Start()
    {
        StartCoroutine(LoadGameData());
    }

    IEnumerator LoadGameData()
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }

        yield return new WaitForSeconds(1f); // Smooth transition

        // Check if we should load saved game data
        if (PlayerPrefs.GetInt("loadGame", 0) == 1)
        {
            UnityEngine.Debug.Log("Loading saved game data...");

            // Wait until the loading is done
            yield return StartCoroutine(LoadSavedGameData());
        }
        else
        {
            UnityEngine.Debug.Log("Starting new game...");
            SaveManager.Clear();
        }

        yield return new WaitForSeconds(1f);

        if (loadingScreen != null)
        {
            UnityEngine.Debug.Log("Disabling loading screen...");
            loadingScreen.SetActive(false);
        }
    }

    IEnumerator LoadSavedGameData()
    {
        SaveEvents.LoadGame();
        yield return null; // Ensures coroutine continues after LoadGame() runs
    }
}
