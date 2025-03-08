using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera closeUpCamera; // Assign the close-up camera in the Inspector
    public Camera topDownCamera; // Assign the top-down camera in the Inspector

    public void SwitchToCloseUp()
    {
        // Enable the close-up camera, disable the top-down camera
        closeUpCamera.enabled = true;
        topDownCamera.enabled = false;
    }

    public void SwitchToTopDown()
    {
        // Enable the top-down camera, disable the close-up camera
        closeUpCamera.enabled = false;
        topDownCamera.enabled = true;
    }

    public void HandlePlayerAnswer(bool isCorrect)
    {
        if (isCorrect)
        {
            SwitchToCloseUp(); // Stay in close-up for correct answers
        }
        else
        {
            SwitchToTopDown(); // Switch to top-down for incorrect answers
        }
    }
}