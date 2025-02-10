using System;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeAmount = 0.1f; // Amount of shake
    public float shakeDuration = 1f; // Duration of the shake
    private Vector3 originalPosition;
    private float shakeTime;

    private void Start()
    {
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (shakeTime > 0)
        {
            // Generate random shake offsets
            Vector3 shakeOffset = UnityEngine.Random.insideUnitSphere * shakeAmount;

            // Apply the shake offset to the camera position
            transform.position = originalPosition + shakeOffset;

            // Reduce shake time
            shakeTime -= Time.deltaTime;
        }
        else
        {
            // Reset the camera position to the original position
            transform.position = originalPosition;
        }
    }

    public void Shake(float amount, float duration)
    {
        shakeAmount = amount;
        shakeDuration = duration;
        shakeTime = duration;
    }
}

//Usage: Call Shake(float amount, float duration) from your game logic when you want to trigger a shake effect.