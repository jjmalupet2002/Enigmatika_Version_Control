using System.Collections;
using UnityEngine;
using UnityEngine.UI; // For Unity's UI Text component

public class SpikeTrap : MonoBehaviour
{
    [Header("SpikeTrap Settings")]
    public float interactRange = 5f; // Adjust the radius as needed
    public GameObject spikeObject;
    public float spikeStartingY;
    public float spikeTargetY;
    public float spikeMovementDelay = 1f;
    public AudioSource trapSpikeSound;
    public AudioSource unlockSpikeSound;

    // Reference to the texts that will be shown
    public Text displayText; // Drag your "Portal Room is locked" Text component here
    public Text unlockedText; // Drag your "Portal room has been unlocked" Text component here

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private bool isPlayerNearby = false;
    private bool isSpikeUnlocked = false; // Flag to track if the spike has been unlocked

    // Enum to track the spike's state
    private enum SpikeState { Closed, Open }
    private SpikeState currentSpikeState = SpikeState.Closed;

    // Public boolean to unlock the spike
    public bool unlockSpike = false;

    // Public boolean to manually trigger the spike trap (for testing)
    public bool triggerSpikeTrap = false; // Trigger for testing

    void Start()
    {
        // Set the initial and target positions for the spike
        initialPosition = new Vector3(spikeObject.transform.position.x, spikeStartingY, spikeObject.transform.position.z);
        targetPosition = new Vector3(spikeObject.transform.position.x, spikeTargetY, spikeObject.transform.position.z);

        // Set the spike to the starting position
        spikeObject.transform.position = initialPosition;

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

        // If unlockSpike is true and the spike hasn't been unlocked yet, reset the spike to its starting position
        if (unlockSpike && !isSpikeUnlocked)
        {
            UnlockSpike();
            unlockSpike = false;  // Reset the unlockSpike flag to prevent multiple unlocks until checked again
        }

        // If player is nearby and the spike is not already open and not unlocked, start moving the spike
        if (isPlayerNearby && currentSpikeState == SpikeState.Closed && !isSpikeUnlocked)
        {
            StartCoroutine(ActivateSpike());
            isPlayerNearby = false;  // Prevent multiple triggers
        }

        // Check if triggerSpikeTrap is enabled for quick testing
        if (triggerSpikeTrap && currentSpikeState == SpikeState.Closed && !isSpikeUnlocked)
        {
            StartCoroutine(ActivateSpike());
            triggerSpikeTrap = false; // Reset triggerSpikeTrap to false after activating
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

    private IEnumerator ActivateSpike()
    {
        // Wait for the specified delay before moving the spike
        yield return new WaitForSeconds(spikeMovementDelay);

        // Play the sound if available
        if (trapSpikeSound != null && !trapSpikeSound.isPlaying)
        {
            trapSpikeSound.Play();
        }

        // Move the spike to the target position using Lerp for smooth movement
        float elapsedTime = 0f;
        float duration = 0.2f; // Duration of the spike movement

        while (elapsedTime < duration)
        {
            spikeObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the spike reaches the exact target position
        spikeObject.transform.position = targetPosition;

        // Set the state to 'Open' as the spike is now fully moved
        currentSpikeState = SpikeState.Open;

        // Show the text
        if (displayText != null)
        {
            displayText.gameObject.SetActive(true); // Enable the text GameObject
            StartCoroutine(HideTextAfterDelay(displayText, 2f)); // Display the text for 2 seconds
        }
    }

    // Method to unlock the spike and move it back to the starting position
    public void UnlockSpike()
    {
        if (isSpikeUnlocked) return;  // Prevent unlocking again if already unlocked

        isSpikeUnlocked = true; // Mark the spike as unlocked

        // Play the unlock sound if available
        if (unlockSpikeSound != null)
        {
            unlockSpikeSound.Play();
        }

        // Show the unlocked text
        if (unlockedText != null)
        {
            unlockedText.gameObject.SetActive(true); // Enable the text GameObject
            StartCoroutine(HideTextAfterDelay(unlockedText, 2f)); // Display the text for 2 seconds
        }

        // Use Lerp to smoothly move the spike back to its starting position
        StartCoroutine(DeactivateSpike());
    }

    private IEnumerator DeactivateSpike()
    {
        // Play sound if available (optional)
        if (trapSpikeSound != null && !trapSpikeSound.isPlaying)
        {
            trapSpikeSound.Play();
        }

        // Move the spike back to its starting position using Lerp for smooth movement
        float elapsedTime = 0f;
        float duration = 4f; // Duration of the spike movement back to start

        while (elapsedTime < duration)
        {
            spikeObject.transform.position = Vector3.Lerp(spikeObject.transform.position, initialPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the spike reaches the exact starting position
        spikeObject.transform.position = initialPosition;

        // Set the state back to 'Closed'
        currentSpikeState = SpikeState.Closed;
    }

    private IEnumerator HideTextAfterDelay(Text textComponent, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        textComponent.gameObject.SetActive(false); // Disable the text GameObject
    }

    // Draw Gizmos to visualize the interact range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
