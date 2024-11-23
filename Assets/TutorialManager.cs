using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public GameObject actualUI;
    public GameObject skipUI;
    public Button skipButton;
    public Button yesButton;
    public Button noButton;
    public AudioSource tutorialMusic;

    void Start()
    {
        // Ensure the actual UI is active and the skip UI is inactive at the start
        actualUI.SetActive(true);
        skipUI.SetActive(false);
        tutorialMusic.Play();

        // Add listeners to the buttons
        skipButton.onClick.AddListener(ShowSkipConfirmation);
        yesButton.onClick.AddListener(SkipTutorial);
        noButton.onClick.AddListener(CancelSkip);
    }

    void ShowSkipConfirmation()
    {
        // Show the skip confirmation UI and disable the skip button
        skipUI.SetActive(true);
        skipButton.gameObject.SetActive(false);
    }

    void SkipTutorial()
    {
        // Stop the music and deactivate both UIs
        tutorialMusic.Stop();
        actualUI.SetActive(false);
        skipUI.SetActive(false);
    }

    void CancelSkip()
    {
        // Hide the skip confirmation UI and re-enable the skip button
        skipUI.SetActive(false);
        skipButton.gameObject.SetActive(true);
    }
}
