using System.Collections;
using System.Diagnostics;
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
        var spinGestureAction = inputActionAsset.FindAction("SpinGesture");

        if (spinGestureAction == null)
        {
            UnityEngine.Debug.LogError("SpinGesture action not found in Input Action Asset.");
            return; // Early exit if the action is not found
        }

        Vector2 touchPosition = spinGestureAction.ReadValue<Vector2>();

        if (spinGestureAction.phase == InputActionPhase.Started)
        {
            isTouchingValve = true;
            previousTouchPosition = touchPosition;
        }
        else if (spinGestureAction.phase == InputActionPhase.Performed && isTouchingValve && !isUnlocked) // Prevent rotation if unlocked
        {
            RotateValve(touchPosition);
            previousTouchPosition = touchPosition;
        }
        else if (spinGestureAction.phase == InputActionPhase.Canceled)
        {
            isTouchingValve = false;
            StartCoroutine(ResetValveRotation());
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

        transform.Rotate(Vector3.forward, rotationAmount);
        UnityEngine.Debug.Log("Valve current rotation: " + currentRotation);

        if (currentRotation >= unlockThreshold)
        {
            if (rampObjectHandler != null && !rampObjectHandler.IsUnlocked())
            {
                rampObjectHandler.UnlockRamp();
                isUnlocked = true; // Set the unlocked flag to true
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
}
