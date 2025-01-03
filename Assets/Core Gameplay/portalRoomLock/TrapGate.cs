using System.Collections;
using UnityEngine;
using UnityEngine.UI; // For Unity's UI Text component

public class TrapGate : MonoBehaviour
{
    [Header("TrapGate Settings")]
    public float interactRange = 5f; // Adjust the radius as needed
    public GameObject gateObject;
    public float gateStartingY;
    public float gateTargetY;
    public float gateMovementDelay = 1f;
    public AudioSource trapGateSound;
    public AudioSource unlockGateSound;

    // Reference to the texts that will be shown
    public Text displayText; // Drag your "Portal Room is locked" Text component here
    public Text unlockedText; // Drag your "Portal room has been unlocked" Text component here

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private bool isPlayerNearby = false;
    private bool isGateUnlocked = false; // Flag to track if the gate has been unlocked

    // Enum to track the gate's state
    private enum GateState { Closed, Open }
    private GateState currentGateState = GateState.Closed;

    // Public boolean to unlock the gate
    public bool unlockGate = false;

    void Start()
    {
        // Set the initial and target positions for the gate
        initialPosition = new Vector3(gateObject.transform.position.x, gateStartingY, gateObject.transform.position.z);
        targetPosition = new Vector3(gateObject.transform.position.x, gateTargetY, gateObject.transform.position.z);

        // Set the gate to the starting position
        gateObject.transform.position = initialPosition;

        // Make sure the texts start hidden (disabled)
        if (displayText != null)
        {
            displayText.gameObject.SetActive(false);
        }

        if (unlockedText != null)
        {
            unlockedText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Continuously check for player proximity
        CheckPlayerProximity();

        // If unlockGate is true and the gate hasn't been unlocked yet, reset the gate to its starting position
        if (unlockGate && !isGateUnlocked)
        {
            UnlockGate();
            unlockGate = false;  // Reset the unlockGate flag to prevent multiple unlocks until checked again
        }

        // If player is nearby and the gate is not already open and not unlocked, start moving the gate
        if (isPlayerNearby && currentGateState == GateState.Closed && !isGateUnlocked)
        {
            StartCoroutine(MoveGate());
            isPlayerNearby = false;  // Prevent multiple triggers
        }
    }

    private void CheckPlayerProximity()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                isPlayerNearby = true;
                return; // Exit the loop once the player is detected
            }
        }
        isPlayerNearby = false;
    }

    private IEnumerator MoveGate()
    {
        // Wait for the specified delay before moving the gate
        yield return new WaitForSeconds(gateMovementDelay);

        // Play the sound if available
        if (trapGateSound != null && !trapGateSound.isPlaying)
        {
            trapGateSound.Play();
        }

        // Move the gate to the target position using Lerp for smooth movement
        float elapsedTime = 0f;
        float duration = 1f; // Duration of the gate movement

        while (elapsedTime < duration)
        {
            gateObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the gate reaches the exact target position
        gateObject.transform.position = targetPosition;

        // Set the state to 'Open' as the gate is now fully moved
        currentGateState = GateState.Open;

        // Show the text
        if (displayText != null)
        {
            displayText.gameObject.SetActive(true); // Enable the text GameObject
            StartCoroutine(HideTextAfterDelay(displayText, 2f)); // Display the text for 2 seconds
        }
    }

    // Method to unlock the gate and move it back to the starting position
    public void UnlockGate()
    {
        if (isGateUnlocked) return;  // Prevent unlocking again if already unlocked

        isGateUnlocked = true; // Mark the gate as unlocked

        // Play the unlock sound if available
        if (unlockGateSound != null)
        {
            unlockGateSound.Play();
        }

        // Show the unlocked text
        if (unlockedText != null)
        {
            unlockedText.gameObject.SetActive(true); // Enable the text GameObject
            StartCoroutine(HideTextAfterDelay(unlockedText, 2f)); // Display the text for 2 seconds
        }

        // Use Lerp to smoothly move the gate back to its starting position
        StartCoroutine(MoveGateBackToStart());
    }

    private IEnumerator MoveGateBackToStart()
    {
        // Play sound if available (optional)
        if (trapGateSound != null && !trapGateSound.isPlaying)
        {
            trapGateSound.Play();
        }

        // Move the gate back to its starting position using Lerp for smooth movement
        float elapsedTime = 0f;
        float duration = 4f; // Duration of the gate movement back to start

        while (elapsedTime < duration)
        {
            gateObject.transform.position = Vector3.Lerp(gateObject.transform.position, initialPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the gate reaches the exact starting position
        gateObject.transform.position = initialPosition;

        // Set the state back to 'Closed'
        currentGateState = GateState.Closed;
    }

    private IEnumerator HideTextAfterDelay(Text textComponent, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        textComponent.gameObject.SetActive(false); // Disable the text GameObject
    }
}
