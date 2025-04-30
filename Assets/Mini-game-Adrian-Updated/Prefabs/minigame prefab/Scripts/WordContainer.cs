using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordContainer : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private LetterContainer letterContainerPrefab; // Reference to the letter container prefab
    [SerializeField] private Transform letterContainerParent; // Parent transform for letter containers
    private List<LetterContainer> letterContainers = new List<LetterContainer>();

    [Header("Settings")]
    private int currentLetterIndex;
    [SerializeField] private int maxLetterCount = 9; // Maximum number of letters allowed
    [SerializeField] private float letterSpacing = 44.8f; // Space between letter containers
    [SerializeField] private Vector2 startPosition = Vector2.zero; // Starting position for the first letter

    private void OnEnable()
    {
        BackspaceKey.onBackspacePressed += RemoveLastLetter;
    }

    private void OnDisable()
    {
        BackspaceKey.onBackspacePressed -= RemoveLastLetter;
    }

    public void Initialize(int wordLength = 0)
    {
        // Clear existing letter containers
        ClearLetterContainers();
        
        // Create new letter containers based on the word length
        if (wordLength > 0 && wordLength <= maxLetterCount)
        {
            CreateLetterContainers(wordLength);
        }
        
        currentLetterIndex = 0;
        foreach (var container in letterContainers)
        {
            container.Initialize();
        }
    }

    private void ClearLetterContainers()
    {
        // Destroy all existing letter containers
        foreach (var container in letterContainers)
        {
            if (container != null)
            {
                Destroy(container.gameObject);
            }
        }
        
        letterContainers.Clear();
    }

    private void CreateLetterContainers(int count)
    {
        // Calculate the center position based on word length
        float totalWidth = (count - 1) * letterSpacing;
        float startX = startPosition.x;  // Use a fixed starting position
        
        for (int i = 0; i < count; i++)
        {
            LetterContainer newContainer = Instantiate(letterContainerPrefab, letterContainerParent);
            
            // Position the container
            RectTransform rectTransform = newContainer.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // For UI elements, set the anchoredPosition
                rectTransform.anchoredPosition = new Vector2(startX + (i * letterSpacing), 0);
            }
            else
            {
                // For non-UI elements, set the local position
                newContainer.transform.localPosition = new Vector3(startX + (i * letterSpacing), 0, 0);
            }
            
            letterContainers.Add(newContainer);
        }
    }

    public void Add(char letter)
    {
        if (currentLetterIndex < letterContainers.Count)
        {
            letterContainers[currentLetterIndex].SetLetter(letter);
            currentLetterIndex++;
        }
        else
        {
            Debug.LogWarning("Attempted to add a letter when the container is already full.");
        }
    }

    public void RemoveLastLetter()
    {
        if (currentLetterIndex > 0)
        {
            currentLetterIndex--;
            letterContainers[currentLetterIndex].SetLetter(' '); // Clear last letter
        }
    }

    public string GetWord()
    {
        string word = "";
        foreach (var container in letterContainers)
        {
            word += container.GetLetter().ToString();
        }
        return word.Trim();
    }

    public bool IsComplete()
    {
        return currentLetterIndex >= letterContainers.Count;
    }

    public int GetLetterContainerCount()
    {
        return letterContainers.Count;
    }
}