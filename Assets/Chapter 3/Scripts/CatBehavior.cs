using System;
using System.Collections;
using UnityEngine;

public class CatBehavior : MonoBehaviour
{
    [Header("References")]
    public SwitchCamera switchCamera;
    public GameObject catObject;
    public Transform cameraTransform;
    public AudioSource catPurrAudioSource;
    public AudioSource catMeowAudioSource;

    [Header("Settings")]
    public float lookDelay = 1.0f; // Delay before the cat looks at the camera
    public Vector2 meowIntervalRange = new Vector2(15f, 30f); // Random interval range for meows
    public LayerMask catLayerMask; // Set this layer mask to the cat layer in Inspector

    private bool isFacingCamera = false;
    private bool isHandlingMeow = false;
    private bool canTouchMeow = true;

    private void Update()
    {
        if (IsCloseUpCameraActive())
        {
            if (!isFacingCamera)
            {
                StartCoroutine(FaceCameraAfterDelay());
            }

            UpdateAudioSettings();

            if (!isHandlingMeow)
            {
                StartCoroutine(HandleMeowSound());
            }

            HandleTouchInput();
        }
        else
        {
            ResetAudioSettings();
            isFacingCamera = false;
            isHandlingMeow = false;
            StopAllCoroutines();
        }
    }

    private bool IsCloseUpCameraActive()
    {
        if (switchCamera != null)
        {
            return switchCamera.currentCameraState == CameraState.CloseUp;
        }
        return false;
    }

    private IEnumerator FaceCameraAfterDelay()
    {
        isFacingCamera = true;
        yield return new WaitForSeconds(lookDelay);

        if (catObject != null && cameraTransform != null)
        {
            Vector3 direction = cameraTransform.position - catObject.transform.position;
            direction.y = 0; // Optional: prevent tilting up/down
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            float rotationSpeed = 2f;

            while (Quaternion.Angle(catObject.transform.rotation, targetRotation) > 0.1f)
            {
                catObject.transform.rotation = Quaternion.Lerp(
                    catObject.transform.rotation,
                    targetRotation,
                    Time.deltaTime * rotationSpeed
                );
                yield return null;
            }

            catObject.transform.rotation = targetRotation;
        }
    }

    private void UpdateAudioSettings()
    {
        if (catPurrAudioSource != null)
        {
            catPurrAudioSource.spatialBlend = 0.3f;
            catPurrAudioSource.volume = 0.8f;
            if (!catPurrAudioSource.isPlaying)
                catPurrAudioSource.Play();
        }

        if (catMeowAudioSource != null && catMeowAudioSource.isPlaying == false)
        {
            // No need to auto play meow here anymore
        }
    }

    private void ResetAudioSettings()
    {
        if (catPurrAudioSource != null)
        {
            catPurrAudioSource.spatialBlend = 0.8f;
            catPurrAudioSource.volume = 0.1f;
            if (!catPurrAudioSource.isPlaying)
                catPurrAudioSource.Play();
        }

        if (catMeowAudioSource != null)
        {
            catMeowAudioSource.Stop();
        }
    }

    private IEnumerator HandleMeowSound()
    {
        isHandlingMeow = true;

        while (IsCloseUpCameraActive())
        {
            float randomDelay = UnityEngine.Random.Range(meowIntervalRange.x, meowIntervalRange.y);
            yield return new WaitForSeconds(randomDelay);

            if (catMeowAudioSource != null && catPurrAudioSource != null && canTouchMeow)
            {
                catPurrAudioSource.mute = true;
                catMeowAudioSource.PlayOneShot(catMeowAudioSource.clip);
                yield return new WaitForSeconds(catMeowAudioSource.clip.length);
                catPurrAudioSource.mute = false;
            }
        }

        isHandlingMeow = false;
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, catLayerMask))
            {
                if (hit.collider.gameObject == catObject && canTouchMeow)
                {
                    StartCoroutine(PlayTouchMeow());
                }
            }
        }
    }

    private IEnumerator PlayTouchMeow()
    {
        canTouchMeow = false;

        if (catPurrAudioSource != null)
            catPurrAudioSource.mute = true;

        if (catMeowAudioSource != null)
            catMeowAudioSource.PlayOneShot(catMeowAudioSource.clip);

        yield return new WaitForSeconds(catMeowAudioSource.clip.length);

        if (catPurrAudioSource != null)
            catPurrAudioSource.mute = false;

        yield return new WaitForSeconds(1.0f); // Cooldown after meow to prevent spam
        canTouchMeow = true;
    }
}
