using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public static WordManager instance;

    [Header("Elements")]
    [SerializeField] private List<string> secretWords; // List of possible secret words
    private string secretWord; // The chosen secret word

    [Header("Questions/Hints")]
    [SerializeField] private Dictionary<string, string> wordToQuestionMap; // Maps words to questions

    private void Awake()
    {
        if (instance == null)
        {
            instance = this; // Assign the current instance
            Debug.Log("WordManager instance assigned.");
        }
        else
        {
            Debug.LogWarning("Duplicate WordManager instance detected and destroyed.");
            Destroy(gameObject); // Destroy duplicate instances
        }

        InitializeWordToQuestionMap(); // Initialize the word-to-question mapping
        ChooseRandomSecretWord(); // Choose secret word early
    }

    private void InitializeWordToQuestionMap()
    {
        wordToQuestionMap = new Dictionary<string, string>
    {
        { "BUTTERFLY", "What is often live in gardens, fields, and forests, and they can be found all over the world?" },
        { "ABHORRENT", "What describes actions, ideas, or behaviors that people strongly dislike or find morally wrong?" },
        { "ABILITIES", "What refers to the skills or talents that someone has?" },
        { "BLEACHERS", "What provides a place for spectators to sit and watch events like games, concerts, or performances." }
    };
    }

    public void ChooseRandomSecretWord()
    {
        if (secretWords == null || secretWords.Count == 0)
        {
            Debug.LogError("Secret word list is empty! Add words to the list in the Inspector.");
            return;
        }

        int randomIndex = Random.Range(0, secretWords.Count);
        secretWord = secretWords[randomIndex].ToUpper(); // Choose and convert to uppercase
        Debug.Log($"Random Secret Word Chosen: {secretWord}");
    }

    public string GetSecretWord()
    {
        Debug.Log($"GetSecretWord called. Current secret word: {secretWord}");
        return secretWord; // Return the selected secret word
    }

    public string GetQuestionForSecretWord()
    {
        if (wordToQuestionMap.ContainsKey(secretWord))
        {
            return wordToQuestionMap[secretWord];
        }
        else
        {
            Debug.LogWarning($"No question found for secret word: {secretWord}");
            return "No question available.";
        }
    }

    public bool IsInitialized()
    {
        bool initialized = !string.IsNullOrEmpty(secretWord);
        if (!initialized)
        {
            Debug.LogWarning("WordManager is not initialized. Secret word is missing.");
        }
        return initialized;
    }
}