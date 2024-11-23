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
        for (int i = 0; i < letterContainers.Length; i++)
        {
            letterContainers[i].Initialize();
        }
    }

    public void Add(char letter)
    {
        letterContainers[currentLetterIndex].SetLetter(letter);
        currentLetterIndex++;
    }

    public bool IsComplete()
    {
        return currentLetterIndex >= 9;
    }

}