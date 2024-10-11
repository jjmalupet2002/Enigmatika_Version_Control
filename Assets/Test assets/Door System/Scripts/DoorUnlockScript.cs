using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem; // Add this line

public class DoorUnlockScript : MonoBehaviour
{
    [Tooltip("Check if this object is a button.")]
    public bool IsButton = true;

    [Tooltip("Check if this object is a lever.")]
    public bool IsLever = false;

    [Tooltip("Animator controller for button animations.")]
    public Animator buttonAnimator;

    [Tooltip("Animator controller for lever animations.")]
    public Animator leverAnimator;

    [Tooltip("Reference to the door object handler.")]
    public DoorObjectHandler doorObjectHandler;

    // Reference to the input action asset
    public InputActionAsset inputActionAsset; // Add this line

    private void OnEnable()
    {
        // Enable the swipe input action
        inputActionAsset.FindAction("SwipeUp").Enable();
    }

    private void OnDisable()
    {
        // Disable the swipe input action
        inputActionAsset.FindAction("SwipeUp").Disable();
    }

    void Update()
    {
        // Check for button press through raycasting
        if (IsButton && Input.GetMouseButtonDown(0)) // 0 for left mouse button
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    ButtonInteract();
                }
            }
        }

        // Check for lever interaction
        if (IsLever)
        {
            HandleLeverInteraction();
        }
    }

    private bool isLockedTextVisible = false; // Track the visibility of the locked text

    public void ButtonInteract()
    {
        // Check if any SwitchCamera instance has the CloseUp camera active
        if (IsCloseUpCameraActive())
        {
            if (doorObjectHandler != null)
            {
                // Only call the UnlockDoor method if the door is locked and locked text is not visible
                if (doorObjectHandler.Locked && !isLockedTextVisible)
                {
                    doorObjectHandler.UnlockDoor(); // Unlocks the door and handles text display
                    buttonAnimator.SetTrigger("ButtonPress");
                    Invoke("OnIdle", 0.2f);
                }
                else if (!doorObjectHandler.Locked)
                {
                    UnityEngine.Debug.Log("Door is already unlocked.");
                }
            }
            else
            {
                UnityEngine.Debug.LogError("No DoorObjectHandler assigned.");
            }
        }
        else
        {
            UnityEngine.Debug.Log("Cannot interact with the door; close-up camera is not active.");
        }
    }

    // Ensure to set the locked text visibility to true when displaying it
    public void ShowLockedText()
    {
        isLockedTextVisible = true;
        // Your logic to display the locked text here
    }

    // Call this method when the locked text fade-out animation finishes
    public void HideLockedText()
    {
        isLockedTextVisible = false;
        // Your logic to hide the locked text here
    }



    private void HandleLeverInteraction()
    {
        // Check if any SwitchCamera instance has the CloseUp camera active
        if (IsCloseUpCameraActive())
        {
            // Get the swipe input vector
            Vector2 swipeInput = inputActionAsset.FindAction("SwipeUp").ReadValue<Vector2>();

            // Check if the swipe input is significant (you can adjust the threshold)
            if (swipeInput.y > 0.5f) // Threshold for swiping up
            {
                // Check for lever interaction through raycasting
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        LeverInteract();
                    }
                }
            }
        }
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

    public void LeverInteract()
    {
        // Trigger the LeverUp animation
        leverAnimator.SetTrigger("LeverUp");

        // Invoke the OnLeverIdle method after a short delay
        Invoke("OnLeverIdle", 0.2f); // Adjust the timing as necessary

        // Check if the doorObjectHandler is not null and if the door is locked
        if (doorObjectHandler != null && doorObjectHandler.Locked)
        {
            doorObjectHandler.UnlockDoor(); // Call the UnlockDoor method
        }
        else if (doorObjectHandler != null)
        {
            UnityEngine.Debug.Log("Door is already unlocked.");
        }
    }

    private void OnIdle()
    {
        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger("ButtonIdle");
        }
    }

    private void OnLeverIdle()
    {
        if (leverAnimator != null)
        {
            leverAnimator.SetTrigger("LeverIdle");
        }
    }
}

