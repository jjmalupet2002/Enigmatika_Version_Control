using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        KeyboardKey.onkeyPressed += DebugLetter;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DebugLetter(string letter)
    {
        Debug.Log(letter);
    }
}