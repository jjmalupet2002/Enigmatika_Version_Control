using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class InteractableDrawerClosetChest : MonoBehaviour
{
    private bool isOpen = false;

    // Booleans to check the type of interactable object
    public bool isCloset = false;
    public bool isDrawer = false;
    public bool isChest = false;

    [Header("Audio Settings")]
    public AudioSource drawerOpenCloseAudio; // Separate AudioSource for Drawer
    public AudioSource cabinetOpenCloseAudio; // Separate AudioSource for Cabinet
    public AudioSource chestOpenCloseAudio; // Separate AudioSource for Chest
    public AudioSource drawerLockedAudioSource;
    public AudioSource chestLockedAudioSource;

    // Drawer settings
    [Header("Drawer Settings")]
    public float openDistance = 0.5f;
    public float openCloseSpeedDrawer = 2f;
    public bool isDrawerLocked = false;
    public Text drawerLockedText;

    [Header("Event for Unlocking a drawer")]
    public UnityEvent onUnlockDrawer;

    // Chest settings
    [Header("Chest Settings")]
    public float closedXRotation = 0f;
    public float openXRotation = -90f;
    public float openCloseSpeedChest = 2f;
    public bool isChestLocked = false;
    public Text chestLockedText;

    [Header("Event for Unlocking a chest")]
    public UnityEvent onUnlockChest;

    // Closet settings
    [Header("Closet Settings")]
    public float closedYRotation = 0f;
    public float openYRotation = 90f;
    public float openCloseSpeedCloset = 2f;

    private bool isMoving = false;
    private Vector3 closedPosition;

    void Start()
    {
        if (isCloset)
            transform.localRotation = Quaternion.Euler(0f, closedYRotation, 0f);
        else if (isDrawer)
            closedPosition = transform.localPosition;
        else if (isChest)
            transform.localRotation = Quaternion.Euler(closedXRotation, 0f, 0f);
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
                    return;
                }
                if (isChest && isChestLocked)
                {
                    PlaySound(chestLockedAudioSource);
                    StartCoroutine(FadeInText(chestLockedText));
                    return;
                }
                Open();
            }
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
}
