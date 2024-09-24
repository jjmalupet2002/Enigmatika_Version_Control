using System.Collections;
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
        foreach (var interactableObject in interactableObjects)
        {
            interactableObject.CallOnBackButtonPressed();
        }
    }
}