using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class StatuePuzzle : MonoBehaviour
{
    [Header("Statue References")]
    public GameObject Knight1;
    public GameObject Knight2;

    [Header("Rotation Variables")]
    public float Knight1StartY; // Changed to Y axis
    public float Knight1EndY; // Changed to Y axis
    public float Knight2StartY; // Changed to Y axis
    public float Knight2EndY; // Changed to Y axis
    public float rotationSpeed = 30f;

    [Header("Audio References")]
    public AudioSource Knight1RotateAudio;
    public AudioSource Knight2RotateAudio;

    [Header("Thief Hideout References")]
    public GameObject ThiefHideout;
    public float ThiefStartY;
    public float ThiefEndY;
    public AudioSource ThiefHideoutRising;

    [Header("Input System")]
    public InputActionAsset inputActions;

    private InputAction rotateAction;
    private bool isRotatingKnight1 = false;
    private bool isRotatingKnight2 = false;
    private bool allStatueRotated = false;

    void Start()
    {
        if (inputActions == null)
        {
            UnityEngine.Debug.LogError("InputActionAsset is NOT assigned in Inspector!");
            return;
        }

        rotateAction = inputActions.FindAction("RotateItem");

        if (rotateAction == null)
        {
            UnityEngine.Debug.LogError("RotateItem action NOT found in InputActionAsset! Check the spelling.");
            return;
        }

        rotateAction.performed += ctx => RotateStatue();
    }

    void Update()
    {
        if (IsCloseUpCameraActive() && !allStatueRotated)
        {
            CheckRaycast();
        }

        // Always check the rotation status and manage audio here
        if (isRotatingKnight1)
        {
            RotateKnight(Knight1, Knight1RotateAudio, Knight1EndY);
        }
        else if (isRotatingKnight2)
        {
            RotateKnight(Knight2, Knight2RotateAudio, Knight2EndY);
        }
    }

    private void CheckRaycast()
    {
        Ray ray = Camera.main.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == Knight1)
            {
                isRotatingKnight1 = true;
                isRotatingKnight2 = false;
            }
            else if (hit.collider.gameObject == Knight2)
            {
                isRotatingKnight2 = true;
                isRotatingKnight1 = false;
            }
        }
    }

    private void RotateStatue()
    {
        // Check if statues are fully rotated, regardless of camera state
        if (Mathf.Abs(Mathf.DeltaAngle(Knight1.transform.rotation.eulerAngles.y, Knight1EndY)) < 1f &&
            Mathf.Abs(Mathf.DeltaAngle(Knight2.transform.rotation.eulerAngles.y, Knight2EndY)) < 1f)
        {
            if (!allStatueRotated)
            {
                allStatueRotated = true;
                rotateAction.Disable();
                // Stop audio for both statues before raising the thief hideout
                StopStatueAudio();
                StartCoroutine(RaiseThiefHideout());
            }
        }
    }

    private void RotateKnight(GameObject knight, AudioSource audioSource, float endRotation)
    {
        // Rotate knight only if close-up camera is active
        if (IsCloseUpCameraActive())
        {
            float currentYRotation = knight.transform.rotation.eulerAngles.y;
            float newYRotation = Mathf.MoveTowardsAngle(currentYRotation, endRotation, rotationSpeed * Time.deltaTime);
            knight.transform.rotation = Quaternion.Euler(knight.transform.rotation.eulerAngles.x, newYRotation, knight.transform.rotation.eulerAngles.z);

            // Check if the knight is still rotating in real-time
            bool isRotating = Mathf.Abs(Mathf.DeltaAngle(currentYRotation, newYRotation)) > 0.1f;

            // If statue is rotating, play the audio if not already playing
            if (isRotating)
            {
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                // Stop the audio immediately when rotation stops
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
    }

    // Method to explicitly stop audio for both statues
    private void StopStatueAudio()
    {
        if (Knight1RotateAudio != null && Knight1RotateAudio.isPlaying)
        {
            Knight1RotateAudio.Stop();
        }
        if (Knight2RotateAudio != null && Knight2RotateAudio.isPlaying)
        {
            Knight2RotateAudio.Stop();
        }
    }

    private IEnumerator RaiseThiefHideout()
    {
        if (ThiefHideoutRising != null)
        {
            ThiefHideoutRising.Play();
        }

        float elapsedTime = 0f;
        Vector3 startPosition = new Vector3(ThiefHideout.transform.position.x, ThiefStartY, ThiefHideout.transform.position.z);
        Vector3 endPosition = new Vector3(ThiefHideout.transform.position.x, ThiefEndY, ThiefHideout.transform.position.z);

        UnityEngine.Debug.Log("Rising Thief Hideout");

        while (elapsedTime < 4f)
        {
            ThiefHideout.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / 4f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        ThiefHideout.transform.position = endPosition;
        ThiefHideoutRising.Stop();
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
}
