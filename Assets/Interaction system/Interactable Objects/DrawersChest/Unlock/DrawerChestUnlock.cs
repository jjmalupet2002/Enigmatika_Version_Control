using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;

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
    private NoteInspectionManager noteInspectionManager;

    [Tooltip("Audio source for button sound.")]
    public AudioSource buttonSound;

    [Tooltip("Audio source for lever sound.")]
    public AudioSource leverSound;

    private void Start()
    {
        // Enable the swipe input action
        inputActionAsset.FindAction("SwipeUp").Enable();

        // Start the BookButton in idle animation
        if (IsButton && buttonAnimator != null)
        {
            buttonAnimator.SetTrigger("BookIdle"); // Start idle animation
        }

        // Get reference to NoteInspectionManager
        noteInspectionManager = NoteInspectionManager.Instance;

        // Log the assignment of noteInspectionManager
        if (noteInspectionManager != null)
        {
           
        }
        else
        {
            UnityEngine.Debug.LogError("NoteInspectionManager is not assigned. Please check if NoteInspectionManager.Instance is properly initialized.");
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
        if (noteInspectionManager == null)
        {
            UnityEngine.Debug.LogError("noteInspectionManager is null in HandleButtonInteraction.");
            return;
        }

        if (noteInspectionManager.isNoteUIActive)
        {
          
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            if (IsCloseUpCameraActive())
            {
                if (buttonSound != null)
                {
                    buttonSound.Play(); // Play button sound
                }
                UnlockIfApplicable();
            }
            else
            {
                UnityEngine.Debug.Log("Cannot interact; close-up camera is not active.");
            }
        }
    }

    private void HandleLeverInteraction()
    {
        if (noteInspectionManager == null)
        {
            UnityEngine.Debug.LogError("noteInspectionManager is null in HandleLeverInteraction.");
            return;
        }

        if (noteInspectionManager.isNoteUIActive)
        {
            return;
        }

        if (IsCloseUpCameraActive())
        {
            Vector2 swipeInput = inputActionAsset.FindAction("SwipeUp").ReadValue<Vector2>();

            if (swipeInput.y > 0.5f) // Threshold for swiping up
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
                {
                    if (leverSound != null)
                    {
                        leverSound.Play(); // Play lever sound
                    }
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
                UnityEngine.Debug.Log("The selected object is already unlocked.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("No InteractableDrawerChest assigned.");
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
                UnityEngine.Debug.LogWarning("buttonAnimator is not assigned. Cannot play button press animation.");
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
                UnityEngine.Debug.LogWarning("leverAnimator is not assigned. Cannot play lever up animation.");
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
                UnityEngine.Debug.LogWarning("buttonAnimator is not assigned. Cannot play button press animation.");
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
                UnityEngine.Debug.LogWarning("leverAnimator is not assigned. Cannot play lever up animation.");
            }
        }
    }

    private void ShowUnlockUI(bool isDrawer)
    {
        if (isDrawer)
        {
            unlockedDrawerUI.SetActive(true);
            Text drawerText = unlockedDrawerUI.GetComponent<Text>();
            if (drawerText != null)
            {
                StartCoroutine(FadeInText(drawerText, 1f)); // Fade in the text over 1 second
            }
            Invoke("HideUnlockUI", 1.5f); // Adjust the delay as necessary
        }
        else
        {
            unlockedChestUI.SetActive(true);
            Text chestText = unlockedChestUI.GetComponent<Text>();
            if (chestText != null)
            {
                StartCoroutine(FadeInText(chestText, 1f)); // Fade in the text over 1 second
            }
            Invoke("HideUnlockUI", 1.5f); // Adjust the delay as necessary
        }
    }

    private void HideUnlockUI()
    {
        Text drawerText = unlockedDrawerUI.GetComponent<Text>();
        if (drawerText != null)
        {
            StartCoroutine(HideTextAfterDelay(drawerText, 0.5f)); // Delay before fading out
        }

        Text chestText = unlockedChestUI.GetComponent<Text>();
        if (chestText != null)
        {
            StartCoroutine(HideTextAfterDelay(chestText, 0.5f)); // Delay before fading out
        }
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
        isInteracting = false; // Reset interaction state
    }

    private IEnumerator FadeInText(Text text, float duration)
    {
        Color originalColor = text.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f); // Fully opaque

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float normalizedTime = t / duration;
            text.color = Color.Lerp(originalColor, targetColor, normalizedTime);
            yield return null;
        }

        text.color = targetColor; // Ensure the target color is fully set
    }

    private IEnumerator HideTextAfterDelay(Text text, float delay)
    {
        yield return new WaitForSeconds(delay);

        Color originalColor = text.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f); // Fully transparent

        for (float t = 0; t < 0.5f; t += Time.deltaTime)
        {
            float normalizedTime = t / 0.5f; // Fade out over 0.5 seconds
            text.color = Color.Lerp(originalColor, targetColor, normalizedTime);
            yield return null;
        }

        text.color = targetColor; // Ensure the target color is fully set
        unlockedDrawerUI.SetActive(false);
        unlockedChestUI.SetActive(false);
    }

    private void BookButtonUp()
    {
        if (buttonAnimator != null)
        {
            buttonAnimator.SetTrigger("BookButtonUp");
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
