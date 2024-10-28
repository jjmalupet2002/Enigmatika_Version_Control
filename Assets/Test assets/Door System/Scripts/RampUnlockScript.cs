using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class RampUnlockScript : MonoBehaviour
{
    [Tooltip("Reference to the ramp object handler.")]
    public RampObjectHandler rampObjectHandler;

    [Tooltip("Rotation speed of the valve.")]
    public float rotationSpeed = 5f;

    public InputActionAsset inputActionAsset;
    public AudioSource rampAudioSource;

    private float currentRotation = 0f;
    private bool isTouchingValve = false;
    private Vector2 previousTouchPosition;
    private bool isValveSpinning = false;
    private bool canSpin = true;

    private float spinStopTimer = 0f;
    private float spinCooldown = 0.2f;
    private float audioPlaybackPosition = 0f;

    private void OnEnable()
    {
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
        if (inputActionAsset != null)
        {
            inputActionAsset.FindAction("SpinGesture").Disable();
        }
    }

    void Update()
    {
        if (IsCloseUpCameraActive())
        {
            HandleValveInteraction();
        }
        else
        {
            ResetValveState(); // Reset values if not in close-up view
        }

        // Real-time audio control based on valve spinning status with delay
        if (isValveSpinning)
        {
            if (!rampAudioSource.isPlaying)
            {
                rampAudioSource.time = audioPlaybackPosition; // Resume from saved position
                rampAudioSource.Play();
            }
            spinStopTimer = 0f; // Reset timer while spinning
        }
        else
        {
            spinStopTimer += Time.deltaTime;
            if (spinStopTimer > spinCooldown && rampAudioSource.isPlaying)
            {
                audioPlaybackPosition = rampAudioSource.time; // Save current playback position
                rampAudioSource.Stop();
            }
        }
    }

    private void HandleValveInteraction()
    {
        var spinGestureAction = inputActionAsset.FindAction("SpinGesture");
        if (spinGestureAction == null)
        {
            UnityEngine.Debug.LogError("SpinGesture action not found in Input Action Asset.");
            return;
        }

        Vector2 touchPosition = spinGestureAction.ReadValue<Vector2>();

        // Check if touch hits the valve collider
        Ray ray = Camera.main.ScreenPointToRay(touchPosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            isTouchingValve = hit.collider != null && hit.collider.gameObject == gameObject;
        }

        isValveSpinning = false; // Reset each frame

        if (spinGestureAction.phase == InputActionPhase.Started && isTouchingValve)
        {
            previousTouchPosition = touchPosition;
        }
        else if (spinGestureAction.phase == InputActionPhase.Performed && isTouchingValve && canSpin)
        {
            if (Vector2.Distance(touchPosition, previousTouchPosition) > 0.1f)
            {
                rampObjectHandler.StartRampMovement();
                RotateValve(touchPosition);
                isValveSpinning = true;
                previousTouchPosition = touchPosition;
            }
        }
        else if (spinGestureAction.phase == InputActionPhase.Canceled)
        {
            isTouchingValve = false;
            rampObjectHandler.StopRampMovement();
            StartCoroutine(ResetValveRotation());
        }

        if (rampObjectHandler.currentRampRotation >= rampObjectHandler.endXRotation)
        {
            canSpin = false;
        }
    }

    private void RotateValve(Vector2 touchPosition)
    {
        Vector2 deltaPosition = touchPosition - previousTouchPosition;
        float rotationAmount = deltaPosition.magnitude * rotationSpeed * Time.deltaTime;

        // Check rotation direction
        if (Vector2.Dot(deltaPosition, Vector2.right) > 0)
        {
            currentRotation += rotationAmount;
        }
        else
        {
            currentRotation -= rotationAmount;
        }

        // Apply rotation
        transform.Rotate(Vector3.forward, rotationAmount);
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
        var switchCameras = FindObjectsOfType<SwitchCamera>();
        foreach (var switchCamera in switchCameras)
        {
            if (switchCamera.currentCameraState == CameraState.CloseUp)
            {
                return true;
            }
        }
        return false;
    }

    private void ResetValveState()
    {
        isTouchingValve = false;
        isValveSpinning = false;
        previousTouchPosition = Vector2.zero;
        rampObjectHandler.StopRampMovement();
        currentRotation = 0;
        spinStopTimer = 0f; // Reset timer when exiting close-up
    }
}
