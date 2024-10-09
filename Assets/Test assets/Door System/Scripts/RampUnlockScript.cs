using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RampUnlockScript : MonoBehaviour
{
    [Tooltip("Reference to the ramp object handler.")]
    public RampObjectHandler rampObjectHandler;

    [Tooltip("Rotation threshold to unlock the ramp.")]
    public float unlockThreshold = 50f; // Rotation threshold to unlock the ramp

    [Tooltip("Rotation speed of the valve.")]
    public float rotationSpeed = 5f; // Rotation speed of the valve

    // Reference to the input action asset
    public InputActionAsset inputActionAsset; // Ensure this variable is defined

    private float currentRotation = 0f;
    private bool isTouchingValve = false;
    private bool isUnlocked = false; // New flag to track if the valve is unlocked
    private Vector2 previousTouchPosition;

    private void OnEnable()
    {
        // Enable the spin gesture input action
        if (inputActionAsset != null)
        {
            inputActionAsset.FindAction("SpinGesture").Enable();
        }
        else
        {
            UnityEngine.Debug.LogError("Input Action Asset is not assigned!");
        }
    }

    private void OnDisable()
    {
        // Disable the spin gesture input action
        if (inputActionAsset != null)
        {
            inputActionAsset.FindAction("SpinGesture").Disable();
        }
    }

    void Update()
    {
        HandleValveInteraction();
    }

    private void HandleValveInteraction()
    {
        // Always check if the close-up camera is active, even if it's already unlocked
        if (IsCloseUpCameraActive())
        {
            // If close-up camera is active, allow spinning the valve
            HandleSpinGesture();
        }
    }

    private void HandleSpinGesture()
    {
        var spinGestureAction = inputActionAsset.FindAction("SpinGesture");
        if (spinGestureAction == null)
        {
            UnityEngine.Debug.LogError("SpinGesture action not found in Input Action Asset.");
            return; // Early exit if the action is not found
        }

        Vector2 touchPosition = spinGestureAction.ReadValue<Vector2>();

        // Handle input phases for valve spinning
        switch (spinGestureAction.phase)
        {
            case InputActionPhase.Started:
                isTouchingValve = true;
                previousTouchPosition = touchPosition;
                UnityEngine.Debug.Log("Started touching valve.");
                break;

            case InputActionPhase.Performed:
                if (isTouchingValve)
                {
                    RotateValve(touchPosition);
                    previousTouchPosition = touchPosition;
                }
                break;

            case InputActionPhase.Canceled:
                isTouchingValve = false;
                StartCoroutine(ResetValveRotation());
                UnityEngine.Debug.Log("Stopped touching valve.");
                break;
        }
    }

    private void RotateValve(Vector2 touchPosition)
    {
        Vector2 deltaPosition = touchPosition - previousTouchPosition;
        float rotationAmount = deltaPosition.magnitude * rotationSpeed * Time.deltaTime;

        // Determine the rotation direction based on the touch movement
        if (Vector2.Dot(deltaPosition, Vector2.right) > 0)
        {
            currentRotation += rotationAmount;
        }
        else
        {
            currentRotation -= rotationAmount;
        }

        // Limit the rotation to a specific range
        currentRotation = Mathf.Clamp(currentRotation, 0, unlockThreshold);
        transform.Rotate(Vector3.forward, rotationAmount);
        UnityEngine.Debug.Log("Valve current rotation: " + currentRotation);

        // Check if the current rotation is enough to unlock the ramp
        if (currentRotation >= unlockThreshold)
        {
            if (rampObjectHandler != null && !isUnlocked) // Use isUnlocked to prevent re-unlocking
            {
                rampObjectHandler.UnlockRamp(currentRotation, unlockThreshold);
                isUnlocked = true; // Set the unlocked flag to true
                UnityEngine.Debug.Log("Ramp unlocked!");
            }
            else
            {
                UnityEngine.Debug.Log("Ramp is already unlocked!");
            }
        }
    }

    private IEnumerator ResetValveRotation()
    {
        while (currentRotation > 0)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;
            currentRotation -= rotationAmount;
            transform.Rotate(Vector3.forward, -rotationAmount);
            yield return null;
        }

        currentRotation = 0;
    }

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
}
