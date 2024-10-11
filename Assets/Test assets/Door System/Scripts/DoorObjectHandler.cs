using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DoorObjectHandler : MonoBehaviour
{
    [Tooltip("Check to lock a door")]
    public bool Locked = false;

    [Tooltip("Check this if the door can open.")]
    public bool CanOpen = true;

    [Tooltip("Check this if the door can close.")]
    public bool CanClose = true;

    [Tooltip("When true, the door hinge is free to move, and the player will be able to move through.")]
    public bool IsOpened = false;

    [Tooltip("The interaction radius for the door.")]
    public float interactRange = 3f;

    [Tooltip("Reference to the Locked Text.")]
    public Text lockedText;

    [Tooltip("Reference to the Unlocked Text.")]
    public Text unlockedText;

    private HingeJoint hinge;
    private Rigidbody rbDoor;
    private bool isPlayerNearby = false;
    private JointLimits hingeLimits;
    private float currentLimit;
    public float OpenSpeed = 3f;

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        rbDoor = GetComponent<Rigidbody>();

        if (hinge == null)
        {
            Debug.LogError("No HingeJoint component found on the door.");
        }

        // Initially disable the text components
        if (lockedText != null) lockedText.enabled = false;
        if (unlockedText != null) unlockedText.enabled = false;
    }

    void Update()
    {
        CheckPlayerProximity();
    }

    private void CheckPlayerProximity()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                isPlayerNearby = true;
                return;
            }
        }
        isPlayerNearby = false;
    }

    public void Interact()
    {
        Debug.Log("Interact method called."); // Debug log to check if the method is called

        // Check if player is nearby and interacting with the door
        if (!isPlayerNearby)
        {
            Debug.Log("Player is not nearby to interact.");
            return; // Early exit if player is not nearby
        }

        if (Locked)
        {
            Debug.Log("Door is locked.");
            if (lockedText != null && !IsOpened) // Only show locked text if the door is not opened
            {
                ShowText(lockedText); // Show locked text
                Debug.Log("Locked text displayed."); // Debug log
            }
            return; // Early exit if the door is locked
        }

        if (IsOpened)
        {
            CloseDoor();
        }
        else
        {
            OpenDoor();
        }
    }


    public void OpenDoor()
    {
        if (!Locked && CanOpen && !IsOpened)
        {
            IsOpened = true;
            rbDoor.AddRelativeTorque(new Vector3(0, 0, 20f)); // Apply torque to open the door
            Debug.Log("Door is now open.");
        }
    }

    public void CloseDoor()
    {
        if (!Locked && CanClose && IsOpened)
        {
            IsOpened = false;
            Debug.Log("Door is now closed.");
        }
    }

    // New method to unlock the door and display unlocked text
    public void UnlockDoor()
    {
        if (Locked) // Only proceed if the door is currently locked
        {
            Locked = false; // Set the door to unlocked
            Debug.Log("Door is now unlocked.");
            if (unlockedText != null)
            {
                ShowText(unlockedText); // Show unlocked text
                Debug.Log("Unlocked text displayed."); // Debug log
            }
        }
    }

    private void FixedUpdate()
    {
        if (IsOpened)
        {
            currentLimit = 85f;
        }
        else
        {
            if (currentLimit > 1f)
                currentLimit -= .5f * OpenSpeed;
        }

        hingeLimits.max = currentLimit;
        hingeLimits.min = -currentLimit;
        hinge.limits = hingeLimits;
    }

    public bool IsPlayerNearby()
    {
        return isPlayerNearby;
    }

    private void ShowText(Text textComponent)
    {
        textComponent.enabled = true; // Enable the text component
        StartCoroutine(FadeInText(textComponent, 1f)); // Fade in over 1 second
    }

    private IEnumerator FadeInText(Text textComponent, float duration)
    {
        Color textColor = textComponent.color;
        textColor.a = 0; // Start fully transparent
        textComponent.color = textColor;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Clamp01(elapsedTime / duration); // Gradually increase alpha
            textComponent.color = textColor;
            yield return null; // Wait for the next frame
        }

        StartCoroutine(FadeOutText(textComponent, 1f)); // Fade out after fading in
    }

    private IEnumerator FadeOutText(Text textComponent, float duration)
    {
        Color textColor = textComponent.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Clamp01(1 - (elapsedTime / duration)); // Gradually decrease alpha
            textComponent.color = textColor;
            yield return null; // Wait for the next frame
        }

        textComponent.enabled = false; // Disable the text component after fading out
        Debug.Log($"{textComponent.name} hidden."); // Debug log
    }

    private IEnumerator HideTextAfterDelay(Text textComponent, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        textComponent.enabled = false; // Disable the text component
        Debug.Log($"{textComponent.name} hidden."); // Debug log
    }
}