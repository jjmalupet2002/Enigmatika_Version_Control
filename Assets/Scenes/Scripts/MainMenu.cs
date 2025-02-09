using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    public bool havePlayed = false; // Boolean variable
    public GameObject playIndicator; // Assign the GameObject in the Inspector

    void Start()
    {
        if (havePlayed)
        {
            playIndicator.SetActive(true); // Make the GameObject visible
        }
    }

    public void PlayGame()
    {
        // Set havePlayed to true when the game starts
        havePlayed = true;

        // Ensure index of the said scene in the Scene Manager is 1, else change the value below
        SceneManager.LoadSceneAsync(1);
    }

    //public void ExitGame()
    //{
    //    Application.Quit();
    //}
}
