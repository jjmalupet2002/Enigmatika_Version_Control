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

    private Camera mainCamera; // Reference to the universal main camera

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
    }

    public void ManageCamera(Camera newCloseUpCamera = null)
    {
        if (currentCameraState == CameraState.Main)
        {
            SetCamera(CameraState.CloseUp, newCloseUpCamera);
            backButton.SetActive(true); // Enable the back button when switching to close-up camera
        }
        else
        {
            SetCamera(CameraState.Main);
            backButton.SetActive(false); // Disable the back button when switching back to the main camera
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
                cam.enabled = cam == closeUpCamera && state == CameraState.CloseUp;
            }
            else
            {
                UnityEngine.Debug.LogError("Close-up camera in list is null.");
            }
        }

        currentCameraState = state;
    }

    public void OnBackButtonPressed()
    {
        if (currentCameraState == CameraState.CloseUp)
        {
            ManageCamera(); // Switch back to the main camera
        }
    }
}