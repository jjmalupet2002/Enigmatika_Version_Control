using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class InteractableObjectHandler : MonoBehaviour
{
    public SwitchCamera switchCameraInstance; // Reference to the specific SwitchCamera instance
    private BackButtonHandler backButtonHandler; // Reference to the centralized back button handler

    void Start()
    {
        backButtonHandler = FindObjectOfType<BackButtonHandler>(); // Find and assign the centralized back button handler
        if (backButtonHandler == null)
        {
            UnityEngine.Debug.LogError("BackButtonHandler not found.");
        }
        else
        {
            backButtonHandler.RegisterInteractableObject(this); // Register this interactable object with the back button handler
        }
}

    public void CallOnBackButtonPressed()
    {
        switchCameraInstance.OnBackButtonPressed();
    }
}