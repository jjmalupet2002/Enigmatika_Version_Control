using System.Collections; // Add this line
using System.Diagnostics;
using UnityEngine;

public class RampObjectHandler : MonoBehaviour
{
    [Tooltip("Reference to the ramp's hinge joint.")]
    public HingeJoint rampHinge;

    private bool isUnlocked = false;

    private void Start()
    {
        LockRamp();
    }

    // Method to lock the ramp
    public void LockRamp()
    {
        isUnlocked = false;

        if (rampHinge != null)
        {
            rampHinge.useMotor = false; // Stop any motor movement
        }

        UnityEngine.Debug.Log("Ramp is now locked.");
    }

    // Method to unlock the ramp
    public void UnlockRamp()
    {
        isUnlocked = true;

        if (rampHinge != null)
        {
            JointMotor motor = rampHinge.motor;
            motor.targetVelocity = 100f; // Adjust the direction based on your setup
            motor.force = 100f;
            rampHinge.motor = motor;
            rampHinge.useMotor = true;

            // Ensure the ramp rotates to 90 degrees
            StartCoroutine(RotateRampToLimit());
        }

        UnityEngine.Debug.Log("Ramp is now unlocked.");
    }

    // Coroutine to smoothly rotate the ramp to its limit
    private IEnumerator RotateRampToLimit()
    {
        while (true)
        {
            // Check if the ramp is not already at 90 degrees
            if (Mathf.Abs(rampHinge.angle) >= 90f)
            {
                // Stop the motor if the ramp has reached the limit
                rampHinge.useMotor = false;
                break; // Exit the loop
            }
            yield return null; // Wait for the next frame
        }

        UnityEngine.Debug.Log("Ramp has reached the limit.");
    }

    // Check if the ramp is unlocked
    public bool IsUnlocked()
    {
        return isUnlocked;
    }
}
