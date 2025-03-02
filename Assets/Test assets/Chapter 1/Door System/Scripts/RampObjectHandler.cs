using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class RampObjectHandler : MonoBehaviour
{
    [Tooltip("Starting rotation angle on the X axis.")]
    public float startXRotation = 0f; // Starting angle (lowest position)

    [Tooltip("Ending rotation angle on the X axis.")]
    public float endXRotation = 90f;   // Ending angle (maximum position)

    [Tooltip("Speed at which the ramp moves down.")]
    public float rampDownSpeed = 2f;   // Speed at which the ramp moves down

    private bool isValveSpinning = false; // Track if the valve is spinning
    public float currentRampRotation;

    private void Start()
    {
        LockRamp();
    }

    public void LockRamp()
    {
        currentRampRotation = startXRotation;
        transform.rotation = Quaternion.Euler(startXRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    public void StartRampMovement()
    {
        isValveSpinning = true;
    }

    public void StopRampMovement()
    {
        isValveSpinning = false;
    }

    public void UpdateRampRotation()
    {
        if (isValveSpinning)
        {
            // Calculate the difference between the current rotation and the end rotation
            float angleDifference = endXRotation - currentRampRotation;

            // If the angle difference is less than a small threshold, stop the ramp
            if (Mathf.Abs(angleDifference) < 0.01f)
            {
                currentRampRotation = endXRotation; // Snap to the end rotation
                StopRampMovement(); // Stop the ramp movement
                UnityEngine.Debug.Log("Ramp has reached the limit.");
                return; // Exit the method to avoid further rotation updates
            }

            // Move towards the end rotation at a consistent speed
            float step = rampDownSpeed * Time.deltaTime; // Fixed speed

            // Smoothly move the current ramp rotation towards the target with a small lerp effect
            currentRampRotation += Mathf.Sign(angleDifference) * Mathf.Min(step, Mathf.Abs(angleDifference)); // Move towards the target

            // Set the ramp's rotation to the updated rotation
            transform.rotation = Quaternion.Euler(currentRampRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }

}