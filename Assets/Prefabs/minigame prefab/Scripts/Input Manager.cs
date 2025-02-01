using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private WordContainer wordContainerPrefab;
    [SerializeField] private Transform wordContainerParent;
    [SerializeField] private Boss boss; // Reference to Boss

    private WordContainer currentWordContainer;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();

        KeyboardKey.onKeyPressed += KeyPressedCallback;
    }

    // Update is called once per frame
    void Update()
    {

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
            Debug.Log("Hit Boss");

            // If the word is correct, damage the boss
            if (boss != null)
            {
                boss.TakeDamage(20); // Deal 20 damage to the boss
            }
            else
            {
                Debug.LogError("Boss reference is missing!");
            }

            // Get a new secret word from WordManager
            WordManager.instance.ChooseRandomSecretWord();
            string newSecretWord = WordManager.instance.GetSecretWord();

            // Reinitialize the keyboard with the new secret word
            InitializeKeyboard(newSecretWord);
        }
        else
        {
            Debug.Log("Wrong word");
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