using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public static WordManager instance;

    [Header("Elements")]
    [SerializeField] private List<string> secretWords; // List of possible secret words
    private string secretWord; // The chosen secret word

    [Header("Questions/Hints")]
    [SerializeField] public Dictionary<string, WordQuestionCategory> wordToQuestionMap; // Maps words to questions and categories



    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (instance == null)
        {
            instance = this; // Assign the current instance
            Debug.Log("WordManager instance assigned.");
        }
        else
        {
            Debug.LogWarning("Duplicate WordManager instance detected and destroyed.");
            Destroy(gameObject); // Destroy duplicate instances
            return; // Exit to prevent further execution
        }

        InitializeWordToQuestionMap(); // Initialize the word-to-question mapping
        ChooseRandomSecretWord(); // Choose a random secret word
    }
    
    [System.Serializable]
    public class WordQuestionCategory
    {
        public string Question;
        public string Category;
        

        public WordQuestionCategory(string question, string category)
        {
            Question = question;
            Category = category;
        }
    }


    private void InitializeWordToQuestionMap()
    {
        // Initialize the dictionary with word-to-question mappings
        wordToQuestionMap = new Dictionary<string, WordQuestionCategory>
        {
            { "AUDACIOUS", new WordQuestionCategory("What word describes someone who is bold, daring, or willing to take risks?", "focus") },
            { "PARAMOUNT", new WordQuestionCategory("What term means something of the highest importance or significance?", "focus") },
            { "TREASURES", new WordQuestionCategory("What word refers to valuable items like gold, jewels, or other prized possessions?", "focus") },
            { "STANDOFFS", new WordQuestionCategory("What term describes a situation where two opposing forces are locked in a tense confrontation?", "focus") },
            { "NOTORIOUS", new WordQuestionCategory("What word describes someone or something that is widely known for being bad or infamous?", "focus") },
            { "COMMANDED", new WordQuestionCategory("What word means to give an official order or directive with authority?", "focus") },
            { "STRATEGIC", new WordQuestionCategory("What term describes actions or plans that are carefully designed to achieve a specific goal?", "focus") },
            { "COMMENCED", new WordQuestionCategory("What word means to begin or start something, like a project or event?", "focus") },
            { "RECOVERED", new WordQuestionCategory("What term means to retrieve something that was lost or stolen?", "focus") },
            { "INFORMANT", new WordQuestionCategory("What word describes someone who provides information, often secretly, to help solve a problem?", "focus") },
        };
    }
    
    

    public void ChooseRandomSecretWord()
    {
        // Check if the secretWords list is valid
        if (secretWords == null || secretWords.Count == 0)
        {
            Debug.LogError("Secret word list is empty! Add words to the list in the Inspector.");
            return;
        }

        // Choose a random word from the list
        int randomIndex = Random.Range(0, secretWords.Count);
        secretWord = secretWords[randomIndex].ToUpper(); // Choose and convert to uppercase
        Debug.Log($"Random Secret Word Chosen: {secretWord}");
    }

    public string GetSecretWord()
    {
        // Return the selected secret word
        if (string.IsNullOrEmpty(secretWord))
        {
            Debug.LogWarning("Secret word is not initialized. Call ChooseRandomSecretWord() first.");
            return null;
        }
        return secretWord;
    }

    public string GetQuestionForSecretWord()
    {
        if (wordToQuestionMap.ContainsKey(secretWord))
        {
            return wordToQuestionMap[secretWord].Question; // Access the Question property
        }
        else
        {
            Debug.LogWarning($"No question found for secret word: {secretWord}");
            return "No question available.";
        }
    }


    public bool IsInitialized()
    {
        // Check if the secret word has been initialized
        bool initialized = !string.IsNullOrEmpty(secretWord);
        if (!initialized)
        {
            Debug.LogWarning("WordManager is not initialized. Secret word is missing.");
        }
        return initialized;
    }
}