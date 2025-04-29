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
        CreateNewWordContainer();

        // Ensure secret word matches the letter container length
        string secretWord = WordManager.instance?.GetSecretWord();
        if (!string.IsNullOrEmpty(secretWord) && secretWord.Length != currentWordContainer.GetLetterContainerCount())
        {
            Debug.LogError("Secret word length does not match the number of LetterContainers!");
        }

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
                CreateNewWordContainer();
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
                scoreManager.RecordCorrectAnswer(); 
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

            // Reinitialize the keyboard with the new secret word
            InitializeKeyboard(newSecretWord);
        }
        else
        {
            Debug.Log("Wrong Answer!");

            boss.HealDamage(20); // Heal 20 damage to the boss
            
            scoreManager.RecordWrongAnswer();
            WordManager.instance.WordAnsweredIncorrectly();


            // Switch to top-down camera for incorrect answer
            cameraManager.SwitchToTopDownCamera();
        }

        // Hide the completed WordContainer
        currentWordContainer.gameObject.SetActive(false);
    }

    private void CreateNewWordContainer()
    {
        currentWordContainer = Instantiate(wordContainerPrefab, wordContainerParent);
        currentWordContainer.Initialize();
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

        KeyboardKey[] keys = FindObjectsOfType<KeyboardKey>();

        if (keys == null || keys.Length == 0)
        {
            return;
        }

        for (int i = 0; i < keys.Length; i++)
        {
            if (i < letters.Length)
            {
                keys[i].SetLetter(letters[i]);
            }
            else
            {
                keys[i].gameObject.SetActive(false);
            }
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