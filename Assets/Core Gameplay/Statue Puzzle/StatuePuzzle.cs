using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using CarterGames.Assets.SaveManager;
using Save;


public class StatuePuzzle : MonoBehaviour
{
    [Header("Statue References")]
    public GameObject Knight1;
    public GameObject Knight2;

    [Header("Rotation Variables")]
    public float Knight1StartY;
    public float Knight1EndY;
    public float Knight2StartY;
    public float Knight2EndY;
    public float rotationSpeed = 30f;

    [Header("Audio References")]
    public AudioSource Knight1RotateAudio;
    public AudioSource Knight2RotateAudio;

    [Header("Thief Hideout References")]
    public GameObject ThiefHideout;
    public float ThiefStartY;
    public float ThiefEndY;
    public AudioSource ThiefHideoutRising;

    [Header("Input System and Save System")]
    public InputActionAsset inputActions;
    public PuzzlesStates1SaveObject statuePuzzleSaveObject;


    private InputAction rotateAction;
    private bool isRotatingKnight1 = false;
    private bool isRotatingKnight2 = false;
    private bool allStatueRotated = false;

    void Start()
    {
        if (inputActions == null)
        {
            return;
        }

        rotateAction = inputActions.FindAction("RotateItem");

        if (rotateAction == null)
        {
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

        if (Knight1 != null && isRotatingKnight1)
        {
            RotateKnight(Knight1, Knight1RotateAudio, Knight1EndY);
        }
        else if (Knight2 != null && isRotatingKnight2)
        {
            RotateKnight(Knight2, Knight2RotateAudio, Knight2EndY);
        }
    }

    // Save puzzle state
    private void SaveStatuePuzzleState()
    {
        statuePuzzleSaveObject.knight1Rotation.Value = Knight1.transform.rotation.eulerAngles;
        statuePuzzleSaveObject.knight2Rotation.Value = Knight2.transform.rotation.eulerAngles;
        statuePuzzleSaveObject.allStatueRotated.Value = allStatueRotated;
        statuePuzzleSaveObject.thiefHideoutPosition.Value = ThiefHideout.transform.position;
    }

    // Load puzzle state
    private void LoadStatuePuzzleState()
    {
        Knight1.transform.rotation = Quaternion.Euler(statuePuzzleSaveObject.knight1Rotation.Value);
        Knight2.transform.rotation = Quaternion.Euler(statuePuzzleSaveObject.knight2Rotation.Value);
        allStatueRotated = statuePuzzleSaveObject.allStatueRotated.Value;
        ThiefHideout.transform.position = statuePuzzleSaveObject.thiefHideoutPosition.Value;
    }

    private void OnEnable()
    {
        SaveEvents.OnSaveGame += SaveStatuePuzzleState;
        SaveEvents.OnLoadGame += LoadStatuePuzzleState;
    }

    private void OnDisable()
    {
        SaveEvents.OnSaveGame -= SaveStatuePuzzleState;
        SaveEvents.OnLoadGame -= LoadStatuePuzzleState;
    }

    private void CheckRaycast()
    {
        if (Camera.main == null)
        {
            UnityEngine.Debug.LogWarning("Main Camera is not found!");
            return;
        }

        if (Touchscreen.current == null)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Touchscreen.current.primaryTouch.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject == Knight1)
            {
                isRotatingKnight1 = true;
                isRotatingKnight2 = false;
            }
            else if (hit.collider != null && hit.collider.gameObject == Knight2)
            {
                isRotatingKnight2 = true;
                isRotatingKnight1 = false;
            }
        }
    }

    private void RotateStatue()
    {
        if (Knight1 == null || Knight2 == null) return;

        if (Mathf.Abs(Mathf.DeltaAngle(Knight1.transform.rotation.eulerAngles.y, Knight1EndY)) < 1f &&
            Mathf.Abs(Mathf.DeltaAngle(Knight2.transform.rotation.eulerAngles.y, Knight2EndY)) < 1f)
        {
            if (!allStatueRotated)
            {
                allStatueRotated = true;
                rotateAction.Disable();
                StopStatueAudio();
                StartCoroutine(RaiseThiefHideout());

                // Auto-save when puzzle is completed
                SaveEvents.SaveGame();
            }
        }
    }

    private void RotateKnight(GameObject knight, AudioSource audioSource, float endRotation)
    {
        if (knight == null) return;

        if (IsCloseUpCameraActive())
        {
            float currentYRotation = knight.transform.rotation.eulerAngles.y;
            float newYRotation = Mathf.MoveTowardsAngle(currentYRotation, endRotation, rotationSpeed * Time.deltaTime);
            knight.transform.rotation = Quaternion.Euler(knight.transform.rotation.eulerAngles.x, newYRotation, knight.transform.rotation.eulerAngles.z);

            bool isRotating = Mathf.Abs(Mathf.DeltaAngle(currentYRotation, newYRotation)) > 0.1f;

            if (isRotating)
            {
                if (audioSource != null && !audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
    }

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
        if (ThiefHideout == null)
        {
            UnityEngine.Debug.LogError("ThiefHideout is not assigned!");
            yield break;
        }

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
        if (ThiefHideoutRising != null)
        {
            ThiefHideoutRising.Stop();
        }
    }

    private bool IsCloseUpCameraActive()
    {
        var switchCameras = FindObjectsOfType<SwitchCamera>();

        foreach (var switchCamera in switchCameras)
        {
            if (switchCamera != null && switchCamera.currentCameraState == CameraState.CloseUp)
            {
                return true;
            }
        }
        return false;
    }
}
