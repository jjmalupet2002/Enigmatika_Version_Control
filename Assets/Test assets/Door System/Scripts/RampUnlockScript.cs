using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class RampUnlockScript : MonoBehaviour
{
    [Tooltip("Reference to the ramp object handler.")]
    public RampObjectHandler rampObjectHandler;

    [Tooltip("Rotation speed of the valve.")]
    public float rotationSpeed = 5f; // Rotation speed of the valve

    // Reference to the input action asset
    public InputActionAsset inputActionAsset; // Ensure this variable is defined

    private float currentRotation = 0f;
    private bool isTouchingValve = false;
    private Vector2 previousTouchPosition;

    // New variable to track if the valve can still spin
    private bool canSpin = true;

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
            previousTouchPosition = touchPosition; // Store the initial touch position
        }
        else if (spinGestureAction.phase == InputActionPhase.Performed && isTouchingValve && canSpin)
        {
            // Start ramp movement only if there is significant spin gesture movement
            if (Vector2.Distance(touchPosition, previousTouchPosition) > 0.1f) // Adjust this threshold as needed
            {
                rampObjectHandler.StartRampMovement(); // Start ramp movement when the valve is being spun
                RotateValve(touchPosition);
                previousTouchPosition = touchPosition; // Update the previous touch position
            }
        }
        else if (spinGestureAction.phase == InputActionPhase.Canceled)
        {
            isTouchingValve = false;
            rampObjectHandler.StopRampMovement(); // Stop ramp movement when the valve stops spinning
            StartCoroutine(ResetValveRotation());
        }

        // Check if the ramp has reached the target rotation and update canSpin
        if (rampObjectHandler.currentRampRotation >= rampObjectHandler.endXRotation)
        {
            canSpin = false; // Disable valve spinning
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

        // Set the valve's rotation
        transform.Rotate(Vector3.forward, rotationAmount);
        UnityEngine.Debug.Log("Valve current rotation: " + currentRotation);

        // Update ramp rotation based on valve rotation
        rampObjectHandler.UpdateRampRotation();
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