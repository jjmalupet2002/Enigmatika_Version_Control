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
        { "AUDACIOUS", "What word describes someone who is bold, daring, or willing to take risks?" },
        { "PARAMOUNT", "What term means something of the highest importance or significance?" },
        { "TREASURES", "What word refers to valuable items like gold, jewels, or other prized possessions?" },
        { "STANDOFFS", "What term describes a situation where two opposing forces are locked in a tense confrontation?" },
        { "NOTORIOUS", "What word describes someone or something that is widely known for being bad or infamous?" },
        { "COMMANDED", "What word means to give an official order or directive with authority?" },
        { "STRATEGIC", "What term describes actions or plans that are carefully designed to achieve a specific goal?" },
        { "COMMENCED", "What word means to begin or start something, like a project or event?" },
        { "RECOVERED", "What term means to retrieve something that was lost or stolen?" },
        { "INFORMANT", "What word describes someone who provides information, often secretly, to help solve a problem?" },
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