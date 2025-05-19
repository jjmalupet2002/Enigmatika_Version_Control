using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Save;
using CarterGames.Assets.SaveManager;
using System.Diagnostics;

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

    [Header("Video Cutscene")]
    public RawImage videoRawImage;
    public VideoPlayer videoPlayer;

    [Header("Saving and Loading")]
    public bool IsTutorialFinished = false;
    public Tutorial_IntroQuestSaveObject tutorialSaveObject;

    // New public bool to allow skipping after the cutscene
    public bool skipTutorialAfterCutscene = false;

    private void OnEnable()
    {
        // Subscribe to the save and load events
        SaveEvents.OnSaveGame += SaveTutorialState;
        SaveEvents.OnLoadGame += LoadTutorialState;
    }

    private void OnDisable()
    {
        // Unsubscribe from the save and load events
        SaveEvents.OnSaveGame -= SaveTutorialState;
        SaveEvents.OnLoadGame -= LoadTutorialState;
    }

    private void Start()
    {
        if (IsTutorialFinished)
        {
            SkipTutorial();  // If the tutorial was already finished, skip it automatically
            return;
        }

        // Ensure UI states at start
        actualUI.SetActive(false);
        skipUI.SetActive(false);
        cutsceneUI.SetActive(true);
        page1.SetActive(true);
        page2.SetActive(false);
        StartCoroutine(FadeInImage(cutSceneImage1));

        // Play tutorial music if it is not null
        if (tutorialMusic != null)
        {
            tutorialMusic.Play();
        }

        // Add button listeners
        skipButton.onClick.AddListener(ShowSkipConfirmation);
        yesButton.onClick.AddListener(SkipTutorial);
        noButton.onClick.AddListener(CancelSkip);
        nextButton.onClick.AddListener(ShowPage2);
        endButton.onClick.AddListener(EndCutscene);
    }

    private void SkipTutorial()
    {
        tutorialMusic.Stop();
        actualUI.SetActive(false);
        skipUI.SetActive(false);
        cutsceneUI.SetActive(false);
        simpleTimer.StartTimer();
        hintManager.ResetHintPoints();
        IsTutorialFinished = true;
        SaveTutorialState();
        postTutorialAudio.Play();
    }

    private void SaveTutorialState()
    {
        tutorialSaveObject.isTutorialFinished.Value = IsTutorialFinished;
    }

    private void LoadTutorialState()
    {
        IsTutorialFinished = tutorialSaveObject.isTutorialFinished.Value;

        // If the tutorial is finished, skip the tutorial automatically
        if (IsTutorialFinished)
        {
            SkipTutorial();
        }
    }

    private void ShowPage2()
    {
        page1.SetActive(false);
        page2.SetActive(true);
        StartCoroutine(FadeInImage(cutSceneImage2));
    }

    private void ShowTutorial()
    {
        cutsceneUI.SetActive(false);
        actualUI.SetActive(true);
        StartCoroutine(PlayCutsceneVideo());
    }

    private void ShowSkipConfirmation()
    {
        skipUI.SetActive(true);
        skipButton.gameObject.SetActive(false);
    }

    private void CancelSkip()
    {
        skipUI.SetActive(false);
        skipButton.gameObject.SetActive(true);
    }

    private void EndCutscene()
    {
        if (skipTutorialAfterCutscene)
        {
            SkipTutorial(); // Skip the tutorial after the cutscene ends
        }
        else
        {
            ShowTutorial(); // Show the tutorial if the skip is not enabled
        }
    }

    private IEnumerator PlayCutsceneVideo()
    {
        if (videoPlayer != null && videoRawImage != null)
        {
            videoRawImage.gameObject.SetActive(true);
            videoPlayer.Play();
            yield return new WaitForSeconds(5f);
            videoPlayer.Stop();
            videoRawImage.gameObject.SetActive(false);
        }
    }

    private IEnumerator FadeInImage(Image image)
    {
        Color color = image.color;
        color.a = 0;
        image.color = color;

        while (color.a < 1)
        {
            color.a += Time.deltaTime / 1f;
            image.color = color;
            yield return null;
        }
    }
}
