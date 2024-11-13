using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedPortalRoomDoorUI : MonoBehaviour
{
    // Reference to the UI GameObject
    public GameObject uiObject;

    // Reference to the PortalRoomCamera
    public Camera portalRoomCamera;

    private void Update()
    {
        // Check if the portalRoomCamera is active and enabled
        if (portalRoomCamera != null)
        {
            // Enable or disable the UI GameObject based on the camera's active state
            uiObject.SetActive(portalRoomCamera.isActiveAndEnabled);
        }
    }
}

