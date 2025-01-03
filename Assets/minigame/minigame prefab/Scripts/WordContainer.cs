using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordContainer : MonoBehaviour
{
    [Header("Elements")]
    private LetterContainer[] letterContainers;

    [Header("Settings")]
    private int currentLetterIndex;

    private void Awake()
    {
        letterContainers = GetComponentsInChildren<LetterContainer>();
        Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Start is called before the first frame update
    }

    // Update is called once per frame
    void Update()
    {
        // Update is called once per frame
    }

    public void Initialize()
    {
        currentLetterIndex = 0; // Reset index when initializing
        for (int i = 0; i < letterContainers.Length; i++)
        {
            letterContainers[i].Initialize();
        }
    }

    public void Add(char letter)
    {
        // Ensure we don't exceed the bounds of the array
        if (currentLetterIndex < letterContainers.Length)
        {
            letterContainers[currentLetterIndex].SetLetter(letter);
            currentLetterIndex++;
        }
        else
        {
            Debug.LogWarning("Attempted to add a letter when the container is already full.");
        }
    }

    public string GetWord()
    {
        string word = "";

        for (int i = 0; i < letterContainers.Length; i++)
        {
            word += letterContainers[i].GetLetter().ToString();
        }

        return word;
    }

    public bool IsComplete()
    {
        // Check if all letters have been filled (index equals the length of the array)
        return currentLetterIndex >= letterContainers.Length;
    }
    public int GetLetterContainerCount()
    {
        return letterContainers.Length;
    }
    
}