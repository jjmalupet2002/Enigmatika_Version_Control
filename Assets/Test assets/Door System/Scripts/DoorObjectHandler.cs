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

    [Tooltip("Audio source for opening sound.")]
    public AudioSource openSound;

    [Tooltip("Audio source for closing sound.")]
    public AudioSource closeSound;

    [Tooltip("Audio source for locked sound.")]
    public AudioSource lockedSound;

    [Tooltip("Audio source for Opening using key sound.")]
    public AudioSource OpeningUsingKey;

    private HingeJoint hinge;
    private Rigidbody rbDoor;
    private bool isPlayerNearby = false;
    private JointLimits hingeLimits;
    private float currentLimit;
    public float OpenSpeed = 3f;

    [Tooltip("Add if the required unlock is a key.")]
    // The key ID required to unlock this door
    public InventoryManager inventoryManager;
    public string requiredKeyId = "";

    void Start()
    {
        hinge = GetComponent<HingeJoint>();
        rbDoor = GetComponent<Rigidbody>();

        if (hinge == null)
        {
            UnityEngine.Debug.LogError("No HingeJoint component found on the door.");
        }

        // Initially disable the text components
        if (lockedText != null) lockedText.enabled = false;
        if (unlockedText != null) unlockedText.enabled = false;

        // Subscribe to the OnItemUsed event
        InventoryManager.Instance.OnItemUsed += OnItemUsed;
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

        if (!isPlayerNearby)
        {
            UnityEngine.Debug.Log("Player is not nearby to interact.");
            return;
        }

        if (Locked)
        {
            UnityEngine.Debug.Log("Door is locked.");
            lockedSound?.Play();
            if (lockedText != null && !IsOpened)
            {
                ShowText(lockedText);
            }
            return;
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
            rbDoor.AddRelativeTorque(new Vector3(0, 0, 20f));
            openSound?.Play();
            UnityEngine.Debug.Log("Door is now open.");
        }
    }

    public void CloseDoor()
    {
        if (!Locked && CanClose && IsOpened)
        {
            IsOpened = false;
            closeSound?.Play();
            UnityEngine.Debug.Log("Door is now closed.");
        }
    }

    // Method to unlock the door
    public void UnlockDoor()
    {
        if (Locked)
        {
            Locked = false;
            UnityEngine.Debug.Log("Door is now unlocked.");
            if (unlockedText != null)
            {
                ShowText(unlockedText);
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
        textComponent.enabled = true;
        StartCoroutine(FadeInText(textComponent, 1f));
    }

    private IEnumerator FadeInText(Text textComponent, float duration)
    {
        Color textColor = textComponent.color;
        textColor.a = 0;
        textComponent.color = textColor;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Clamp01(elapsedTime / duration);
            textComponent.color = textColor;
            yield return null;
        }

        StartCoroutine(FadeOutText(textComponent, 1f));
    }

    private IEnumerator FadeOutText(Text textComponent, float duration)
    {
        Color textColor = textComponent.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            textColor.a = Mathf.Clamp01(1 - (elapsedTime / duration));
            textComponent.color = textColor;
            yield return null;
        }

        textComponent.enabled = false;
    }

    private IEnumerator HideTextAfterDelay(Text textComponent, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        textComponent.enabled = false; // Disable the text component
        UnityEngine.Debug.Log($"{textComponent.name} hidden."); // Debug log
    }

    private void PlaySound(AudioSource audioSource)
    {
        if (audioSource != null)
            audioSource.Play();
    }

    private void OnItemUsed(ItemData item)
    {
        // Check if the item used is a key and matches the required key ID
        if (!string.IsNullOrEmpty(item.keyId) && item.keyId == requiredKeyId)
        {
            // Call the Interact method to check if the player is nearby before unlocking the door
            if (isPlayerNearby)
            {
                PlaySound(OpeningUsingKey);
                UnlockDoor(); // Unlock the door if the player is nearby and the key matches
              


                // Start a coroutine to delete the item after a delay
                StartCoroutine(DeleteItemAfterDelay(item, 1.5f)); // 1 second delay
            }
            else
            {
                // Optionally, you could show a message saying the player is too far from the door
                UnityEngine.Debug.Log("Player is too far to unlock the door.");
            }
        }
    }

    // Coroutine to handle the delay before deleting the item
    private IEnumerator DeleteItemAfterDelay(ItemData item, float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Delete the item from the inventory
        inventoryManager.DeleteItem(item); // Delete item after delay
    }
}


