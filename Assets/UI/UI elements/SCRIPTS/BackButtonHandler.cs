using System.Collections.Generic;
using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    private List<InteractableObjectHandler> interactableObjects = new List<InteractableObjectHandler>();
    private CloseUpViewUIController closeUpViewUIController;
    private ItemInspectionManager itemInspectionManager;

    private void Start()
    {
        // Get instances of the managers
        closeUpViewUIController = FindObjectOfType<CloseUpViewUIController>();
        itemInspectionManager = FindObjectOfType<ItemInspectionManager>();
    }

    public void RegisterInteractableObject(InteractableObjectHandler interactableObject)
    {
        interactableObjects.Add(interactableObject);
    }

    public void OnBackButtonPressed()
    {
        // Check if inspection mode is active and stop inspection if it is
        if (itemInspectionManager != null && itemInspectionManager.IsInspecting())
        {
            itemInspectionManager.StopInspection();

            // Disable the UI when exiting inspection mode
            closeUpViewUIController.SetUIActive(false);
            return; // Exit early to prevent closing the close-up view
        }

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

