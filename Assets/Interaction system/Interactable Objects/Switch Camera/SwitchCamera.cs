using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    Main,
    CloseUp
}

public class SwitchCamera : MonoBehaviour
{
    public List<Camera> CloseUpCameras = new List<Camera>(); // List to store close-up cameras
    public CameraState currentCameraState = CameraState.Main; // Current camera state
    public GameObject backButton; // Reference to the back button UI object
    public InteractableObjectHandler interactableObject; // Reference to the TableModel script for handling interactions

    private Camera mainCamera; // Reference to the universal main camera
    private TalkandInteract talkAndInteract; // Reference to TalkandInteract script
    private ProximityOutline[] proximityOutlines; // Array to store all ProximityOutline components
    private Camera currentCloseUpCamera; // Current close-up camera

    void Start()
    {
        mainCamera = Camera.main; // Find and assign the universal main camera by tag
        if (mainCamera == null)
        {
            UnityEngine.Debug.LogError("Main camera not found.");
        }

        if (backButton == null)
        {
            UnityEngine.Debug.LogError("Back button UI object not assigned.");
        }

        talkAndInteract = FindObjectOfType<TalkandInteract>(); // Find and assign the TalkandInteract script
        if (talkAndInteract == null)
        {
            UnityEngine.Debug.LogError("TalkandInteract script not found.");
        }

        proximityOutlines = FindObjectsOfType<ProximityOutline>(); // Find all ProximityOutline components
    }

    public void ManageCamera(Camera newCloseUpCamera = null)
    {
        if (currentCameraState == CameraState.Main)
        {
            SetCamera(CameraState.CloseUp, newCloseUpCamera);
            currentCloseUpCamera = newCloseUpCamera; // Update current close-up camera
            backButton.SetActive(true); // Enable the back button when switching to close-up camera
            ToggleOutlines(false); // Disable outlines
            GameStateManager.Instance.DisableUIElements(); // Disable UI elements
        }
        else
        {
            SetCamera(CameraState.Main); // Switch back to the main camera
            backButton.SetActive(false); // Disable the back button when switching back to the main camera
            ToggleOutlines(true); // Enable outlines
            GameStateManager.Instance.EnableUIElements(); // Enable UI elements

            // Reset interactionProcessed flag in TalkandInteract script
            if (talkAndInteract != null)
            {
                talkAndInteract.OnInteractButtonPressed();
                talkAndInteract.interactionProcessed = false; // Reset the interactionProcessed flag to false
            }

            currentCloseUpCamera = null; // Reset current close-up camera
        }
    }

    private void SetCamera(CameraState state, Camera closeUpCamera = null)
    {
        if (mainCamera != null)
        {
            mainCamera.enabled = state == CameraState.Main;
        }
        else
        {
            UnityEngine.Debug.LogError("Main camera is null.");
        }

        foreach (var cam in CloseUpCameras)
        {
            if (cam != null)
            {
                cam.enabled = state == CameraState.CloseUp && cam == closeUpCamera;
            }
            else
            {
                UnityEngine.Debug.LogError("Close-up camera in list is null.");
            }
        }

        currentCameraState = state;
    }

    private void ToggleOutlines(bool enable)
    {
        foreach (var proximityOutline in proximityOutlines)
        {
            if (proximityOutline != null)
            {
                proximityOutline.ForceDisableOutline(!enable); // Toggle the outline based on the camera state
            }
            else
            {
                UnityEngine.Debug.LogError("ProximityOutline component is null.");
            }
        }
    }

    public void OnBackButtonPressed()
    {
        if (currentCameraState == CameraState.CloseUp)
        {
            ManageCamera(); // Switch back to the main camera
        }
    }
}
