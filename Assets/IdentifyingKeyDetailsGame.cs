using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IdentifyingKeyDetailsGame : MonoBehaviour
{
    [Header("Main UI References")]
    public GameObject scrollViewContent;
    public GameObject startExaminationBG; // <-- New reference
    public Button startClueExaminationButton;
    public Text selectedAnswerText; // <-- New field for displaying selected answer
    public GameObject scoringUI; // <-- New reference for scoring UI
    public Text scoreRemarkText; // <-- New reference for score remark
    public Text scoreText; // <-- New reference for displaying score

    [System.Serializable]
    public class KeyDetailEntry
    {
        public Button keyDetailButton;
        public string associatedWord;
        public string buttonAnswer; // "Correct" or "Wrong"
        public string question; // <-- Question field
        public GameObject feedbackUI;
        public string wrongFeedback;
        public string correctFeedback;
        public Text feedbackText;
        public Button correctButton;
        public Button wrongButton;
        public Button exitFeedbackUIButton;
        public bool hasBeenAnswered; // <-- Boolean to track if the entry has been answered
    }

    [Header("Key Detail Entries")]
    public List<KeyDetailEntry> keyDetails = new List<KeyDetailEntry>();

    private void Start()
    {
        if (startClueExaminationButton != null)
            startClueExaminationButton.onClick.AddListener(OnStartClueExamination);

        foreach (var entry in keyDetails)
        {
            entry.feedbackUI.SetActive(false);
            entry.keyDetailButton.onClick.AddListener(() => OnKeyDetailSelected(entry));
            entry.correctButton.onClick.AddListener(() => OnAnswerSelected(entry, "Correct"));
            entry.wrongButton.onClick.AddListener(() => OnAnswerSelected(entry, "Wrong"));
            entry.exitFeedbackUIButton.onClick.AddListener(() => entry.feedbackUI.SetActive(false));
        }
    }

    private void OnStartClueExamination()
    {
        scrollViewContent.SetActive(true);

        // Disable background and button
        if (startExaminationBG != null)
            startExaminationBG.SetActive(false);

        if (startClueExaminationButton != null)
            startClueExaminationButton.gameObject.SetActive(false);
    }

    private void OnKeyDetailSelected(KeyDetailEntry entry)
    {
        entry.feedbackUI.SetActive(true);
        entry.feedbackText.text = entry.question; // Display the question first
        selectedAnswerText.text = ""; // Clear previous selected answer
    }

    private void OnAnswerSelected(KeyDetailEntry entry, string selectedAnswer)
    {
        // Set the selected answer text based on the button pressed
        if (selectedAnswer == "Correct")
        {
            selectedAnswerText.text = "Selected Answer: Correct";
        }
        else
        {
            selectedAnswerText.text = "Selected Answer: Wrong";
        }

        // Display feedback based on the answer
        if (selectedAnswer == entry.buttonAnswer)
        {
            entry.feedbackText.text = entry.correctFeedback;
        }
        else
        {
            entry.feedbackText.text = entry.wrongFeedback;
        }

        // Disable the GameObject for the correct and wrong buttons
        entry.correctButton.gameObject.SetActive(false);
        entry.wrongButton.gameObject.SetActive(false);

        // Mark the entry as answered
        entry.hasBeenAnswered = true;

        StopAllCoroutines();
        StartCoroutine(AutoHideFeedback(entry));

        // Check if all entries have been answered
        CheckAllAnswered();
    }

    private IEnumerator AutoHideFeedback(KeyDetailEntry entry)
    {
        yield return new WaitForSeconds(10f);
        entry.feedbackUI.SetActive(false);
        selectedAnswerText.text = ""; // Clear previous selected answer
    }

    private void CheckAllAnswered()
    {
        // Check if all entries have been answered
        bool allAnswered = true;
        foreach (var entry in keyDetails)
        {
            if (!entry.hasBeenAnswered)
            {
                allAnswered = false;
                break;
            }
        }

        if (allAnswered)
        {
            DisplayScoreUI();
        }
    }

    private void DisplayScoreUI()
    {
        // Calculate score: Correct answers divided by total entries
        int correctAnswers = 0;
        foreach (var entry in keyDetails)
        {
            if (entry.buttonAnswer == "Correct" && entry.hasBeenAnswered)
            {
                correctAnswers++;
            }
        }

        // Display scoring UI
        scoringUI.SetActive(true);
        scoreRemarkText.text = "Well done!"; // Example score remark, you can adjust this based on score
        scoreText.text = $"Score: {correctAnswers}/{keyDetails.Count}";

        // Disable scroll view content and enable the background
        scrollViewContent.SetActive(false);
        startExaminationBG.SetActive(true);
    }
}
