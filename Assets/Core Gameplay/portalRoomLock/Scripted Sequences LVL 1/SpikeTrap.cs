using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using CarterGames.Assets.SaveManager;
using Save;

public class SpikeTrap : MonoBehaviour
{
    [Header("SpikeTrap Settings")]
    public GameObject spikeObject;
    public float spikeStartingY;
    public float spikeTargetY;
    public float spikeMovementDelay = 1f;
    public AudioSource trapSpikeSound;
    public AudioSource unlockSpikeSound;
    public PuzzlesStates1SaveObject spikeTrapSaveObject;

    // Reference to the Gold King Trigger object
    public Transform goldKingTrigger; // Assign in Inspector
    private Vector3 lastKnownPosition;

    // Public boolean to unlock the spike
    public bool unlockSpike = false;

    // Reference to displayed texts (if needed)
    public Text displayText;
    public Text unlockedText;

    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private bool isSpikeUnlocked = false;

    // Enum to track the spike's state
    private enum SpikeState { Closed, Open }
    private SpikeState currentSpikeState = SpikeState.Closed;

    void Start()
    {
        // Set spike positions
        initialPosition = new Vector3(spikeObject.transform.position.x, spikeStartingY, spikeObject.transform.position.z);
        targetPosition = new Vector3(spikeObject.transform.position.x, spikeTargetY, spikeObject.transform.position.z);
        spikeObject.transform.position = initialPosition;

        // Store initial position of Gold King Trigger
        if (goldKingTrigger != null)
        {
            lastKnownPosition = goldKingTrigger.position;
        }

        // Ensure texts start hidden
        if (displayText != null) displayText.gameObject.SetActive(false);
        if (unlockedText != null) unlockedText.gameObject.SetActive(false);
    }

    void Update()
    {
        // Check if the Gold King Trigger object has moved
        if (goldKingTrigger != null && goldKingTrigger.position != lastKnownPosition)
        {
            TriggerSpikeTrap();
            lastKnownPosition = goldKingTrigger.position; // Update position after detection
        }

        // Unlock spike if requested
        if (unlockSpike && !isSpikeUnlocked)
        {
            UnlockSpike();
            unlockSpike = false; // Prevent repeated unlocks
        }
    }
    private void OnEnable()
    {
        SaveEvents.OnSaveGame += SaveSpikeState;
        SaveEvents.OnLoadGame += LoadSpikeState;
    }

    private void OnDisable()
    {
        SaveEvents.OnSaveGame -= SaveSpikeState;
        SaveEvents.OnLoadGame -= LoadSpikeState;
    }

    private void TriggerSpikeTrap()
    {
        if (currentSpikeState == SpikeState.Closed && !isSpikeUnlocked)
        {
            StartCoroutine(TriggerWithDelay());
        }
    }

    private IEnumerator ActivateSpike()
    {
        yield return new WaitForSeconds(spikeMovementDelay);

        if (trapSpikeSound != null && !trapSpikeSound.isPlaying)
        {
            trapSpikeSound.Play();
        }

        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            spikeObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spikeObject.transform.position = targetPosition;
        currentSpikeState = SpikeState.Open;

        if (displayText != null)
        {
            displayText.gameObject.SetActive(true);
            StartCoroutine(HideTextAfterDelay(displayText, 2f));
        }
    }

    public void UnlockSpike()
    {
        if (isSpikeUnlocked) return;

        isSpikeUnlocked = true;

        if (unlockSpikeSound != null)
        {
            unlockSpikeSound.Play();
        }

        if (unlockedText != null)
        {
            unlockedText.gameObject.SetActive(true);
            StartCoroutine(HideTextAfterDelay(unlockedText, 2f));
        }

        StartCoroutine(DeactivateSpike());
        SaveEvents.SaveGame();
    }

    private void SaveSpikeState()
    {
        spikeTrapSaveObject.spikePosition.Value = spikeObject.transform.position;
        spikeTrapSaveObject.isSpikeUnlocked.Value = isSpikeUnlocked;
        spikeTrapSaveObject.currentSpikeState.Value = (int)currentSpikeState;
    }
    private void LoadSpikeState()
    {
        if (spikeTrapSaveObject == null)
        {
            UnityEngine.Debug.LogError("[LoadSpikeState] ERROR: spikeTrapSaveObject is null!");
            return;
        }

        if (spikeTrapSaveObject.spikePosition == null)
        {
            UnityEngine.Debug.LogWarning("[LoadSpikeState] WARNING: No saved spike position found!");
            return;
        }

        spikeObject.transform.position = spikeTrapSaveObject.spikePosition.Value;
        isSpikeUnlocked = spikeTrapSaveObject.isSpikeUnlocked.Value;
        currentSpikeState = (SpikeState)spikeTrapSaveObject.currentSpikeState.Value;
    }

    private IEnumerator DeactivateSpike()
    {
        if (trapSpikeSound != null && !trapSpikeSound.isPlaying)
        {
            trapSpikeSound.Play();
        }

        float elapsedTime = 0f;
        float duration = 4f;

        while (elapsedTime < duration)
        {
            spikeObject.transform.position = Vector3.Lerp(spikeObject.transform.position, initialPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spikeObject.transform.position = initialPosition;
        currentSpikeState = SpikeState.Closed;
    }

    private IEnumerator TriggerWithDelay()
    {
        // Wait for 2 seconds before triggering the spike
        yield return new WaitForSeconds(2f);

        // Now trigger the spike
        StartCoroutine(ActivateSpike());
    }

    private IEnumerator HideTextAfterDelay(Text textComponent, float delay)
    {
        yield return new WaitForSeconds(delay);
        textComponent.gameObject.SetActive(false);
    }
}
