using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private int correctAnswers = 0;
    private int wrongAnswers = 0;
    
    // Dictionary to store correct answers per category
    private Dictionary<string, int> correctAnswersByCategory = new Dictionary<string, int>();
    // Dictionary to store wrong answers per category
    private Dictionary<string, int> wrongAnswersByCategory = new Dictionary<string, int>();

    [Header("UI Elements")]
    [SerializeField] private TMP_Text scoreText; // Reference to the TextMeshPro component
    [SerializeField] private GameObject missionCompleteScreen; // Reference to the mission complete screen
    [SerializeField] private TMP_Text categoryStatsText; // Reference to category stats text
    [SerializeField] private bool showDetailedStats = true; // Toggle to show/hide detailed category stats

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Debug.Log("ScoreManager instance assigned.");
        }
        else
        {
            Debug.LogWarning("Duplicate ScoreManager instance detected and destroyed.");
            Destroy(gameObject);
        }
        
        // Initialize category statistics if WordManager is available
        if (WordManager.instance != null)
        {
            foreach (string category in WordManager.instance.GetAllCategories())
            {
                correctAnswersByCategory[category] = 0;
                wrongAnswersByCategory[category] = 0;
            }
        }
        else
        {
            Debug.LogWarning("WordManager not found! Category stats may not be properly initialized.");
        }
    }

    // Call this method when an answer is correct
    public void RecordCorrectAnswer()
    {
        correctAnswers++;
        UpdateScoreDisplay();
    }

    // Call this method when an answer is correct (with category)
    public void RecordCorrectAnswer(string category)
    {
        correctAnswers++;
        
        // Add the category if it doesn't exist yet
        if (!correctAnswersByCategory.ContainsKey(category))
        {
            correctAnswersByCategory[category] = 0;
        }
        
        correctAnswersByCategory[category]++;
        UpdateScoreDisplay();
    }

    // Call this method when an answer is wrong
    public void RecordWrongAnswer()
    {
        wrongAnswers++;
        UpdateScoreDisplay();
    }

    // Call this method when an answer is wrong (with category)
    public void RecordWrongAnswer(string category)
    {
        wrongAnswers++;
        
        // Add the category if it doesn't exist yet
        if (!wrongAnswersByCategory.ContainsKey(category))
        {
            wrongAnswersByCategory[category] = 0;
        }
        
        wrongAnswersByCategory[category]++;
        UpdateScoreDisplay();
    }

    // Update the score display
    private void UpdateScoreDisplay()
    {
        StringBuilder scoreBuilder = new StringBuilder();
        
        // Add basic score information
        scoreBuilder.AppendLine($"Correct Answers: {correctAnswers}");
        scoreBuilder.AppendLine();
        scoreBuilder.AppendLine($"Wrong Answers: {wrongAnswers}");
        
        // Update the basic score text
        if (scoreText != null)
        {
            scoreText.text = scoreBuilder.ToString();
        }
        
        // Update category stats if the text component exists
        if (categoryStatsText != null && showDetailedStats)
        {
            UpdateCategoryStats();
        }
    }
    
    // Update category statistics display
    private void UpdateCategoryStats()
    {
        if (categoryStatsText == null) return;
        
        StringBuilder statsBuilder = new StringBuilder();
        statsBuilder.AppendLine("--- Category Statistics ---");
        
        // Sort categories by number of correct answers (descending)
        var sortedCategories = correctAnswersByCategory.OrderByDescending(c => c.Value)
                                                       .Select(c => c.Key)
                                                       .ToList();
        
        foreach (string category in sortedCategories)
        {
            int correct = correctAnswersByCategory.ContainsKey(category) ? correctAnswersByCategory[category] : 0;
            int wrong = wrongAnswersByCategory.ContainsKey(category) ? wrongAnswersByCategory[category] : 0;
            int total = correct + wrong;
            
            if (total > 0) // Only show categories with at least one attempt
            {
                float accuracy = total > 0 ? (float)correct / total * 100 : 0;
                statsBuilder.AppendLine($"{category}: {correct}/{total} ({accuracy:F0}%)");
            }
        }
        
        categoryStatsText.text = statsBuilder.ToString();
    }

    // Call this method to display the mission complete screen
    public void ShowMissionCompleteScreen()
    {
        if (missionCompleteScreen != null)
        {
            missionCompleteScreen.SetActive(true);
        }
        
        UpdateScoreDisplay(); // Ensure the score is updated on the screen
    }

    // Optionally, reset the scores
    public void ResetScores()
    {
        correctAnswers = 0;
        wrongAnswers = 0;
        
        // Reset category counters
        foreach (string category in correctAnswersByCategory.Keys.ToList())
        {
            correctAnswersByCategory[category] = 0;
        }
        
        foreach (string category in wrongAnswersByCategory.Keys.ToList())
        {
            wrongAnswersByCategory[category] = 0;
        }
        
        UpdateScoreDisplay();
    }
    
    // Get category statistics as a string (can be used for saving/loading)
    public string GetCategoryStatsString()
    {
        StringBuilder statsBuilder = new StringBuilder();
        
        foreach (string category in correctAnswersByCategory.Keys)
        {
            int correct = correctAnswersByCategory[category];
            int wrong = wrongAnswersByCategory.ContainsKey(category) ? wrongAnswersByCategory[category] : 0;
            int total = correct + wrong;
            
            if (total > 0)
            {
                float accuracy = (float)correct / total * 100;
                statsBuilder.AppendLine($"{category}: {correct}/{total} ({accuracy:F0}% accuracy)");
            }
        }
        
        return statsBuilder.ToString();
    }
}