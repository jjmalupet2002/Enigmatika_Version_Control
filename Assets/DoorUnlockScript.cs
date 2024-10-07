using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem; // Add this line

public class DoorUnlockScript : MonoBehaviour
{
    [Tooltip("Check if this object is a button.")]
    public bool IsButton = true;

    [Tooltip("Check if this object is a valve.")]
    public bool IsValve = false;

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

    public void ButtonInteract()
    {
        if (doorObjectHandler != null)
        {
            if (doorObjectHandler.Locked)
            {
                doorObjectHandler.Locked = false; // Unlock the door
                UnityEngine.Debug.Log("Door is now unlocked.");
                // Trigger the button press animation
                buttonAnimator.SetTrigger("ButtonPress");
                // Optionally trigger idle animation after a short delay
                Invoke("OnIdle", 0.2f);
            }
            else
            {
                UnityEngine.Debug.Log("Door is already unlocked.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("No DoorObjectHandler assigned.");
        }
    }

    private void OnIdle()
    {
        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger("ButtonIdle");
        }
    }

    // Placeholder methods for valve and lever interactions
    public void ValveInteract()
    {
        // Placeholder for valve interaction logic
    }

    private void HandleLeverInteraction()
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

    public void LeverInteract()
    {
        // Trigger the LeverUp animation
        leverAnimator.SetTrigger("LeverUp");
        // Call the LeverInteract method from the animation event
        // Ensure this method exists and is called in the animation event
        // You can also invoke OnLeverIdle after a delay if needed
        Invoke("OnLeverIdle", 0.2f); // Adjust the timing as necessary

        if (doorObjectHandler != null && doorObjectHandler.Locked)
        {
            doorObjectHandler.Locked = false; // Unlock the door
            UnityEngine.Debug.Log("Door is now unlocked.");
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
