using System;
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

    private void OnEnable()
    {
        BackspaceKey.onBackspacePressed += RemoveLastLetter;
    }

    private void OnDisable()
    {
        BackspaceKey.onBackspacePressed -= RemoveLastLetter;
    }

    public void Initialize()
    {
        currentLetterIndex = 0;
        foreach (var container in letterContainers)
        {
            container.Initialize();
        }
    }

    public void Add(char letter)
    {
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

    public void RemoveLastLetter()
    {
        if (currentLetterIndex > 0)
        {
            currentLetterIndex--;
            letterContainers[currentLetterIndex].SetLetter(' '); // Clear last letter
        }
        else
        {
            
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
        return currentLetterIndex >= letterContainers.Length;
    }

    public int GetLetterContainerCount()
    {
        return letterContainers.Length;
    }
}
