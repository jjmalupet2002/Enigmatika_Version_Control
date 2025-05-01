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
    
    [Header("Word Categories")]
    [SerializeField] private Dictionary<string, string> wordToCategoryMap; // Maps words to their categories
    [SerializeField] private List<string> availableCategories; // List of all available categories
    [SerializeField] private int minCategoryDiversity = 5; // Minimum number of different categories to include
    [SerializeField] private List<string> recentlyUsedCategories = new List<string>(); // Track recently used categories

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

    // Player prefs keys
    private const string EASY_BOX_KEY = "LeitnerEasyBox";
    private const string MEDIUM_BOX_KEY = "LeitnerMediumBox";
    private const string HARD_BOX_KEY = "LeitnerHardBox";
    private const string PERFORMANCE_KEY = "LeitnerPerformance";
    private const string CATEGORIES_KEY = "WordCategories";
    private const string RECENT_CATEGORIES_KEY = "RecentCategories";

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
        InitializeWordCategoryMap(); // Initialize the word-to-category mapping
        InitializeLeitnerSystem(); // Set up the Leitner system
        LoadRecentCategories(); // Load recently used categories
        ChooseWordUsingLeitnerSystem(); // Choose word using Leitner system
    }

    private void InitializeWordToQuestionMap()
    {
        wordToQuestionMap = new Dictionary<string, string>
        {
            { "AUDACIOUS", "What word describes someone who is bold, daring, or willing to take risks?" }, // Understanding Vocabulary
            { "PARAMOUNT", "What term means something of the highest importance or significance?" },
            { "TREASURE", "What word refers to valuable items like gold, jewels, or other prized possessions?" },
            { "STANDOFF", "What term describes a situation where two opposing forces are locked in a tense confrontation?" },
            { "NOTORIOUS", "What word describes someone or something that is widely known for being bad or infamous?" },
            { "COMMAND", "What word means to give an official order or directive with authority?" },
            { "STRATEGY", "What term describes actions or plans that are carefully designed to achieve a specific goal?" },
            { "COMMENCE", "What word means to begin or start something, like a project or event?" },
            { "RECOVER", "What term means to retrieve something that was lost or stolen?" },
            { "INFORMANT", "What word describes someone who provides information, often secretly, to help solve a problem?" },
            
            { "KINGDOM", "Where did the story take place?" }, // Identifying Key Details
            { "DUST", "What was dancing in the light in the King’s hall?" },
            { "ANGRY", "What was the King's mood at the beginning?" },
            { "GARETH", "Who is the King's advisor?" },
            { "WARS", "What had recently depleted the kingdom's funds?" },
            { "NUGGET", "What kind of gift was given by the Eastern Emperor?" },
            { "VAULT", "Where was the nugget kept before it was stolen?" },
            { "SHADOW", "Who is the notorious thief mentioned?" },
            { "TAVERN", "Where did Gareth go to retrieve the nugget?" },
            { "CASTLE", "What was the nugget meant to fund?" },
            
            { "PUNISH", "What might the King do if Gareth fails" }, // Making Predictions
            { "RETURN", "What could the Shadow do after escaping?" },
            { "GUARDS", "How can Gareth prevent future thefts?" },
            { "FOREST", "Where might the Shadow flee after the tavern" },
            { "ESCAPE", "What could happen if the guild helped the Shadow" },
            { "SECURE", "What will likely happen to the nugget now?" },
            { "POWER", "If the King is betrayed again, what might he lose?" },
            { "LISTEN", "What might Gareth do with his informants next?" },
            { "REFORM", "What could the King do to strengthen his rule?" },
            { "REVENGE", "What might the Shadow seek after his loss?" },
            
            { "LOYAL", "What is one word to describe Gareth?" }, // Character Analysis
            { "CLUMSY", "What flaw does Gareth have?" },
            { "STERN", "What trait best describes the King?" },
            { "EXPAND", "What is the King’s goal for the kingdom?" },
            { "STEALTHY", "What word describes the Shadow’s methods?" },
            { "CLEVER", "Why is Gareth still effective despite his flaws?" },
            { "FURIOUS", "What emotion does the King show when angry?" },
            { "GREED", "What might motivate the Shadow?" },
            { "HONOR", "What does Gareth value most in his duty?" },
            { "JUST", "What best defines the King’s leadership?" },
        };
    }

    private void InitializeWordCategoryMap()
    {
        // Initialize default categories if the list is empty
        if (availableCategories == null || availableCategories.Count == 0)
        {
            availableCategories = new List<string>
            {
                "Identifying Key Details",
                "Making Predictions",
                "Character Analysis",
                "Understanding Vocabulary"
            };
        
            if (enableDebugLogs)
            {
                Debug.Log("No categories found in Inspector. Using default categories.");
            }
        }
        else if (enableDebugLogs)
        {
            Debug.Log($"Using {availableCategories.Count} categories from Inspector.");
        }

        // Make sure "Uncategorized" is available if needed
        if (!availableCategories.Contains("Uncategorized"))
        {
            availableCategories.Add("Uncategorized");
        }
        
        // Initialize word-to-category mapping
        wordToCategoryMap = new Dictionary<string, string>
        {
            { "AUDACIOUS", "Understanding Vocabulary" },
            { "PARAMOUNT", "Understanding Vocabulary" },
            { "TREASURE", "Understanding Vocabulary" },
            { "STANDOFF", "Understanding Vocabulary" },
            { "NOTORIOUS", "Understanding Vocabulary" },
            { "COMMAND", "Understanding Vocabulary" },
            { "STRATEGY", "Understanding Vocabulary" },
            { "COMMENCE", "Understanding Vocabulary" },
            { "RECOVER", "Understanding Vocabulary" },
            { "INFORMANT", "Understanding Vocabulary" },
            { "KINGDOM", "Identifying Key Details" },
            { "DUSTY", "Identifying Key Details" },
            { "ANGRY", "Identifying Key Details" },
            { "GARETH", "Identifying Key Details" },
            { "WARS", "Identifying Key Details" },
            { "NUGGET", "Identifying Key Details" },
            { "VAULT", "Identifying Key Details" },
            { "SHADOW", "Identifying Key Details" },
            { "TAVERN", "Identifying Key Details" },
            { "CASTLE", "Identifying Key Details" },
            { "PUNISH", "Making Predictions" },
            { "RETURN", "Making Predictions" },
            { "GUARDS", "Making Predictions" },
            { "FOREST", "Making Predictions" },
            { "ESCAPE", "Making Predictions" },
            { "SECURE", "Making Predictions" },
            { "POWER", "Making Predictions" },
            { "LISTEN", "Making Predictions" },
            { "REFORM", "Making Predictions" },
            { "REVENGE", "Making Predictions" },
            { "LOYAL", "Character Analysis" },
            { "CLUMSY", "Character Analysis" },
            { "STERN", "Character Analysis" },
            { "EXPAND", "Character Analysis" },
            { "STEALTHY", "Character Analysis" },
            { "CLEVER", "Character Analysis" },
            { "FURIOUS", "Character Analysis" },
            { "GREEDY", "Character Analysis" },
            { "HONOR", "Character Analysis" },
            { "JUST", "Character Analysis" },
            
        };

        // Verify all words have categories
        foreach (string word in secretWords)
        {
            string upperWord = word.ToUpper();
            if (!wordToCategoryMap.ContainsKey(upperWord))
            {
                Debug.LogWarning($"Word '{upperWord}' has no assigned category!");
                wordToCategoryMap[upperWord] = "Uncategorized";
                
                if (!availableCategories.Contains("Uncategorized"))
                {
                    availableCategories.Add("Uncategorized");
                }
            }
        }

        // Log categories
        if (enableDebugLogs)
        {
            Debug.Log($"Word categories initialized with {availableCategories.Count} categories");
            foreach (var category in availableCategories)
            {
                int wordCount = wordToCategoryMap.Count(kvp => kvp.Value == category);
                Debug.Log($"Category '{category}': {wordCount} words");
            }
        }
    }

    private void InitializeLeitnerSystem()
    {
        // Initialize word performance tracking
        foreach (string word in secretWords)
        {
            string upperWord = word.ToUpper();
            if (!wordPerformance.ContainsKey(upperWord))
            {
                wordPerformance[upperWord] = 0;
            }
        }

        // Load saved Leitner system state
        bool savedDataExists = LoadLeitnerSystemState();

        // If no saved data, initialize all words in hard box
        if (!savedDataExists)
        {
            if (enableDebugLogs)
            {
                Debug.Log("No saved Leitner data found. Initializing all words in hard box.");
            }

            // Clear existing boxes just to be safe
            easyBox.Clear();
            mediumBox.Clear();
            hardBox.Clear();

            // Place all words in the hard box
            foreach (string word in secretWords)
            {
                string upperWord = word.ToUpper();
                hardBox[upperWord] = 0;
            }
        }

        // Print detailed status of the Leitner system
        if (enableDebugLogs)
        {
            Debug.Log($"Leitner System Initialized with {secretWords.Count} words");
            Debug.Log($"Box distribution - Easy: {easyBox.Count}, Medium: {mediumBox.Count}, Hard: {hardBox.Count}");

            // Print all words and their box placement
            Debug.Log("--- EASY BOX WORDS ---");
            foreach (var word in easyBox.Keys)
            {
                Debug.Log($"EASY: {word} (Performance: {wordPerformance[word]}, Category: {GetCategoryForWord(word)})");
            }

            Debug.Log("--- MEDIUM BOX WORDS ---");
            foreach (var word in mediumBox.Keys)
            {
                Debug.Log($"MEDIUM: {word} (Performance: {wordPerformance[word]}, Category: {GetCategoryForWord(word)})");
            }

            Debug.Log("--- HARD BOX WORDS ---");
            foreach (var word in hardBox.Keys)
            {
                Debug.Log($"HARD: {word} (Performance: {wordPerformance[word]}, Category: {GetCategoryForWord(word)})");
            }
        }
    }

    private bool LoadLeitnerSystemState()
    {
        // Check if we have saved data
        if (PlayerPrefs.HasKey(EASY_BOX_KEY) || PlayerPrefs.HasKey(MEDIUM_BOX_KEY) || PlayerPrefs.HasKey(HARD_BOX_KEY))
        {
            // Clear existing boxes before loading
            easyBox.Clear();
            mediumBox.Clear();
            hardBox.Clear();

            // Load easy box
            string easyBoxData = PlayerPrefs.GetString(EASY_BOX_KEY, "");
            if (!string.IsNullOrEmpty(easyBoxData))
            {
                string[] easyWords = easyBoxData.Split(',');
                foreach (string word in easyWords)
                {
                    if (!string.IsNullOrEmpty(word))
                    {
                        easyBox[word] = 0;
                    }
                }
            }

            // Load medium box
            string mediumBoxData = PlayerPrefs.GetString(MEDIUM_BOX_KEY, "");
            if (!string.IsNullOrEmpty(mediumBoxData))
            {
                string[] mediumWords = mediumBoxData.Split(',');
                foreach (string word in mediumWords)
                {
                    if (!string.IsNullOrEmpty(word))
                    {
                        mediumBox[word] = 0;
                    }
                }
            }

            // Load hard box
            string hardBoxData = PlayerPrefs.GetString(HARD_BOX_KEY, "");
            if (!string.IsNullOrEmpty(hardBoxData))
            {
                string[] hardWords = hardBoxData.Split(',');
                foreach (string word in hardWords)
                {
                    if (!string.IsNullOrEmpty(word))
                    {
                        hardBox[word] = 0;
                    }
                }
            }

            // Load performance data
            string performanceData = PlayerPrefs.GetString(PERFORMANCE_KEY, "");
            if (!string.IsNullOrEmpty(performanceData))
            {
                string[] performanceEntries = performanceData.Split(';');
                foreach (string entry in performanceEntries)
                {
                    if (!string.IsNullOrEmpty(entry) && entry.Contains(":"))
                    {
                        string[] parts = entry.Split(':');
                        if (parts.Length == 2)
                        {
                            string word = parts[0];
                            if (int.TryParse(parts[1], out int performance))
                            {
                                wordPerformance[word] = performance;
                            }
                        }
                    }
                }
            }

            if (enableDebugLogs)
            {
                Debug.Log("Leitner system state loaded successfully");
            }

            return true;
        }

        return false;
    }

    private void LoadRecentCategories()
    {
        string savedCategories = PlayerPrefs.GetString(RECENT_CATEGORIES_KEY, "");
        if (!string.IsNullOrEmpty(savedCategories))
        {
            string[] categories = savedCategories.Split(',');
            recentlyUsedCategories = new List<string>(categories);
            
            if (enableDebugLogs)
            {
                Debug.Log($"Loaded {recentlyUsedCategories.Count} recently used categories");
            }
        }
    }

    private void SaveRecentCategories()
    {
        string categoriesToSave = string.Join(",", recentlyUsedCategories);
        PlayerPrefs.SetString(RECENT_CATEGORIES_KEY, categoriesToSave);
        PlayerPrefs.Save();
        
        if (enableDebugLogs)
        {
            Debug.Log($"Saved {recentlyUsedCategories.Count} recently used categories");
        }
    }

    private void SaveLeitnerSystemState()
    {
        // Save easy box words
        string easyBoxData = string.Join(",", easyBox.Keys);
        PlayerPrefs.SetString(EASY_BOX_KEY, easyBoxData);

        // Save medium box words
        string mediumBoxData = string.Join(",", mediumBox.Keys);
        PlayerPrefs.SetString(MEDIUM_BOX_KEY, mediumBoxData);

        // Save hard box words
        string hardBoxData = string.Join(",", hardBox.Keys);
        PlayerPrefs.SetString(HARD_BOX_KEY, hardBoxData);

        // Save performance data
        List<string> performanceEntries = new List<string>();
        foreach (var entry in wordPerformance)
        {
            performanceEntries.Add($"{entry.Key}:{entry.Value}");
        }
        string performanceData = string.Join(";", performanceEntries);
        PlayerPrefs.SetString(PERFORMANCE_KEY, performanceData);

        // Save changes
        PlayerPrefs.Save();

        if (enableDebugLogs)
        {
            Debug.Log("Leitner system state saved successfully");
        }
    }

    public void ChooseWordUsingLeitnerSystem()
    {
        if (secretWords == null || secretWords.Count == 0)
        {
            Debug.LogError("Secret word list is empty! Add words to the list in the Inspector.");
            return;
        }

        // First, get words from each Leitner box
        Dictionary<string, List<string>> categorizedEasyWords = GetCategorizedWords(easyBox);
        Dictionary<string, List<string>> categorizedMediumWords = GetCategorizedWords(mediumBox);
        Dictionary<string, List<string>> categorizedHardWords = GetCategorizedWords(hardBox);

        // Check if we have enough diverse categories to work with
        HashSet<string> allAvailableCategories = new HashSet<string>();
        foreach (var category in categorizedEasyWords.Keys) allAvailableCategories.Add(category);
        foreach (var category in categorizedMediumWords.Keys) allAvailableCategories.Add(category);
        foreach (var category in categorizedHardWords.Keys) allAvailableCategories.Add(category);

        if (enableDebugLogs)
        {
            Debug.Log($"Found {allAvailableCategories.Count} different categories across all Leitner boxes");
        }

        // Select box based on probability
        float randomValue = Random.value;
        Dictionary<string, List<string>> selectedBoxWords;
        string boxSelected;

        if (randomValue < easyWordSelectionChance && easyBox.Count > 0)
        {
            selectedBoxWords = categorizedEasyWords;
            boxSelected = "EASY";
        }
        else if (randomValue < easyWordSelectionChance + mediumWordSelectionChance && mediumBox.Count > 0)
        {
            selectedBoxWords = categorizedMediumWords;
            boxSelected = "MEDIUM";
        }
        else
        {
            selectedBoxWords = categorizedHardWords;
            boxSelected = "HARD";
        }

        // If selected box is empty, fall back to hard box or random selection
        if (selectedBoxWords.Count == 0)
        {
            if (categorizedHardWords.Count > 0)
            {
                selectedBoxWords = categorizedHardWords;
                boxSelected = "HARD (fallback)";
            }
            else
            {
                // Last resort: choose randomly from all words
                int randomIndex = Random.Range(0, secretWords.Count);
                secretWord = secretWords[randomIndex].ToUpper();
                boxSelected = "RANDOM (all boxes empty)";

                // Add to hard box if not found in any box
                if (GetBoxForWord(secretWord) == "NOT FOUND")
                {
                    hardBox[secretWord] = 0;
                }

                // Update recently used categories
                UpdateRecentlyUsedCategories(GetCategoryForWord(secretWord));
                
                // Log selection info
                if (enableDebugLogs)
                {
                    Debug.Log($"========= WORD SELECTION =========");
                    Debug.Log($"Selected word '{secretWord}' from {boxSelected}");
                    Debug.Log($"Category: {GetCategoryForWord(secretWord)}");
                    Debug.Log($"Current performance score: {wordPerformance[secretWord]}");
                }
                
                return;
            }
        }

        // Prioritize categories that haven't been used recently
        List<string> priorityCategories = new List<string>();
        foreach (var category in selectedBoxWords.Keys)
        {
            if (!recentlyUsedCategories.Contains(category))
            {
                priorityCategories.Add(category);
            }
        }

        // If we have priority categories, select from those first
        string selectedCategory;
        if (priorityCategories.Count > 0)
        {
            int randomCategoryIndex = Random.Range(0, priorityCategories.Count);
            selectedCategory = priorityCategories[randomCategoryIndex];
        }
        else
        {
            // Otherwise select any category
            List<string> availableCategories = new List<string>(selectedBoxWords.Keys);
            int randomCategoryIndex = Random.Range(0, availableCategories.Count);
            selectedCategory = availableCategories[randomCategoryIndex];
        }

        // Select a random word from the chosen category
        List<string> wordsInCategory = selectedBoxWords[selectedCategory];
        int randomWordIndex = Random.Range(0, wordsInCategory.Count);
        secretWord = wordsInCategory[randomWordIndex];

        // Update recently used categories
        UpdateRecentlyUsedCategories(selectedCategory);

        // Print detailed selection info
        if (enableDebugLogs)
        {
            Debug.Log($"========= WORD SELECTION =========");
            Debug.Log($"Selected word '{secretWord}' from {boxSelected} box");
            Debug.Log($"Category: {selectedCategory}");
            Debug.Log($"Current performance score: {wordPerformance[secretWord]}");
            Debug.Log($"Box distribution - Easy: {easyBox.Count}, Medium: {mediumBox.Count}, Hard: {hardBox.Count}");
            Debug.Log($"Random value: {randomValue} (Easy threshold: {easyWordSelectionChance}, Medium threshold: {easyWordSelectionChance + mediumWordSelectionChance})");
            Debug.Log($"Recently used categories: {string.Join(", ", recentlyUsedCategories)}");
        }
    }

    private Dictionary<string, List<string>> GetCategorizedWords(Dictionary<string, int> box)
    {
        // Group words by category
        Dictionary<string, List<string>> categorizedWords = new Dictionary<string, List<string>>();
        
        foreach (string word in box.Keys)
        {
            string category = GetCategoryForWord(word);
            
            if (!categorizedWords.ContainsKey(category))
            {
                categorizedWords[category] = new List<string>();
            }
            
            categorizedWords[category].Add(word);
        }
        
        return categorizedWords;
    }

    private void UpdateRecentlyUsedCategories(string categoryUsed)
    {
        // Add the category to the recently used list
        if (!recentlyUsedCategories.Contains(categoryUsed))
        {
            recentlyUsedCategories.Add(categoryUsed);
        }
        
        // Keep only the most recent categories (limited by minCategoryDiversity)
        while (recentlyUsedCategories.Count > minCategoryDiversity)
        {
            recentlyUsedCategories.RemoveAt(0); // Remove the oldest category
        }
        
        // Save the updated list
        SaveRecentCategories();
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
                Debug.Log($"Category: {GetCategoryForWord(secretWord)}");
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
            
            // Notify score manager with category info
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.RecordCorrectAnswer(GetCategoryForWord(secretWord));
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
                Debug.Log($"Category: {GetCategoryForWord(secretWord)}");
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
            
            // Notify score manager with category info
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.RecordWrongAnswer(GetCategoryForWord(secretWord));
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
    
    // Helper method to get a word's category
    public string GetCategoryForWord(string word)
    {
        if (wordToCategoryMap.ContainsKey(word))
            return wordToCategoryMap[word];
        else
            return "Uncategorized";
    }
    
    // Get all available categories
    public List<string> GetAllCategories()
    {
        return availableCategories;
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
            status.AppendLine($"  - {word} (Performance: {wordPerformance[word]}, Category: {GetCategoryForWord(word)})");
        }

        status.AppendLine($"MEDIUM box ({mediumBox.Count} words):");
        foreach (var word in mediumBox.Keys)
        {
            status.AppendLine($"  - {word} (Performance: {wordPerformance[word]}, Category: {GetCategoryForWord(word)})");
        }

        status.AppendLine($"HARD box ({hardBox.Count} words):");
        foreach (var word in hardBox.Keys)
        {
            status.AppendLine($"  - {word} (Performance: {wordPerformance[word]}, Category: {GetCategoryForWord(word)})");
        }

        status.AppendLine($"Recently used categories: {string.Join(", ", recentlyUsedCategories)}");

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
        
        // Validate minimum category diversity
        if (minCategoryDiversity < 1)
        {
            Debug.LogWarning("Minimum category diversity must be at least 1. Setting to 1.");
            minCategoryDiversity = 1;
        }
    }
    
    public void ResetLeitnerSystem()
    {
        if (enableDebugLogs)
        {
            Debug.Log("Resetting Leitner system...");
        }

        easyBox.Clear();
        mediumBox.Clear();
        hardBox.Clear();
        wordPerformance.Clear();
        recentlyUsedCategories.Clear();

        foreach (string word in secretWords)
        {
            string upperWord = word.ToUpper();
            hardBox[upperWord] = 0;
            wordPerformance[upperWord] = 0;
        }

        SaveLeitnerSystemState();
        SaveRecentCategories();

        if (enableDebugLogs)
        {
            Debug.Log("Leitner system has been reset.");
        }
    }
}