using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

public class WordManager : MonoBehaviour
{
    public static WordManager instance;

    [Header("Elements")]
    [SerializeField] private List<string> secretWords; // List of possible secret words
    private string secretWord; // The chosen secret word

    [Header("Questions/Hints")]
    [SerializeField] private Dictionary<string, string> wordToQuestionMap; // Maps words to questions

    [Header("Leitner System Settings")]
    [SerializeField] private int numberOfBoxes = 3; // Number of difficulty levels
    [SerializeField] private float easyWordSelectionChance = 0.2f; // 20% chance to select from easy box
    [SerializeField] private float mediumWordSelectionChance = 0.3f; // 30% chance to select from medium box
    // Hard words get the remaining 50% chance
    [SerializeField] private bool enableDebugLogs = true; // Toggle for detailed debug logs

    // Leitner system boxes (dictionaries store word:correctness_count pairs)
    private Dictionary<string, int> easyBox = new Dictionary<string, int>();
    private Dictionary<string, int> mediumBox = new Dictionary<string, int>();
    private Dictionary<string, int> hardBox = new Dictionary<string, int>();

    // Player performance tracking
    private Dictionary<string, int> wordPerformance = new Dictionary<string, int>();

    // Constants for performance tracking
    private const int PROMOTION_THRESHOLD = 2; // Get word right this many times to promote
    private const int DEMOTION_THRESHOLD = -1; // Get word wrong this many times to demote

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
        InitializeLeitnerSystem(); // Setup the Leitner system
        ChooseWordUsingLeitnerSystem(); // Choose word using Leitner system
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

    private void InitializeLeitnerSystem()
    {
        // Initialize performance tracking for all words
        foreach (string word in secretWords)
        {
            string upperWord = word.ToUpper();
            wordPerformance[upperWord] = 0;

            // Initially place all words in the hard box
            hardBox[upperWord] = 0;
        }

        // If you have saved data, you would load it here
        LoadLeitnerSystemState();

        // Print detailed status of the Leitner system
        if (enableDebugLogs)
        {
            Debug.Log($"Leitner System Initialized with {secretWords.Count} words");
            Debug.Log($"Box distribution - Easy: {easyBox.Count}, Medium: {mediumBox.Count}, Hard: {hardBox.Count}");

            // Print all words and their box placement
            Debug.Log("--- EASY BOX WORDS ---");
            foreach (var word in easyBox.Keys)
            {
                Debug.Log($"EASY: {word} (Performance: {wordPerformance[word]})");
            }

            Debug.Log("--- MEDIUM BOX WORDS ---");
            foreach (var word in mediumBox.Keys)
            {
                Debug.Log($"MEDIUM: {word} (Performance: {wordPerformance[word]})");
            }

            Debug.Log("--- HARD BOX WORDS ---");
            foreach (var word in hardBox.Keys)
            {
                Debug.Log($"HARD: {word} (Performance: {wordPerformance[word]})");
            }
        }
    }

    private void LoadLeitnerSystemState()
    {
        // For now, this is just a placeholder
        // In a real implementation, you would load saved data from PlayerPrefs or a file

        // Example implementation could be:
        // if (PlayerPrefs.HasKey("LeitnerSystemData"))
        // {
        //     string jsonData = PlayerPrefs.GetString("LeitnerSystemData");
        //     LeitnerData data = JsonUtility.FromJson<LeitnerData>(jsonData);
        //     // Populate boxes from saved data
        // }

        if (enableDebugLogs)
        {
            Debug.Log("LoadLeitnerSystemState: No saved data found, using default initialization");
        }
    }

    private void SaveLeitnerSystemState()
    {
        // For now, this is just a placeholder
        // In a real implementation, you would save data to PlayerPrefs or a file

        // Example implementation could be:
        // LeitnerData data = new LeitnerData();
        // data.easyWords = easyBox.Keys.ToList();
        // data.mediumWords = mediumBox.Keys.ToList();
        // data.hardWords = hardBox.Keys.ToList();
        // data.wordPerformance = wordPerformance;
        // string jsonData = JsonUtility.ToJson(data);
        // PlayerPrefs.SetString("LeitnerSystemData", jsonData);
        // PlayerPrefs.Save();

        if (enableDebugLogs)
        {
            Debug.Log("SaveLeitnerSystemState: Leitner system state saved");
        }
    }

    public void ChooseWordUsingLeitnerSystem()
    {
        if (secretWords == null || secretWords.Count == 0)
        {
            Debug.LogError("Secret word list is empty! Add words to the list in the Inspector.");
            return;
        }

        float randomValue = Random.value; // Random value between 0 and 1
        string boxSelected = ""; // Track which box we're selecting from

        // Choose a box based on probability
        if (randomValue < easyWordSelectionChance && easyBox.Count > 0)
        {
            // Select from easy box
            secretWord = SelectRandomWordFromBox(easyBox);
            boxSelected = "EASY";
        }
        else if (randomValue < easyWordSelectionChance + mediumWordSelectionChance && mediumBox.Count > 0)
        {
            // Select from medium box
            secretWord = SelectRandomWordFromBox(mediumBox);
            boxSelected = "MEDIUM";
        }
        else
        {
            // Select from hard box (or default to random if empty)
            if (hardBox.Count > 0)
            {
                secretWord = SelectRandomWordFromBox(hardBox);
                boxSelected = "HARD";
            }
            else
            {
                // Fallback to random selection if no words in hard box
                int randomIndex = Random.Range(0, secretWords.Count);
                secretWord = secretWords[randomIndex].ToUpper();
                boxSelected = "RANDOM (Hard box empty)";
            }
        }

        // Print detailed selection info
        if (enableDebugLogs)
        {
            Debug.Log($"========= WORD SELECTION =========");
            Debug.Log($"Selected word '{secretWord}' from {boxSelected} box");
            Debug.Log($"Current performance score: {wordPerformance[secretWord]}");
            Debug.Log($"Box distribution - Easy: {easyBox.Count}, Medium: {mediumBox.Count}, Hard: {hardBox.Count}");
            Debug.Log($"Random value: {randomValue} (Easy threshold: {easyWordSelectionChance}, Medium threshold: {easyWordSelectionChance + mediumWordSelectionChance})");
        }
    }

    private string SelectRandomWordFromBox(Dictionary<string, int> box)
    {
        List<string> words = new List<string>(box.Keys);
        int randomIndex = Random.Range(0, words.Count);
        return words[randomIndex];
    }

    // Call this method when the player gets the word correct
    public void WordAnsweredCorrectly()
    {
        if (string.IsNullOrEmpty(secretWord)) return;

        // Increment the performance counter for this word
        if (wordPerformance.ContainsKey(secretWord))
        {
            wordPerformance[secretWord]++;

            if (enableDebugLogs)
            {
                Debug.Log($"✓ Word '{secretWord}' answered CORRECTLY");
                Debug.Log($"Performance score increased to {wordPerformance[secretWord]}");
                Debug.Log($"Current box: {GetBoxForWord(secretWord)}");
                Debug.Log($"Promotion threshold: {PROMOTION_THRESHOLD}");
            }

            // Check if the word should be promoted
            if (wordPerformance[secretWord] >= PROMOTION_THRESHOLD)
            {
                PromoteWord(secretWord);
                wordPerformance[secretWord] = 0; // Reset counter after promotion
            }
        }

        SaveLeitnerSystemState();
    }

    // Call this method when the player gets the word incorrect
    public void WordAnsweredIncorrectly()
    {
        if (string.IsNullOrEmpty(secretWord)) return;

        // Decrement the performance counter for this word
        if (wordPerformance.ContainsKey(secretWord))
        {
            wordPerformance[secretWord]--;

            if (enableDebugLogs)
            {
                Debug.Log($"✗ Word '{secretWord}' answered INCORRECTLY");
                Debug.Log($"Performance score decreased to {wordPerformance[secretWord]}");
                Debug.Log($"Current box: {GetBoxForWord(secretWord)}");
                Debug.Log($"Demotion threshold: {DEMOTION_THRESHOLD}");
            }

            // Check if the word should be demoted
            if (wordPerformance[secretWord] <= DEMOTION_THRESHOLD)
            {
                DemoteWord(secretWord);
                wordPerformance[secretWord] = 0; // Reset counter after demotion
            }
        }

        SaveLeitnerSystemState();
    }

    private void PromoteWord(string word)
    {
        string oldBox = GetBoxForWord(word);
        string newBox = oldBox;

        if (hardBox.ContainsKey(word))
        {
            // Promote from hard to medium
            hardBox.Remove(word);
            mediumBox[word] = 0;
            newBox = "MEDIUM";
        }
        else if (mediumBox.ContainsKey(word))
        {
            // Promote from medium to easy
            mediumBox.Remove(word);
            easyBox[word] = 0;
            newBox = "EASY";
        }

        if (enableDebugLogs)
        {
            if (oldBox == newBox)
            {
                Debug.Log($"Word '{word}' is already in the {oldBox} box, can't promote further");
            }
            else
            {
                Debug.Log($"⬆ PROMOTED: Word '{word}' moved from {oldBox} to {newBox}");
                PrintBoxDistribution();
            }
        }
    }

    private void DemoteWord(string word)
    {
        string oldBox = GetBoxForWord(word);
        string newBox = oldBox;

        if (easyBox.ContainsKey(word))
        {
            // Demote from easy to medium
            easyBox.Remove(word);
            mediumBox[word] = 0;
            newBox = "MEDIUM";
        }
        else if (mediumBox.ContainsKey(word))
        {
            // Demote from medium to hard
            mediumBox.Remove(word);
            hardBox[word] = 0;
            newBox = "HARD";
        }

        if (enableDebugLogs)
        {
            if (oldBox == newBox)
            {
                Debug.Log($"Word '{word}' is already in the {oldBox} box, can't demote further");
            }
            else
            {
                Debug.Log($"⬇ DEMOTED: Word '{word}' moved from {oldBox} to {newBox}");
                PrintBoxDistribution();
            }
        }
    }

    // Helper method to determine which box a word is in
    public string GetBoxForWord(string word)
    {
        if (easyBox.ContainsKey(word))
            return "EASY";
        else if (mediumBox.ContainsKey(word))
            return "MEDIUM";
        else if (hardBox.ContainsKey(word))
            return "HARD";
        else
            return "NOT FOUND";
    }

    // Helper method to print box distribution
    private void PrintBoxDistribution()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"--- UPDATED BOX DISTRIBUTION ---");
            Debug.Log($"EASY: {easyBox.Count} words, MEDIUM: {mediumBox.Count} words, HARD: {hardBox.Count} words");
        }
    }

    public string GetSecretWord()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"GetSecretWord called. Current secret word: {secretWord}");
        }
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

    // Public method to get the current distribution of words (for debugging or UI)
    public string GetLeitnerSystemStatus()
    {
        StringBuilder status = new StringBuilder();
        status.AppendLine("===== LEITNER SYSTEM STATUS =====");
        status.AppendLine($"EASY box ({easyBox.Count} words):");
        foreach (var word in easyBox.Keys)
        {
            status.AppendLine($"  - {word} (Performance: {wordPerformance[word]})");
        }

        status.AppendLine($"MEDIUM box ({mediumBox.Count} words):");
        foreach (var word in mediumBox.Keys)
        {
            status.AppendLine($"  - {word} (Performance: {wordPerformance[word]})");
        }

        status.AppendLine($"HARD box ({hardBox.Count} words):");
        foreach (var word in hardBox.Keys)
        {
            status.AppendLine($"  - {word} (Performance: {wordPerformance[word]})");
        }

        return status.ToString();
    }

    // Method to print the current Leitner system status to the console
    public void PrintLeitnerSystemStatus()
    {
        if (enableDebugLogs)
        {
            Debug.Log(GetLeitnerSystemStatus());
        }
    }

    // For Unity Editor - OnValidate is called when a value is changed in the Inspector
    private void OnValidate()
    {
        // Ensure probabilities don't exceed 1.0
        float total = easyWordSelectionChance + mediumWordSelectionChance;
        if (total > 1.0f)
        {
            Debug.LogWarning("Selection probabilities exceed 1.0! Adjusting values.");
            easyWordSelectionChance = Mathf.Min(easyWordSelectionChance, 1.0f);
            mediumWordSelectionChance = Mathf.Min(mediumWordSelectionChance, 1.0f - easyWordSelectionChance);
        }
    }
}