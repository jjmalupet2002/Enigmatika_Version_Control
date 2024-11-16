using UnityEngine;
using System.Collections;

public class GateUnlockScript : MonoBehaviour
{
    [Header("Lock references")]

    public GameObject lockObject1;
    public GameObject lockObject2;
    public GameObject lockObject3;
    public GameObject lockObject4;

    [Header("Key unlock animation references")]

    public Animator unlockingAnim1;
    public Animator unlockingAnim2;
    public Animator unlockingAnim3;
    public Animator unlockingAnim4;

    [Header("Required Key ID")]

    public string lock1KeyID;
    public string lock2KeyID;
    public string lock3KeyID;
    public string lock4KeyID;

    [Header("Locks unlock sounds (can be null)")]

    public AudioSource lock1UnlockAudio;
    public AudioSource lock2UnlockAudio;
    public AudioSource lock3UnlockAudio;
    public AudioSource lock4UnlockAudio;

    [Header("Inventory Manager asset")]
    public InventoryManager inventoryManager;

    [Header("TrapGate script reference")]
    public TrapGate trapGateScript;

    public float detectionRadius = 2.0f; // Radius to detect the player

    private bool lock1Unlocked = false;
    private bool lock2Unlocked = false;
    private bool lock3Unlocked = false;
    private bool lock4Unlocked = false;

    private bool isPlayerNearby = false;

    private void OnEnable()
    {
        // Subscribe to the inventory item's event
        InventoryManager.Instance.OnItemUsed += OnItemUsed;
    }

    private void OnDisable()
    {
        // Unsubscribe when this object is disabled
        InventoryManager.Instance.OnItemUsed -= OnItemUsed;
    }

    private void Update()
    {
        CheckIfPlayerIsNearby();
    }

    private void OnItemUsed(ItemData item)
    {
        // Check if the item used is a key and matches the required key ID
        if (!string.IsNullOrEmpty(item.keyId))
        {
            if (item.keyId == lock1KeyID && !lock1Unlocked && isPlayerNearby && IsTouchingLock(lockObject1))
            {
                UnlockLock(1, lockObject1, unlockingAnim1, lock1UnlockAudio);
                lock1Unlocked = true;
            }
            else if (item.keyId == lock2KeyID && !lock2Unlocked && isPlayerNearby && IsTouchingLock(lockObject2))
            {
                UnlockLock(2, lockObject2, unlockingAnim2, lock2UnlockAudio);
                lock2Unlocked = true;
            }
            else if (item.keyId == lock3KeyID && !lock3Unlocked && isPlayerNearby && IsTouchingLock(lockObject3))
            {
                UnlockLock(3, lockObject3, unlockingAnim3, lock3UnlockAudio);
                lock3Unlocked = true;
            }
            else if (item.keyId == lock4KeyID && !lock4Unlocked && isPlayerNearby && IsTouchingLock(lockObject4))
            {
                UnlockLock(4, lockObject4, unlockingAnim4, lock4UnlockAudio);
                lock4Unlocked = true;
            }

            // Check if all locks are unlocked
            if (lock1Unlocked && lock2Unlocked && lock3Unlocked && lock4Unlocked)
            {
                trapGateScript.UnlockGate(); // Unlock the trap gate
            }
        }
    }

    private void UnlockLock(int lockIndex, GameObject lockObject, Animator unlockingAnim, AudioSource unlockSound)
    {
        Debug.Log($"Unlocking lock {lockIndex}");

        // Play the unlock animation
        unlockingAnim.SetTrigger("Unlock");

        // Play the unlock sound if available
        if (unlockSound != null)
        {
            unlockSound.Play();
        }

        // Disable the collider of the lock object after unlocking
        Collider lockCollider = lockObject.GetComponent<Collider>();
        if (lockCollider != null)
        {
            lockCollider.enabled = false;
        }
    }

    private bool IsTouchingLock(GameObject lockObject)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == lockObject)
            {
                Debug.Log($"Lock object {lockObject.name} is being touched by the mouse.");
                return true;
            }
        }

        UnityEngine.Debug.Log("Raycast did not hit any object.");
        return false;
    }

    private void CheckIfPlayerIsNearby()
    {
        // Check for the player in the detection radius around each lock object
        isPlayerNearby = IsPlayerNearLock(lockObject1) || IsPlayerNearLock(lockObject2) ||
                         IsPlayerNearLock(lockObject3) || IsPlayerNearLock(lockObject4);
    }

    private bool IsPlayerNearLock(GameObject lockObject)
    {
        Collider[] hitColliders = Physics.OverlapSphere(lockObject.transform.position, detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
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
}
