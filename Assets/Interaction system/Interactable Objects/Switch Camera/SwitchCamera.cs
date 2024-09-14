using System.Collections.Generic;
using System.Diagnostics;
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

    private Camera mainCamera; // Reference to the universal main camera

    void Start()
    {
        mainCamera = Camera.main; // Find and assign the universal main camera by tag
        if (mainCamera == null)
        {
            UnityEngine.Debug.LogError("Main camera not found.");
        }
    }

    public void ManageCamera(Camera newCloseUpCamera = null)
    {
        if (currentCameraState == CameraState.Main)
        {
            SetCamera(CameraState.CloseUp, newCloseUpCamera);
        }
        else
        {
            SetCamera(CameraState.Main);
        }
    }

    private void SetCamera(CameraState state, Camera closeUpCamera = null)
    {
        if (mainCamera != null)
        {
            mainCamera.gameObject.SetActive(state == CameraState.Main);
        }

        foreach (var cam in CloseUpCameras)
        {
            if (cam != null)
            {
                cam.gameObject.SetActive(cam == closeUpCamera && state == CameraState.CloseUp);
            }
        }

        currentCameraState = state;
    }
}
