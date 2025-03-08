using UnityEngine;
using UnityEngine.UI;

public class QuestionDisplay : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Text questionText; // Reference to the UI Text element

    private WordManager wordManager;

    private void Update()
    {
        wordManager = WordManager.instance;

        if (wordManager == null)
        {
            Debug.LogError("WordManager instance not found!");
            return;
        }

        DisplayQuestion();
    }

    private void DisplayQuestion()
    {
        if (wordManager.IsInitialized())
        {
            string question = wordManager.GetQuestionForSecretWord();
            questionText.text = $"Question: {question}";
        }
        else
        {
            questionText.text = "Question not available.";
        }
    }
}