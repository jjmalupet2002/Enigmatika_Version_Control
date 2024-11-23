using UnityEngine;
using UnityEngine.UI;

public class SimpleTimer : MonoBehaviour
{
    public float timeElapsed = 0f; // Track the time passed since the start of the game
    public bool isTimerActive = false; // Boolean to control when the timer starts
    public Text timeText; // Reference to the UI Text element to display the timer
    private bool isCloseUpActive = false; // Track if close-up camera is active

    void Update()
    {
        // Update close-up camera status
        isCloseUpActive = IsCloseUpCameraActive();

        // Update timer only if the timer is active
        if (isTimerActive)
        {
            timeElapsed += Time.deltaTime;
        }

        // Only display the timer text if the close-up camera is NOT active
        if (!isCloseUpActive)
        {
            DisplayTime(timeElapsed);
            timeText.gameObject.SetActive(true); // Ensure timer text is visible when needed
        }
        else
        {
            timeText.gameObject.SetActive(false); // Hide the timer text if close-up camera is active
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1; // Optional: Add 1 for more accurate display rounding

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Method to check if any CloseUp camera is active
    private bool IsCloseUpCameraActive()
    {
        // Get all instances of SwitchCamera
        var switchCameras = FindObjectsOfType<SwitchCamera>();
        // Check if any instance has the CloseUp camera active
        foreach (var switchCamera in switchCameras)
        {
            if (switchCamera.currentCameraState == CameraState.CloseUp)
            {
                return true; // Return true if any close-up camera is active
            }
        }
        return false; // No close-up camera is active
    }

    // Call this method from another script to start the timer
    public void StartTimer()
    {
        isTimerActive = true;
    }

    // Call this method to stop the timer
    public void StopTimer()
    {
        isTimerActive = false;
    }
}
