using System.Collections;
using UnityEngine;

public class RampObjectHandler : MonoBehaviour
{
    [Tooltip("Starting rotation angle on the X axis.")]
    public float startXRotation = 0f; // Starting angle (lowest position)

    [Tooltip("Ending rotation angle on the X axis.")]
    public float endXRotation = 90f;   // Ending angle (maximum position)

    [Tooltip("Speed at which the ramp goes down.")]
    public float rampDownSpeed = 2f;   // Speed at which the ramp returns to the starting position

    private bool isUnlocked = false;
    private float targetRampRotation;

    private void Start()
    {
        LockRamp();
    }

    public void LockRamp()
    {
        isUnlocked = false;
        transform.rotation = Quaternion.Euler(startXRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
       
    }

    public void UnlockRamp(float valveRotation, float unlockThreshold)
    {
        isUnlocked = true;
        targetRampRotation = Mathf.Lerp(startXRotation, endXRotation, valveRotation / unlockThreshold);
        StartCoroutine(RotateRampToLimit(valveRotation));
   
    }

    private IEnumerator RotateRampToLimit(float valveRotation)
    {
        float elapsedTime = 0f;
        float duration = 2f; // Duration of the ramp rotation

        // Ramp down based on the valve rotation
        while (elapsedTime < duration)
        {
            // Lerp the angle between the current angle and the target angle
            float newAngle = Mathf.Lerp(startXRotation, targetRampRotation, elapsedTime / duration);

            // Apply the new angle to the ramp's rotation
            transform.rotation = Quaternion.Euler(newAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the ramp is set to the final ending angle
        transform.rotation = Quaternion.Euler(targetRampRotation, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        UnityEngine.Debug.Log("Ramp has reached the limit.");
        yield return new WaitForSeconds(1f); // Optional delay
    }

    public bool IsUnlocked()
    {
        return isUnlocked;
    }
}
