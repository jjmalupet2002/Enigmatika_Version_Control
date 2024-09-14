using System.Diagnostics;
using UnityEngine;

public class SwitchCamera : MonoBehaviour
{
    public GameObject Camera_1; // Main isometric camera
    public GameObject Camera_2; // Close-up camera
    private bool isCloseUp = false;

    // Call this method to switch cameras
    public void ManageCamera()
    {
        UnityEngine.Debug.Log("ManageCamera called");
        if (isCloseUp)
        {
            SetIsometricView();
        }
        else
        {
            SetCloseUpView();
        }
        isCloseUp = !isCloseUp;
    }

    void SetIsometricView()
    {
        UnityEngine.Debug.Log("Switching to Isometric View");
        if (Camera_1 != null && Camera_2 != null)
        {
            Camera_1.SetActive(true);
            Camera_2.SetActive(false);
        }
        else
        {
            UnityEngine.Debug.LogError("Camera_1 or Camera_2 is not assigned.");
        }
    }

    void SetCloseUpView()
    {
        UnityEngine.Debug.Log("Switching to Close-Up View");
        if (Camera_1 != null && Camera_2 != null)
        {
            Camera_1.SetActive(false);
            Camera_2.SetActive(true);
        }
        else
        {
            UnityEngine.Debug.LogError("Camera_1 or Camera_2 is not assigned.");
        }
    }
}
