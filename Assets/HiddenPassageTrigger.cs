using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class HiddenPassageTrigger : MonoBehaviour
{
    [Header("Lever Settings")]
    public Animator leverAnimator;
    public AudioSource leverAudioSource;
    public bool isLever = true;

    [Header("Passage Settings")]
    public GameObject passageObject;
    public float startX;
    public float targetX;
    public float moveSpeed = 2f;

    [Header("Input Settings")]
    public InputActionAsset inputActionAsset;

    [Header("Camera Settings")]
    public Camera targetCamera; // Public reference to the camera component

    private bool isMoving = false;

    void Start()
    {
        if (leverAnimator == null)
        {
            leverAnimator = GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        inputActionAsset.FindAction("SwipeUp").Enable();
    }

    private void OnDisable()
    {
        inputActionAsset.FindAction("SwipeUp").Disable();
    }

    void Update()
    {
        if (isLever)
        {
            HandleLeverInteraction();
        }
    }

    private void OnLeverIdle()
    {
        if (leverAnimator != null)
        {
            leverAnimator.SetTrigger("LeverIdle");
        }
    }

    private void HandleLeverInteraction()
    {
        if (IsCloseUpCameraActive())
        {
            Vector2 swipeInput = inputActionAsset.FindAction("SwipeUp").ReadValue<Vector2>();

            if (swipeInput.y > 0.5f)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
                {
                    TriggerLever();
                }
            }
        }
    }

    private bool IsCloseUpCameraActive()
    {
        var switchCameras = FindObjectsOfType<SwitchCamera>();
        foreach (var switchCamera in switchCameras)
        {
            if (switchCamera.currentCameraState == CameraState.CloseUp)
            {
                return true;
            }
        }
        return false;
    }

    private void TriggerLever()
    {
        if (leverAudioSource != null)
        {
            leverAudioSource.Play();
        }

        leverAnimator.SetTrigger("LeverUp");
        Invoke("ResetLever", 0.2f);

        // Start the combined coroutine for both camera activation and passage movement
        StartCoroutine(HandleLeverAndPassageMovement());
    }

    private void ResetLever()
    {
        if (leverAnimator.GetCurrentAnimatorStateInfo(0).IsName("LeverIdle"))
            return; // Prevents re-triggering the idle animation if it's already playing

        leverAnimator.SetTrigger("LeverIdle");
    }

    private IEnumerator HandleLeverAndPassageMovement()
    {
        // Wait for 2 seconds before activating the camera and moving the passage
        yield return new WaitForSeconds(2f);

        // Activate the camera for 2.5 seconds
        if (targetCamera != null)
        {
            StartCoroutine(ActivateCameraForDuration(2.5f));
        }

        // Start moving the passage after the camera delay
        StartCoroutine(MovePassage());
    }

    private IEnumerator MovePassage()
    {
        if (passageObject == null || isMoving)
            yield break;

        isMoving = true;
        Vector3 startPosition = passageObject.transform.position;
        Vector3 targetPosition = new Vector3(targetX, startPosition.y, startPosition.z);

        float elapsedTime = 0;
        while (elapsedTime < moveSpeed)
        {
            passageObject.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        passageObject.transform.position = targetPosition;
        isMoving = false;
    }

    // Coroutine to enable the camera for a specified duration
    private IEnumerator ActivateCameraForDuration(float duration)
    {
        targetCamera.enabled = true;  // Enable the camera
        yield return new WaitForSeconds(duration);  // Wait for the duration
        targetCamera.enabled = false;  // Disable the camera
    }
}
