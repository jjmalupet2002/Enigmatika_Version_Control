using UnityEngine;
using UnityEngine.InputSystem;

public class DrawerChestUnlock : MonoBehaviour
{
    [Tooltip("Check if this object is a Book.")]
    public bool IsButton = true;

    [Tooltip("Check if this object is a lever.")]
    public bool IsLever = false;

    [Tooltip("Animator controller for Book animations.")]
    public Animator buttonAnimator;

    [Tooltip("Animator controller for lever animations.")]
    public Animator leverAnimator;

    [Tooltip("Reference to the drawer or chest object handler.")]
    public InteractableDrawerClosetChest drawerChestHandler;

    [Tooltip("UI GameObject for showing unlocked chest.")]
    public GameObject unlockedChestUI;

    [Tooltip("UI GameObject for showing unlocked drawer.")]
    public GameObject unlockedDrawerUI;

    // Reference to the input action asset
    public InputActionAsset inputActionAsset;

    private bool isInteracting = false; // Prevents multiple interactions

    private void OnEnable()
    {
        // Enable the swipe input action
        inputActionAsset.FindAction("SwipeUp").Enable();

        // Start the BookButton in idle animation
        if (IsButton && buttonAnimator != null)
        {
            buttonAnimator.SetTrigger("BookIdle"); // Start idle animation
        }
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
            HandleButtonInteraction();
        }

        // Check for lever interaction
        if (IsLever && !isInteracting) // Ensure not already interacting
        {
            HandleLeverInteraction();
        }
    }

    private void HandleButtonInteraction()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            if (IsCloseUpCameraActive())
            {
                UnlockIfApplicable();
            }
            else
            {
                Debug.Log("Cannot interact; close-up camera is not active.");
            }
        }
    }

    private void HandleLeverInteraction()
    {
        if (IsCloseUpCameraActive())
        {
            Vector2 swipeInput = inputActionAsset.FindAction("SwipeUp").ReadValue<Vector2>();

            if (swipeInput.y > 0.5f) // Threshold for swiping up
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
                {
                    LeverInteract();
                }
            }
        }
    }

    private void UnlockIfApplicable()
    {
        if (drawerChestHandler != null)
        {
            // Check if it's a drawer or a chest and unlock accordingly
            if (drawerChestHandler.isDrawerLocked && drawerChestHandler.isDrawer) // Unlock drawer
            {
                UnlockDrawer();
            }
            else if (drawerChestHandler.isChestLocked && drawerChestHandler.isChest) // Unlock chest
            {
                UnlockChest();
            }
            else
            {
                Debug.Log("The selected object is already unlocked.");
            }
        }
        else
        {
            Debug.LogError("No InteractableDrawerChest assigned.");
        }
    }

    private void UnlockDrawer()
    {
        ShowUnlockUI(true); // Show the unlocked drawer UI
        drawerChestHandler.onUnlockDrawer.Invoke(); // Invoke unlock event
        drawerChestHandler.isDrawerLocked = false; // Unlock the drawer

        // Handle button animator if it's a button
        if (IsButton)
        {
            if (buttonAnimator != null)
            {
                buttonAnimator.SetTrigger("BookButtonDown"); // Trigger button press
                Invoke("BookButtonUp", 0.3f); // Automatically trigger BookButtonUp after a short delay
            }
            else
            {
                Debug.LogWarning("buttonAnimator is not assigned. Cannot play button press animation.");
            }
        }

        // Handle lever animator if it's a lever
        if (IsLever)
        {
            if (leverAnimator != null)
            {
                leverAnimator.SetTrigger("LeverUp");
                Invoke("OnLeverIdle", 0.2f);
            }
            else
            {
                Debug.LogWarning("leverAnimator is not assigned. Cannot play lever up animation.");
            }
        }
    }

    private void UnlockChest()
    {
        ShowUnlockUI(false); // Show unlocked chest UI
        drawerChestHandler.onUnlockChest.Invoke(); // Invoke unlock event
        drawerChestHandler.isChestLocked = false; // Unlock the chest

        // Handle button animator if it's a button
        if (IsButton)
        {
            if (buttonAnimator != null)
            {
                buttonAnimator.SetTrigger("BookButtonDown"); // Trigger button press
                Invoke("BookButtonUp", 0.3f); // Automatically trigger BookButtonUp after a short delay
            }
            else
            {
                Debug.LogWarning("buttonAnimator is not assigned. Cannot play button press animation.");
            }
        }

        // Handle lever animator if it's a lever
        if (IsLever)
        {
            if (leverAnimator != null)
            {
                leverAnimator.SetTrigger("LeverUp");
                Invoke("OnLeverIdle", 0.2f);
            }
            else
            {
                Debug.LogWarning("leverAnimator is not assigned. Cannot play lever up animation.");
            }
        }
    }

    private void ShowUnlockUI(bool isDrawer)
    {
        if (isDrawer)
        {
            unlockedDrawerUI.SetActive(true);
            Invoke("HideUnlockUI", 1.5f);
        }
        else
        {
            unlockedChestUI.SetActive(true);
            Invoke("HideUnlockUI", 1.5f);
        }
    }

    private void HideUnlockUI()
    {
        unlockedChestUI.SetActive(false);
        unlockedDrawerUI.SetActive(false);
    }

    public void LeverInteract()
    {
        isInteracting = true; // Prevent multiple interactions
        Invoke("ResetInteraction", 1f); // Adjust delay as necessary

        // Call UnlockIfApplicable to handle unlocking logic for levers as well
        UnlockIfApplicable();
    }

    private bool IsCloseUpCameraActive()
    {
        var switchCameras = FindObjectsOfType<SwitchCamera>();
        foreach (var switchCamera in switchCameras)
        {
            if (switchCamera.currentCameraState == CameraState.CloseUp)
            {
                return true; // Return true if any close-up camera is active
            }
        }
        return false; // No close-up camera is active
    }

    private void ResetInteraction()
    {
        isInteracting = false; // Allow interaction again
    }

    private void OnLeverIdle()
    {
        if (leverAnimator != null)
        {
            leverAnimator.SetTrigger("LeverIdle");
        }
    }

    private void BookButtonUp()
    {
        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger("BookButtonUp"); // Trigger button up animation
        }
    }
}