using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using CarterGames.Assets.SaveManager; // Import SaveManager for saving/loading
using Save;

public class GateUnlockScript : MonoBehaviour
{
    // Lock Object References
    [Header("Lock Object References")]
    public GameObject[] lockObjects; // Array of lock objects

    // Key Unlock Animation References (animator controllers)
    [Header("Key Unlock Animation References")]
    public GameObject[] keyObjects; // Array of key objects
    private Animator[] keyAnimators; // Animators for the key objects
    public string unlockTrigger = "Unlock"; // Trigger to activate unlock animation

    // Required Key IDs
    [Header("Required Key IDs")]
    public string[] requiredKeyIds; // Array of required key IDs for each lock

    // Lock Unlock Sounds (can be null)
    [Header("Lock Unlock Sounds")]
    public AudioSource unlockAudioSource; // Audio to play when unlocking the lock

    // Inventory Manager asset
    public InventoryManager inventoryManager;

    // TrapGate script reference
    [Header("TrapGate Script Reference")]
    public TrapGate trapGateScript;

    // Unlock Button References
    [Header("Unlock Button References")]
    public Button[] unlockButtons; // Array of unlock buttons

    // Camera Reference
    [Header("Camera Reference")]
    public Camera portalRoomCamera; // Reference to the camera component (PortalRoomCamera)
    public GameObject EscapeText;
    [SerializeField] private PortalRoomTrapLockSaveObject saveObject; // Reference to the save object
  
    // Variables for item usage and button click
    private bool[] isUnlockButtonClicked; // Array to track button click states
    private bool[] hasUsedKey; // Array to track if the correct key has been used
    private ItemData[] currentItems; // Array to store the current items

    // Track each lock object's unlock state
    private bool[] lockStates;
    private bool[] keyAnimationStates;

    private void OnEnable()
    {
        InventoryManager.Instance.OnItemUsed += OnItemUsed;
        SaveEvents.OnSaveGame += SaveLockState;
        SaveEvents.OnLoadGame += LoadLockState;

        // Initialize arrays
        isUnlockButtonClicked = new bool[unlockButtons.Length];
        hasUsedKey = new bool[unlockButtons.Length];
        currentItems = new ItemData[unlockButtons.Length];
        lockStates = new bool[unlockButtons.Length];
        keyAnimators = new Animator[keyObjects.Length];

        // Subscribe to the button click events
        for (int i = 0; i < unlockButtons.Length; i++)
        {
            int index = i; // Local copy of loop variable
            if (unlockButtons[i] != null)
            {
                unlockButtons[i].onClick.AddListener(() => OnUnlockButtonClick(index));
            }

            if (keyObjects[i] != null)
            {
                keyAnimators[i] = keyObjects[i].GetComponent<Animator>();
            }
        }
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnItemUsed -= OnItemUsed;
        SaveEvents.OnSaveGame -= SaveLockState;
        SaveEvents.OnLoadGame -= LoadLockState;

        // Unsubscribe from the button click events
        for (int i = 0; i < unlockButtons.Length; i++)
        {
            if (unlockButtons[i] != null)
            {
                unlockButtons[i].onClick.RemoveListener(() => OnUnlockButtonClick(i));
            }
        }
    }

    private void SaveLockState()
    {
        saveObject.lockStates.Value = lockStates;
        saveObject.keyAnimationStates.Value = keyAnimationStates;
    }

    private void LoadLockState()
    {
        lockStates = saveObject.lockStates.Value ?? new bool[unlockButtons.Length];
        keyAnimationStates = saveObject.keyAnimationStates.Value ?? new bool[keyObjects.Length];

        for (int i = 0; i < lockStates.Length; i++)
        {
            if (lockStates[i])
            {
                // Ensure buttons are disabled for already unlocked locks
                DisableUnlockButton(i);
            }

            if (keyAnimationStates[i] && keyAnimators[i] != null)
            {
                keyAnimators[i].SetTrigger(unlockTrigger);
            }
        }
    }

    // When the item is used, update the hasUsedKey flags accordingly
    private void OnItemUsed(ItemData item)
    {
        // Reset the used key flags to avoid showing multiple buttons
        for (int i = 0; i < hasUsedKey.Length; i++)
        {
            hasUsedKey[i] = false;  // Clear all flags when a new key is equipped
        }

        // Check each required key ID and update the corresponding flag
        for (int i = 0; i < requiredKeyIds.Length; i++)
        {
            if (string.IsNullOrEmpty(requiredKeyIds[i]) || item.keyId != requiredKeyIds[i])
            {
                continue; // Skip if the key doesn't match or is not required
            }

            // Mark that the key has been used and store the item
            hasUsedKey[i] = true;
            currentItems[i] = item;
        }
    }

    private void OnUnlockButtonClick(int index)
    {
        UnityEngine.Debug.Log($"Unlock Button {index} Clicked!");
        isUnlockButtonClicked[index] = true;
        TryUnlock(index);
    }

    // Method to check both conditions and perform unlock action for a specific lock
    public void TryUnlock(int index)
    {
        if (hasUsedKey[index] && isUnlockButtonClicked[index])
        {
            UnityEngine.Debug.Log($"Attempting to unlock lock {index}...");

            // Trigger unlock animation on the key object
            if (keyAnimators[index] != null)
            {
                keyAnimators[index].SetTrigger(unlockTrigger);
                keyAnimationStates[index] = true; // Mark animation as played
                UnityEngine.Debug.Log($"Unlock animation triggered on key {index}!");
            }

            // Play unlock sound
            if (unlockAudioSource != null)
            {
                unlockAudioSource.Play();
            }

            // Set the lock state to unlocked
            lockStates[index] = true;

            // **Auto-save the game after updating lock state**
            SaveEvents.SaveGame();

            // Remove used key from inventory
            if (currentItems[index] != null)
            {
                StartCoroutine(DeleteItemAfterDelay(currentItems[index], 1.5f));
            }

            // Disable the unlock button
            DisableUnlockButton(index);

            // Check if all locks are unlocked
            CheckAllLocks();

            // Reset flags after unlocking
            isUnlockButtonClicked[index] = false;
            hasUsedKey[index] = false;
        }
    }

    private void CheckAllLocks()
    {
        foreach (bool state in lockStates)
        {
            if (!state)
            {
                return; // Exit if any lock is still locked
            }
        }

        // Debug to check if it's reaching this point
        UnityEngine.Debug.Log("All locks are unlocked, attempting to unlock the gate!");

        // All locks are unlocked, unlock the gate
        trapGateScript.UnlockGate();
        EscapeText.SetActive(true);

        // **Auto-save the game after unlocking all locks**
        SaveEvents.SaveGame();
    }

    // Coroutine to delete item from inventory after a delay
    private IEnumerator DeleteItemAfterDelay(ItemData item, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait before deleting the item
        inventoryManager.DeleteItem(item);
    }

    private void DisableUnlockButton(int index)
    {
        if (unlockButtons[index] != null)
        {
            UnityEngine.Debug.Log($"Button {index} disabled.");
            unlockButtons[index].gameObject.SetActive(false); // Disable the button
            unlockButtons[index].interactable = false; // Disable button interaction
        }
    }


    private void Update()
    {
        // Check if the PortalRoomCamera is active
        bool isPortalRoomCameraActive = portalRoomCamera != null && portalRoomCamera.GetComponent<Camera>().enabled;

        // Loop through each unlock button and update its active state based on key usage and camera activation
        for (int i = 0; i < unlockButtons.Length; i++)
        {
            // Reset button visibility each frame to handle key switching properly
            if (hasUsedKey[i] && isPortalRoomCameraActive)
            {
                unlockButtons[i].gameObject.SetActive(true);  // Activate the unlock button
                unlockButtons[i].interactable = true;         // Enable interaction
            }
            else
            {
                unlockButtons[i].gameObject.SetActive(false); // Deactivate the unlock button
                unlockButtons[i].interactable = false;        // Disable interaction
            }
        }

        // Toggle key objects' mesh renderers based purely on the PortalRoomCamera active state
        foreach (GameObject keyObject in keyObjects)
        {
            if (keyObject != null)
            {
                MeshRenderer meshRenderer = keyObject.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    // The key object is visible only when the PortalRoomCamera is active
                    meshRenderer.enabled = isPortalRoomCameraActive;
                }
            }
        }
    }
}