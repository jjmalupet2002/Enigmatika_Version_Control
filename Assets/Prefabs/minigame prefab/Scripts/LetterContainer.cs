using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LetterContainer : MonoBehaviour
{
    [Header("TextMeshPro Letter")]
    [SerializeField] private TextMeshPro letter;


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
        letter.text = "";
    }

    public void SetLetter(char letter)
    {
        this.letter.text = letter.ToString();
    }
}