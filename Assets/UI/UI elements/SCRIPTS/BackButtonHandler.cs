using System.Collections.Generic;
using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    private List<InteractableObjectHandler> interactableObjects = new List<InteractableObjectHandler>();
    private List<ItemInspectionManager> itemInspectionManagers = new List<ItemInspectionManager>();

    private void Start()
    {
        // Find all instances of ItemInspectionManager in the scene
        itemInspectionManagers.AddRange(FindObjectsOfType<ItemInspectionManager>());

        // Debug log to check if ItemInspectionManagers are found
        if (itemInspectionManagers.Count == 0)
        {
            UnityEngine.Debug.LogWarning("No ItemInspectionManager instances found in the scene.");
        }
    }

    public void RegisterInteractableObject(InteractableObjectHandler interactableObject)
    {
        interactableObjects.Add(interactableObject);
    }

    public void OnBackButtonPressed()
    {
        // Check if any note inspection mode is active and stop inspection if it is
        if (NoteInspectionManager.Instance != null && NoteInspectionManager.Instance.isNoteUIActive)
        {
            foreach (var noteUI in NoteInspectionManager.Instance.noteUIs.Values)
            {
                foreach (var ui in noteUI)
                {
                    if (ui.activeSelf)
                    {
                        ui.SetActive(false); // Hide the note UI
                    }
                }
            }
            NoteInspectionManager.Instance.isNoteUIActive = false; // Reset the flag
            NoteInspectionManager.Instance.EnableNoteInspection(true); // Allow note inspection again

            // Add the toggle for the read button
            NoteUIController.Instance.ToggleReadButton(false);

            // Check if the current note object is tagged as "Book" before closing the book UI
            if (NoteInspectionManager.Instance.currentNoteObject != null &&
                NoteInspectionManager.Instance.currentNoteObject.CompareTag("Book"))
            {
                NoteUIController.Instance.ToggleBookCanvasGroup(); // Close the book canvas group
            }

            return; // Exit early if we closed the note UI
        }

        // Existing logic for other inspection managers
        foreach (var itemInspectionManager in itemInspectionManagers)
        {
            if (itemInspectionManager == null)
            {
                UnityEngine.Debug.LogWarning("ItemInspectionManager is null in OnBackButtonPressed.");
                continue; // Skip to the next manager
            }

            if (itemInspectionManager.IsInspecting())
            {
                itemInspectionManager.StopInspection();
                return; // Exit early after stopping inspection
            }
        }

        // Trigger back button pressed logic in SwitchCamera
        SwitchCamera switchCamera = FindObjectOfType<SwitchCamera>();
        if (switchCamera != null)
        {
            switchCamera.OnBackButtonPressed();
        }
        else
        {
            UnityEngine.Debug.LogWarning("No SwitchCamera instance found in the scene.");
        }

        foreach (var interactableObject in interactableObjects)
        {
            if (interactableObject == null)
            {
                UnityEngine.Debug.LogWarning("InteractableObject is null in OnBackButtonPressed.");
                continue; // Skip to the next interactable object
            }

            interactableObject.CallOnBackButtonPressed();
        }
    }
}