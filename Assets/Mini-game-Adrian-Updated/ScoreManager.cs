using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class ScoreManager : MonoBehaviour
{
    private int correctAnswers = 0;
    private int wrongAnswers = 0;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text scoreText; // Reference to the TextMeshPro component
    [SerializeField] private GameObject missionCompleteScreen; // Reference to the mission complete screen

    // Call this method when an answer is correct
    public void RecordCorrectAnswer()
    {
        correctAnswers++;
        UpdateScoreDisplay();
    }

    // Call this method when an answer is wrong
    public void RecordWrongAnswer()
    {
        wrongAnswers++;
        UpdateScoreDisplay();
    }

    // Update the score display
    private void UpdateScoreDisplay()
    {
        scoreText.text = $"Correct Answers: {correctAnswers}\n\n\nWrong Answers: {wrongAnswers}";
    }

    // Call this method to display the mission complete screen
    public void ShowMissionCompleteScreen()
    {
        missionCompleteScreen.SetActive(true);
        UpdateScoreDisplay(); // Ensure the score is updated on the screen
    }

    // Optionally, reset the scores
    public void ResetScores()
    {
        correctAnswers = 0;
        wrongAnswers = 0;
        UpdateScoreDisplay();
    }
}