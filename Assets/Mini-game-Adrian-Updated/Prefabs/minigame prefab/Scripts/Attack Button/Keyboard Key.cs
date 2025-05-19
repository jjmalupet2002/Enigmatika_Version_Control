using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class KeyboardKey : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private TextMeshProUGUI letterText;

    [Header(" Events ")]
    public static Action<char> onKeyPressed;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SendKeyPressedEvent);
    }

    // Update is called once per frame
    void Update()
    {

    }
    

    private void SendKeyPressedEvent()
    {
        if (letterText != null && letterText.text.Length > 0)
        {
            onKeyPressed?.Invoke(letterText.text[0]);
        }
    }

    public void SetLetter(char letter)
    {
        // Make sure this method is only called on active GameObjects
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true); // Force activate if needed
        }
    
        if (letterText != null)
        {
            letterText.text = letter.ToString();
            Debug.Log($"KeyboardKey set to letter: {letter}");
        }
        else
        {
            Debug.LogError("Letter text is null in KeyboardKey!");
        }
    }
    
    public void ClearKey()
    {
        if (letterText != null)
        {
            letterText.text = " ";
        }
    }
}