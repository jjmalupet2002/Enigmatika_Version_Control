using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private WordContainer wordContainerPrefab;
    [SerializeField] private Transform wordContainerParent;

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


}
