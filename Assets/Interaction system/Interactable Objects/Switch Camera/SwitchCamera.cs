using System.Collections.Generic;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    // Dictionary to hold camera views, with unique identifiers for each
    public Dictionary<int, Camera> cameras = new Dictionary<int, Camera>();

    // Public field for the close-up view camera to be set in the Unity Inspector
    public Camera closeUpCamera; // Example for a close-up camera

    private int currentCameraId;
    private Camera defaultCamera;

    void Start()
    {
        // Find and set the default camera
        GameObject defaultCameraObject = GameObject.FindGameObjectWithTag("MainCamera");
        if (defaultCameraObject != null)
        {
            defaultCamera = defaultCameraObject.GetComponent<Camera>();
            if (defaultCamera != null)
            {
                // Register the default camera with ID 1
                RegisterCamera(1, defaultCamera);
                currentCameraId = 1; // Set default camera ID
                SetCamera(currentCameraId);
            }
            else
            {
                Debug.LogError("No Camera component found on the MainCamera object.");
            }
        }
        else
        {
            Debug.LogError("MainCamera object not found.");
        }

        // Register the close-up camera if assigned
        if (closeUpCamera != null)
        {
            RegisterCamera(2, closeUpCamera); // Assign a unique ID for the close-up camera
        }
    }

    public void SetCamera(int id)
    {
        // Deactivate all cameras
        foreach (var cam in cameras.Values)
        {
            if (cam != null)
            {
                cam.gameObject.SetActive(false);
            }
        }

        // Activate the selected camera
        if (cameras.TryGetValue(id, out Camera camera))
        {
            camera.gameObject.SetActive(true);
            Debug.Log($"Camera {id} is now active.");
        }
        else
        {
            Debug.LogError($"Camera with ID {id} not found.");
        }
    }

    public void SwitchToNextCamera()
    {
        // Logic to cycle through cameras
        currentCameraId = (currentCameraId % cameras.Count) + 1;
        SetCamera(currentCameraId);
    }

    public void RegisterCamera(int id, Camera camera)
    {
        if (!cameras.ContainsKey(id))
        {
            cameras.Add(id, camera);
        }
    }
}
