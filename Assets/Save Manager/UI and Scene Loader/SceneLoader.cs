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
            SaveEvents.LoadGame();
        }
        else
        {
            UnityEngine.Debug.Log("Starting new game...");
            SaveManager.Clear();
        }

        yield return new WaitForSeconds(1f);

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }
}
