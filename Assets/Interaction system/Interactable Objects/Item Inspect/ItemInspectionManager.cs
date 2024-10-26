using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Diagnostics;

public class ItemInspectionManager : MonoBehaviour
{
    public List<GameObject> itemsToInspect; // List of items to inspect
    private Transform inspectionPoint;
    private bool isInspecting = false;
    private float rotationSpeed = 1000f;
    private float rotateSmoothness = 0.1f; // Adjust the smoothness of rotation

    private Vector3 targetRotation;

    // Store original positions, rotations, and shadow casting modes for each item
    private List<Vector3> originalPositions = new List<Vector3>();
    private List<Quaternion> originalRotations = new List<Quaternion>();
    private List<ShadowCastingMode> originalShadowCastingModes = new List<ShadowCastingMode>();

    // Store current positions and rotations for manipulation
    private List<Vector3> currentPositions = new List<Vector3>();
    private List<Quaternion> currentRotations = new List<Quaternion>();

    private int currentItemIndex = -1; // To track the current item being inspected

    // Reference to SwitchCamera script
    public SwitchCamera switchCamera;

    // Reference to the Input Action Asset
    public InputActionAsset inputActions;
    private InputAction rotateItemAction;

    public CloseUpViewUIController closeUpViewUIController;

    [SerializeField] private BackButtonHandler backButtonHandler;

    private NoteInspectionManager noteInspectionManager;

    // List of AudioSources for each item
    private List<AudioSource> itemAudioSources = new List<AudioSource>();

    void Start()
    {
        // Initialize the input action for rotation
        rotateItemAction = inputActions.FindAction("RotateItem");
        rotateItemAction.Enable();

        inspectionPoint = transform;
        targetRotation = Vector3.zero;

        // Find the NoteInspectionManager instance in the scene
        noteInspectionManager = FindObjectOfType<NoteInspectionManager>();

        // Store the original position, rotation, and shadow casting mode for each item, and set up audio sources
        foreach (GameObject item in itemsToInspect)
        {
            if (item != null)
            {
                originalPositions.Add(item.transform.position);
                originalRotations.Add(item.transform.rotation);

                currentPositions.Add(item.transform.position);
                currentRotations.Add(item.transform.rotation);

                MeshRenderer meshRenderer = item.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    originalShadowCastingModes.Add(meshRenderer.shadowCastingMode);
                }
                else
                {
                    originalShadowCastingModes.Add(ShadowCastingMode.Off);
                }

                // Get the AudioSource from each item and add to the list
                AudioSource audioSource = item.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    itemAudioSources.Add(audioSource);
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"No AudioSource found on {item.name}, make sure each item has an AudioSource component.");
                    itemAudioSources.Add(null); // Add null if no AudioSource is found
                }
            }
        }
    }

    public bool IsInspecting()
    {
        return isInspecting;
    }

    public void StopInspection()
    {
        if (closeUpViewUIController != null)
        {
            closeUpViewUIController.SetUIActive(false);
        }

        if (itemsToInspect.Count > 0 && isInspecting && currentItemIndex >= 0)
        {
            GameObject itemToInspect = itemsToInspect[currentItemIndex];
            itemToInspect.transform.SetParent(null);
            itemToInspect.transform.position = originalPositions[currentItemIndex];
            itemToInspect.transform.rotation = originalRotations[currentItemIndex];

            // Stop playing the specific audio for the inspected item
            if (itemAudioSources[currentItemIndex] != null)
            {
                itemAudioSources[currentItemIndex].Stop();
            }

            isInspecting = false;

            MeshRenderer meshRenderer = itemToInspect.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.shadowCastingMode = originalShadowCastingModes[currentItemIndex];
            }

            EnableAllItemColliders();
        }
    }

    void InspectItem()
    {
        if (closeUpViewUIController != null)
        {
            closeUpViewUIController.SetUIActive(true);
        }

        if (itemsToInspect.Count > 0 && !isInspecting && currentItemIndex >= 0)
        {
            GameObject itemToInspect = itemsToInspect[currentItemIndex];
            itemToInspect.transform.position = inspectionPoint.position;
            itemToInspect.transform.rotation = originalRotations[currentItemIndex];
            itemToInspect.transform.SetParent(inspectionPoint);

            isInspecting = true;
            targetRotation = originalRotations[currentItemIndex].eulerAngles;

            currentPositions[currentItemIndex] = originalPositions[currentItemIndex];
            currentRotations[currentItemIndex] = originalRotations[currentItemIndex];

            DisableOtherItemColliders(itemToInspect);

            MeshRenderer meshRenderer = itemToInspect.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            }

            // Play the specific audio for the inspected item
            if (itemAudioSources[currentItemIndex] != null)
            {
                itemAudioSources[currentItemIndex].Play();
            }
        }
    }

    void Update()
    {
        if (switchCamera != null && switchCamera.currentCameraState == CameraState.CloseUp && noteInspectionManager != null && !noteInspectionManager.isNoteUIActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                for (int i = 0; i < itemsToInspect.Count; i++)
                {
                    if (Physics.Raycast(ray, out hit) && hit.transform.gameObject == itemsToInspect[i])
                    {
                        currentItemIndex = i;
                        InspectItem();
                        break;
                    }
                }
            }
        }

        if (isInspecting)
        {
            if (noteInspectionManager != null)
            {
                noteInspectionManager.enabled = false;
            }

            if (Input.GetMouseButton(0))
            {
                float rotateX = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
                float rotateY = -Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

                targetRotation += new Vector3(rotateX, rotateY, 0f);

                Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
                itemsToInspect[currentItemIndex].transform.rotation = Quaternion.Slerp(itemsToInspect[currentItemIndex].transform.rotation, targetQuaternion, rotateSmoothness);
            }

            var touchscreen = Touchscreen.current;
            if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
            {
                var touch = touchscreen.primaryTouch;
                if (touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
                {
                    float rotateX = -touch.delta.ReadValue().y * rotationSpeed * 0.02f * Time.deltaTime;
                    float rotateY = -touch.delta.ReadValue().x * rotationSpeed * 0.02f * Time.deltaTime;

                    targetRotation += new Vector3(rotateX, rotateY, 0f);

                    Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
                    itemsToInspect[currentItemIndex].transform.rotation = Quaternion.Slerp(itemsToInspect[currentItemIndex].transform.rotation, targetQuaternion, rotateSmoothness);
                }
            }
        }
        else
        {
            if (noteInspectionManager != null)
            {
                noteInspectionManager.enabled = true;
            }
        }
    }

    private void DisableOtherItemColliders(GameObject inspectedItem)
    {
        foreach (GameObject item in itemsToInspect)
        {
            if (item != inspectedItem)
            {
                Collider collider = item.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.enabled = false;
                }
            }
        }
    }

    private void EnableAllItemColliders()
    {
        foreach (GameObject item in itemsToInspect)
        {
            Collider collider = item.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
        }
    }
}
