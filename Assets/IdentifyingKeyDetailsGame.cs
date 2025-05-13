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

    [System.Serializable]
    public class KeyDetailEntry
    {
        public Button keyDetailButton;
        public string associatedWord;
        public string buttonAnswer; // "Correct" or "Wrong"
        public string question; // <-- New field for the question
        public GameObject feedbackUI;
        public string wrongFeedback;
        public string correctFeedback;
        public Text feedbackText;
        public Button correctButton;
        public Button wrongButton;
        public Button exitFeedbackUIButton;
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
    }

    private void OnAnswerSelected(KeyDetailEntry entry, string selectedAnswer)
    {
        if (selectedAnswer == entry.buttonAnswer)
        {
            entry.feedbackText.text = entry.correctFeedback;
        }
        else
        {
            entry.feedbackText.text = entry.wrongFeedback;
        }

        StopAllCoroutines();
        StartCoroutine(AutoHideFeedback(entry));
    }

    private IEnumerator AutoHideFeedback(KeyDetailEntry entry)
    {
        yield return new WaitForSeconds(10f);
        entry.feedbackUI.SetActive(false);
    }
}
