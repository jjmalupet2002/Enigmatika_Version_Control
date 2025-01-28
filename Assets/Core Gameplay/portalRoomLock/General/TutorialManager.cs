using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Settings")]
    public GameObject actualUI;
    public GameObject skipUI;
    public Button skipButton;
    public Button yesButton;
    public Button noButton;
    public AudioSource tutorialMusic;

    [Header("Cutscene Settings")]
    public GameObject cutsceneUI;
    public GameObject page1;
    public GameObject page2;
    public Button nextButton;
    public Button endButton;
    public Image cutSceneImage1;
    public Image cutSceneImage2;
    public SimpleTimer simpleTimer;

    [Header("Post-Tutorial Audio")]
    public AudioSource postTutorialAudio;
    public HintPointManager hintManager;

    void Start()
    {
        // Ensure the actual UI and skip UI are inactive at the start
        actualUI.SetActive(false);
        skipUI.SetActive(false);

        // Ensure the cutscene UI is active at the start and show the first page
        cutsceneUI.SetActive(true);
        page1.SetActive(true);
        page2.SetActive(false);
        StartCoroutine(FadeInImage(cutSceneImage1));

        // Play the tutorial music
        tutorialMusic.Play();

        // Add listeners to the buttons
        skipButton.onClick.AddListener(ShowSkipConfirmation);
        yesButton.onClick.AddListener(SkipTutorial);
        noButton.onClick.AddListener(CancelSkip);
        nextButton.onClick.AddListener(ShowPage2);
        endButton.onClick.AddListener(ShowTutorial);
    }

    void ShowPage2()
    {
        // Show the second page and hide the first page
        page1.SetActive(false);
        page2.SetActive(true);
        StartCoroutine(FadeInImage(cutSceneImage2));
    }

    void ShowTutorial()
    {
        // Hide the cutscene UI and show the actual tutorial UI
        cutsceneUI.SetActive(false);
        actualUI.SetActive(true);
    }

    void ShowSkipConfirmation()
    {
        // Show the skip confirmation UI and disable the skip button
        skipUI.SetActive(true);
        skipButton.gameObject.SetActive(false);
    }

    void SkipTutorial()
    {
        // Stop the music and deactivate all UIs
        tutorialMusic.Stop();
        actualUI.SetActive(false);
        skipUI.SetActive(false);
        cutsceneUI.SetActive(false);
        simpleTimer.StartTimer();
        hintManager.ResetHintPoints();

        // Play the post-tutorial audio
        postTutorialAudio.Play();
    }

    void CancelSkip()
    {
        // Hide the skip confirmation UI and re-enable the skip button
        skipUI.SetActive(false);
        skipButton.gameObject.SetActive(true);
    }

    IEnumerator FadeInImage(Image image)
    {
        Color color = image.color;
        color.a = 0;
        image.color = color;

        while (color.a < 1)
        {
            color.a += Time.deltaTime / 1f; // Adjust the duration of the fade-in effect as needed
            image.color = color;
            yield return null;
        }
    }
}
