using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractableDrawerClosetChest : MonoBehaviour
{
    private bool isOpen = false;
    private bool isMoving = false;
    private Vector3 closedPosition;

    public bool isCloset = false;
    public bool isDrawer = false;
    public bool isChest = false;

    [Header("Audio Settings")]
    public AudioSource drawerOpenCloseAudio;
    public AudioSource cabinetOpenCloseAudio;
    public AudioSource chestOpenCloseAudio;
    public AudioSource drawerLockedAudioSource;
    public AudioSource chestLockedAudioSource;

    [Header("Drawer Settings")]
    public float openDistance = 0.5f;
    public float openCloseSpeedDrawer = 2f;
    public bool isDrawerLocked = false;
    public Text drawerLockedText;

    [Header("Event for Unlocking a drawer")]
    public UnityEvent onUnlockDrawer;

    [Header("Chest Settings")]
    public float closedXRotation = 0f;
    public float openXRotation = -90f;
    public float openCloseSpeedChest = 2f;
    public bool isChestLocked = false;
    public Text chestLockedText;

    [Header("Event for Unlocking a chest")]
    public UnityEvent onUnlockChest;

    [Header("Closet Settings")]
    public float closedYRotation = 0f;
    public float openYRotation = 90f;
    public float openCloseSpeedCloset = 2f;

    [Header("Key Settings")]
    public string requiredKeyId;
    public InventoryManager inventoryManager;
    public Button backButton; // Reference to the back button



    // New UI element to show when a key is required and the player is close up
    public GameObject unlockUI; // Reference to your unlock UI (e.g., a button to unlock)

    void Start()
    {
        // Set initial positions and rotations based on the type of object (closet, drawer, chest)
        if (isCloset)
            transform.localRotation = Quaternion.Euler(0f, closedYRotation, 0f);
        else if (isDrawer)
            closedPosition = transform.localPosition;
        else if (isChest)
            transform.localRotation = Quaternion.Euler(closedXRotation, 0f, 0f);

        InventoryManager.Instance.OnItemUsed += OnItemUsed;

        // Ensure that the back button is properly set up
        if (backButton != null)
        {
            // Attach the HideUnlockUI method to the button click event
            backButton.onClick.AddListener(HideUnlockUI);
        }

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            HandleInput(Input.mousePosition);
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            HandleInput(Input.GetTouch(0).position);

        if (isMoving)
            MoveInteractable();

    }

    void HandleInput(Vector2 inputPosition)
    {
        NoteInspectionManager noteManager = FindObjectOfType<NoteInspectionManager>();
        if (noteManager != null && noteManager.isNoteUIActive) return;

        ItemInspectionManager[] itemInspectionManagers = FindObjectsOfType<ItemInspectionManager>();
        foreach (var itemInspectionManager in itemInspectionManagers)
            if (itemInspectionManager.IsInspecting()) return;

        if (!IsCloseUpCameraActive()) return;

        Ray ray = Camera.main.ScreenPointToRay(inputPosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject && !isMoving)
            ToggleDrawerClosetOrChest();

    }

    public void ToggleDrawerClosetOrChest()
    {
        if (!isMoving)
        {
            if (isOpen)
                Close();
            else
            {
                if (isDrawer && isDrawerLocked)
                {
                    PlaySound(drawerLockedAudioSource);
                    StartCoroutine(FadeInText(drawerLockedText));
                    ShowUnlockUIIfNeeded(); // Call here
                    return;
                }
                if (isChest && isChestLocked)
                {
                    PlaySound(chestLockedAudioSource);
                    StartCoroutine(FadeInText(chestLockedText));
                    ShowUnlockUIIfNeeded(); // Call here
                    return;
                }
                Open();
            }
        }
    }

    private void ShowUnlockUIIfNeeded()
    {
        // Check if unlockUI is assigned, requiredKeyId is filled, and close-up camera is active for this instance
        if (unlockUI != null && !string.IsNullOrEmpty(requiredKeyId) && IsCloseUpCameraActive())
        {
            UnityEngine.Debug.Log("Showing Unlock UI");
            unlockUI.SetActive(true); // Show the UI if all conditions are met
        }
        else if (unlockUI != null)
        {
            UnityEngine.Debug.Log("Hiding Unlock UI");
            unlockUI.SetActive(false); // Hide the UI if conditions are not met
        }
    }

    public void HideUnlockUI()
    {
        // Check if the unlockUI is currently active
        if (unlockUI != null && unlockUI.activeSelf)
        {
            UnityEngine.Debug.Log("Back button pressed, hiding unlock UI.");
            unlockUI.SetActive(false); // Hide the UI if it's currently active
        }
        else
        {
            UnityEngine.Debug.Log("Unlock UI is not active, no action taken.");
        }
    }

    private IEnumerator FadeInText(Text text)
    {
        text.gameObject.SetActive(true);
        Color textColor = text.color;
        textColor.a = 0;
        text.color = textColor;

        while (textColor.a < 1)
        {
            textColor.a += Time.deltaTime;
            text.color = textColor;
            yield return null;
        }

        StartCoroutine(FadeOutText(text, 2f));
    }

    private IEnumerator FadeOutText(Text text, float duration)
    {
        Color textColor = text.color;
        while (textColor.a > 0)
        {
            textColor.a -= Time.deltaTime / duration;
            text.color = textColor;
            yield return null;
        }
        text.gameObject.SetActive(false);
    }

    private bool IsCloseUpCameraActive()
    {
        var switchCameras = FindObjectsOfType<SwitchCamera>();
        foreach (var switchCamera in switchCameras)
            if (switchCamera.currentCameraState == CameraState.CloseUp)
                return true;
        return false;
    }

    private void Open()
    {
        isOpen = true;
        isMoving = true;
        PlaySound(isDrawer ? drawerOpenCloseAudio : isChest ? chestOpenCloseAudio : cabinetOpenCloseAudio);

        if (isDrawer && !isDrawerLocked)
            onUnlockDrawer.Invoke();
        else if (isChest && !isChestLocked)
            onUnlockChest.Invoke();

        
    }

    private void Close()
    {
        isOpen = false;
        isMoving = true;
        PlaySound(isDrawer ? drawerOpenCloseAudio : isChest ? chestOpenCloseAudio : cabinetOpenCloseAudio);
    }

    private void PlaySound(AudioSource audioSource)
    {
        if (audioSource != null)
            audioSource.Play();
    }

    private void MoveInteractable()
    {
        if (isCloset)
            RotateInteractable(isOpen ? openYRotation : closedYRotation, openCloseSpeedCloset);
        else if (isDrawer)
            MoveInteractablePosition(openDistance, openCloseSpeedDrawer);
        else if (isChest)
            RotateInteractable(isOpen ? openXRotation : closedXRotation, openCloseSpeedChest);
    }

    private void MoveInteractablePosition(float distance, float speed)
    {
        Vector3 targetPosition = closedPosition + transform.localRotation * Vector3.forward * (isOpen ? distance : 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * speed);

        if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
        {
            transform.localPosition = targetPosition;
            isMoving = false;
        }
    }

    private void RotateInteractable(float targetAngle, float speed)
    {
        Quaternion targetRotation = Quaternion.Euler(isChest ? targetAngle : 0, isCloset ? targetAngle : 0, 0);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * speed);

        if (Quaternion.Angle(transform.localRotation, targetRotation) < 0.1f)
        {
            transform.localRotation = targetRotation;
            isMoving = false;
        }
    }

    public void OnItemUsed(ItemData item)
    {
        if (string.IsNullOrEmpty(requiredKeyId)) return; // Do nothing if no key is required

        if (item.keyId == requiredKeyId && IsCloseUpCameraActive())
        {
            if (isDrawer)
            {
                isDrawerLocked = false;
                PlaySound(drawerOpenCloseAudio);
                onUnlockDrawer.Invoke();
            }
            else if (isChest)
            {
                isChestLocked = false;
                PlaySound(chestOpenCloseAudio);
                onUnlockChest.Invoke();
            }

            StartCoroutine(DeleteItemAfterDelay(item, 1f));
        }
        else
        {
            UnityEngine.Debug.Log("Key is incorrect or player is too far.");
        }
    }

    private IEnumerator DeleteItemAfterDelay(ItemData item, float delay)
    {
        yield return new WaitForSeconds(delay);
        inventoryManager.DeleteItem(item);
    }
}
