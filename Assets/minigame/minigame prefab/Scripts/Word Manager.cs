using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public static WordManager instance;

    [Header("Elements")]
    [SerializeField] private List<string> secretWords; // List of possible secret words
    private string secretWord; // The chosen secret word

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

        ChooseRandomSecretWord(); // Choose secret word early
    }

    private void ChooseRandomSecretWord()
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
