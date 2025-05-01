using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private WordContainer wordContainerPrefab;
    [SerializeField] private Transform wordContainerParent;
    [SerializeField] private Boss boss; // Reference to Boss
    [SerializeField] private ScoreManager scoreManager; // Reference to ScoreManager

    [Header("Camera Manager")]
    [SerializeField] private CameraManager cameraManager; // Reference to CameraManager

    private WordContainer currentWordContainer;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();

        KeyboardKey.onKeyPressed += KeyPressedCallback;
        cameraManager.SwitchToCloseUpCamera(); // Start with the close-up camera
    }

    private void Initialize()
    {
        // Ensure secret word matches the letter container length
        string secretWord = WordManager.instance?.GetSecretWord();
        if (string.IsNullOrEmpty(secretWord))
        {
            Debug.LogError("Secret word is null or empty!");
            return;
        }
        
        CreateNewWordContainer(secretWord.Length);
        InitializeKeyboard(secretWord); // Set keyboard buttons
    }

    private void KeyPressedCallback(char letter)
    {
        if (currentWordContainer != null)
        {
            currentWordContainer.Add(letter);

            if (currentWordContainer.IsComplete())
            {
                CheckWord();
                
                // Get the new secret word before creating a new container
                string newSecretWord = WordManager.instance.GetSecretWord();
                
                // Create a new word container with the appropriate length
                CreateNewWordContainer(newSecretWord.Length);
            }
        }
        else
        {
            Debug.LogError("Current WordContainer is null.");
        }
    }

    private void CheckWord()
{
    string wordToCheck = currentWordContainer.GetWord();
    string secretWord = WordManager.instance?.GetSecretWord();

    if (string.IsNullOrEmpty(secretWord))
    {
        Debug.LogError("Secret word is null or empty!");
        return;
    }

    // Debug log for comparison
    Debug.Log($"Word to Check: {wordToCheck}, Secret Word: {secretWord}");

    // Ensure lengths match before comparing
    if (wordToCheck.Length != secretWord.Length)
    {
        Debug.LogError($"Word length mismatch! WordToCheck: {wordToCheck.Length}, SecretWord: {secretWord.Length}");
        return;
    }

    if (wordToCheck == secretWord)
    {
        Debug.Log("Correct Answer! Hit Boss");

        // If the word is correct, damage the boss
        if (boss != null)
        {
            boss.TakeDamage(20); // Deal 20 damage to the boss
            
            // This will handle both updating the performance tracking and notifying ScoreManager with category info
            WordManager.instance.WordAnsweredCorrectly();
        }
        else
        {
            Debug.LogError("Boss reference is missing!");
        }

        // Switch to close-up camera for correct answer
        cameraManager.SwitchToCloseUpCamera();

        // Get a new secret word from WordManager
        WordManager.instance.ChooseWordUsingLeitnerSystem();
        string newSecretWord = WordManager.instance.GetSecretWord();
        
        // Hide the completed WordContainer BEFORE reinitializing the keyboard
        currentWordContainer.gameObject.SetActive(false);
        
        // Reinitialize the keyboard with the new secret word
        InitializeKeyboard(newSecretWord);
    }
    else
    {
        Debug.Log("Wrong Answer!");

        boss.HealDamage(20); // Heal 20 damage to the boss
        
        // This will handle both updating the performance tracking and notifying ScoreManager with category info
        WordManager.instance.WordAnsweredIncorrectly();

        // Switch to top-down camera for incorrect answer
        cameraManager.SwitchToTopDownCamera();
        
        // Hide the completed WordContainer
        currentWordContainer.gameObject.SetActive(false);

        // Get a new secret word from WordManager
        WordManager.instance.ChooseWordUsingLeitnerSystem();
        string newSecretWord = WordManager.instance.GetSecretWord();

        // Reinitialize the keyboard and word container with the new secret word
        InitializeKeyboard(newSecretWord);
        CreateNewWordContainer(newSecretWord.Length);
    }
}
    private void CreateNewWordContainer(int wordLength)
    {
        currentWordContainer = Instantiate(wordContainerPrefab, wordContainerParent);
        currentWordContainer.Initialize(wordLength);
    }

    private void InitializeKeyboard(string secretWord)
    {
        if (string.IsNullOrEmpty(secretWord))
        {
            Debug.LogError("Secret word is null or empty!");
            return;
        }

        Debug.Log($"Initializing keyboard with secret word: {secretWord}");

        // Shuffle the letters of the secret word
        char[] letters = secretWord.ToCharArray();
        ShuffleArray(letters);

        KeyboardKey[] keys = FindObjectsOfType<KeyboardKey>(true); // Include inactive keys

        if (keys == null || keys.Length == 0)
        {
            Debug.LogError("No KeyboardKey components found in the scene!");
            return;
        }

        Debug.Log($"Found {keys.Length} keyboard keys for word of length {letters.Length}");

        // FIRST: Activate ALL keys (in case any were deactivated)
        foreach (var key in keys)
        {
            key.gameObject.SetActive(true);
        }
        
        // SECOND: Apply shuffling for keys
        // Assuming keys and letters are defined, with keys.Length >= letters.Length

        System.Random random = new System.Random();

        // Calculate how many random filler letters are needed
        int totalKeys = keys.Length;
        int correctLetterCount = letters.Length;
        int fillerCount = totalKeys - correctLetterCount;

        // Generate random filler letters
        char[] fillerLetters = new char[fillerCount];
        for (int i = 0; i < fillerCount; i++)
        {
            fillerLetters[i] = (char)random.Next('A', 'Z' + 1);
        }

        // Combine correct letters and filler letters into one array
        char[] combinedLetters = new char[totalKeys];

        // Fill combinedLetters with random filler letters first
        for (int i = 0; i < fillerCount; i++)
        {
            combinedLetters[i] = fillerLetters[i];
        }

        // Then fill combinedLetters with correct letters
        for (int i = 0; i < correctLetterCount; i++)
        {
            combinedLetters[fillerCount + i] = letters[i];
        }

        // Apply Durstenfeld shuffle to combinedLetters
        for (int i = totalKeys - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            char temp = combinedLetters[i];
            combinedLetters[i] = combinedLetters[j];
            combinedLetters[j] = temp;
        }

        // Assign shuffled letters to keys
        for (int i = 0; i < totalKeys; i++)
        {
            keys[i].SetLetter(combinedLetters[i]);
            Debug.Log($"Key {i} set to letter {combinedLetters[i]}");
        }



    }

    private void ShuffleArray(char[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            char temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}