using System.Collections;
using UnityEngine;
using UnityEngine.UI; // For Unity's UI Text component
using CarterGames.Assets.SaveManager; // Include SaveManager namespace
using Save;
using System.Diagnostics;

public class TrapGate : MonoBehaviour
{
    [Header("TrapGate Settings")]
    public float interactRange = 5f;
    public GameObject gateObject;
    public float gateStartingY;
    public float gateTargetY;
    public float gateMovementDelay = 1f;
    public AudioSource trapGateSound;
    public AudioSource unlockGateSound;

    public Text displayText;
    public Text unlockedText;

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    private bool isPlayerNearby = false;
    private bool isGateUnlocked = false;

    private enum GateState { Open, Closed }
    private GateState currentGateState = GateState.Open;

    public bool unlockGate = false;

    public PortalRoomTrapLockSaveObject portalRoomTrapLockSaveObject;

    void Start()
    {
        initialPosition = new Vector3(gateObject.transform.position.x, gateStartingY, gateObject.transform.position.z);
        targetPosition = new Vector3(gateObject.transform.position.x, gateTargetY, gateObject.transform.position.z);
        gateObject.transform.position = initialPosition;

        if (displayText != null) displayText.gameObject.SetActive(false);
        if (unlockedText != null) unlockedText.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        SaveEvents.OnSaveGame += SaveGateState;
        SaveEvents.OnLoadGame += LoadGateState;
    }

    void OnDisable()
    {
        SaveEvents.OnSaveGame -= SaveGateState;
        SaveEvents.OnLoadGame -= LoadGateState;
    }

    void Update()
    {
        CheckPlayerProximity();

        if (unlockGate && !isGateUnlocked)
        {
            UnlockGate();
            unlockGate = false;
        }

        if (isPlayerNearby && currentGateState == GateState.Open && !isGateUnlocked)
        {
            StartCoroutine(MoveGate());
            isPlayerNearby = false;
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
                return;
            }
        }
        isPlayerNearby = false;
    }

    private IEnumerator MoveGate()
    {
        yield return new WaitForSeconds(gateMovementDelay);

        if (trapGateSound != null && !trapGateSound.isPlaying)
            trapGateSound.Play();

        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            gateObject.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gateObject.transform.position = targetPosition;
        currentGateState = GateState.Closed;

        if (displayText != null)
        {
            displayText.gameObject.SetActive(true);
            StartCoroutine(HideTextAfterDelay(displayText, 2f));
        }
    }

    public void UnlockGate()
    {
        if (isGateUnlocked) return;

        isGateUnlocked = true;

        if (unlockGateSound != null)
            unlockGateSound.Play();

        if (unlockedText != null)
        {
            unlockedText.gameObject.SetActive(true);
            StartCoroutine(HideTextAfterDelay(unlockedText, 2f));
        }

        StartCoroutine(MoveGateBackToStart());
        SaveGateState(); // Save the gate state when unlocked
        SaveEvents.SaveGame();
    }

    private IEnumerator MoveGateBackToStart()
    {
        if (trapGateSound != null && !trapGateSound.isPlaying)
            trapGateSound.Play();

        float elapsedTime = 0f;
        float duration = 4f;

        while (elapsedTime < duration)
        {
            gateObject.transform.position = Vector3.Lerp(gateObject.transform.position, initialPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gateObject.transform.position = initialPosition;
        currentGateState = GateState.Open;
    }

    private IEnumerator HideTextAfterDelay(Text textComponent, float delay)
    {
        yield return new WaitForSeconds(delay);
        textComponent.gameObject.SetActive(false);
    }

    private void SaveGateState()
    {
        // Save the gate's position and state
        portalRoomTrapLockSaveObject.gatePosition.Value = gateObject.transform.position;
        portalRoomTrapLockSaveObject.isGateUnlocked.Value = isGateUnlocked; // Save the unlocked state
        portalRoomTrapLockSaveObject.currentGateState.Value = (int)currentGateState; // Save the current gate state (Closed = 0, Open = 1)
    }

    private void LoadGateState()
    {
        isGateUnlocked = portalRoomTrapLockSaveObject.isGateUnlocked.Value;
        currentGateState = (GateState)portalRoomTrapLockSaveObject.currentGateState.Value;

        // Ensure correct position is loaded from saved data
        gateObject.transform.position = portalRoomTrapLockSaveObject.gatePosition.Value;
    }
}