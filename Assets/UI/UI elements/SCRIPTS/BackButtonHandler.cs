using System.Collections.Generic;
using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    private List<InteractableObjectHandler> interactableObjects = new List<InteractableObjectHandler>();

    public void RegisterInteractableObject(InteractableObjectHandler interactableObject)
    {
        interactableObjects.Add(interactableObject);
    }

    public void OnBackButtonPressed()
    {
        // Trigger back button pressed logic in SwitchCamera
        SwitchCamera switchCamera = FindObjectOfType<SwitchCamera>();
        if (switchCamera != null)
        {
            switchCamera.OnBackButtonPressed();
        }

        foreach (var interactableObject in interactableObjects)
        {
            interactableObject.CallOnBackButtonPressed();
        }
    }
}
