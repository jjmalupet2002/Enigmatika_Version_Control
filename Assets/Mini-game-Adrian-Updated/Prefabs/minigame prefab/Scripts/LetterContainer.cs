using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LetterContainer : MonoBehaviour
{
    [Header("TextMeshPro Letter")]
    [SerializeField] private TextMeshProUGUI letter;

    public void Initialize()
    {
        letter.text = "";
    }

    public void SetLetter(char letter)
    {
        this.letter.text = letter.ToString();
    }

    public char GetLetter()
    {
        // Check if text is empty to avoid index out of range errors
        return letter.text.Length > 0 ? letter.text[0] : ' ';
    }
}