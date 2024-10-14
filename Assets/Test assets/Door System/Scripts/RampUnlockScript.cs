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
        // Check if the close-up camera is active
        if (!IsCloseUpCameraActive())
        {
           
            return; // Skip interaction if close-up camera is not active
        }

        var spinGestureAction = inputActionAsset.FindAction("SpinGesture");

        if (spinGestureAction == null)
        {
            UnityEngine.Debug.LogError("SpinGesture action not found in Input Action Asset.");
            return; // Early exit if the action is not found
        }

        Vector2 touchPosition = spinGestureAction.ReadValue<Vector2>();

        // Check if the touch is hitting the valve collider on every update
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isTouchingValve = true; // Set to true if touching the valve
            }
            else
            {
                isTouchingValve = false; // Reset if not touching the valve
            }
        }

        // Now check the input phases
        if (spinGestureAction.phase == InputActionPhase.Started && isTouchingValve)
        {
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
