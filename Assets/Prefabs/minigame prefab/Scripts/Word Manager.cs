using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public static WordManager instance;

    [Header("Elements")]
    [SerializeField] private string secretWord;

    private void Awake()
    {
        if (instance == null)
            instance = this; // Assign the current instance
        else
            Destroy(gameObject); // Destroy duplicate instances
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetSecretWord()
    {
        return secretWord.ToUpper();
    }
}
