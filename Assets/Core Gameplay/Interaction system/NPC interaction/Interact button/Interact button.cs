using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TalkandInteract : MonoBehaviour
{
    public Button defaultButton; // Assign in the Inspector
    public Button talkButton;    // Assign in the Inspector
    public Button interactButton; // On-Screen Button for interaction

    [SerializeField] private float npcInteractRange = 2f;       // Radius for NPC interaction
    [SerializeField] private float objectInteractRange = 3f;    // Radius for Object interaction

    private PlayerInput playerInput; // Reference to PlayerInput component
    private InputAction interactAction; // Action reference
    private bool isNearNPC = false; // Track NPC proximity
    private bool isNearInteractable = false; // Track interactable proximity
    private NPCInteractable currentNPC; // Reference to the current NPC
    private GameObject currentInteractable; // Reference to the current interactable object

    public bool interactionProcessed = false;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>(); // Get PlayerInput component attached to the GameObject
        if (playerInput == null)
        {
            UnityEngine.Debug.LogError("PlayerInput component not found!");
        }
    }

    private void OnEnable()
    {
        if (playerInput == null)
        {
            UnityEngine.Debug.LogError("PlayerInput component is not assigned.");
            return;
        }

        interactAction = playerInput.actions.FindAction("Interact", true); // Find the action by name
        if (interactAction != null)
        {
            interactAction.Enable();
            interactAction.performed += OnInteractPerformed;
        }
        else
        {
            UnityEngine.Debug.LogError("Interact action not found!");
        }

        if (interactButton != null)
        {
            interactButton.gameObject.SetActive(true);
            interactButton.onClick.AddListener(OnInteractButtonPressed);
        }

        StartCoroutine(CheckProximity()); // Start coroutine for proximity check
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.Disable();
            interactAction.performed -= OnInteractPerformed;
        }

        StopCoroutine(CheckProximity()); // Stop coroutine when disabled

        if (interactButton != null)
        {
            interactButton.onClick.RemoveListener(OnInteractButtonPressed);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        OnInteractButtonPressed();
    }

    public void OnInteractButtonPressed()
    {
        if (!interactionProcessed) // Check if interaction has already been processed
        {
            if (isNearNPC)
            {
                PerformNPCInteraction();
            }
            else if (isNearInteractable)
            {
                PerformObjectInteraction();
            }
        }
    }

    private void PerformNPCInteraction()
    {
        if (currentNPC != null)
        {

            GameStateManager.Instance.SetPlayerMovementState(false);

            currentNPC.Interact();

            LookAtPlayer npcLookAtPlayer = currentNPC.GetComponent<LookAtPlayer>();
            if (npcLookAtPlayer != null)
            {
                npcLookAtPlayer.StartFacingPlayer();
            }

            GameStateManager.Instance.SetPlayerMovementState(true);
        }
    }

    private void PerformObjectInteraction()
    {
        if (currentInteractable != null)
        {
          
            SwitchCamera switchCam = currentInteractable.GetComponent<SwitchCamera>();
            if (switchCam != null)
            {
         
                Camera closeUpCam = currentInteractable.GetComponentInChildren<Camera>();
                switchCam.ManageCamera(closeUpCam);
            }
            else
            {
                UnityEngine.Debug.LogError("SwitchCamera component not found on interactable object.");
            }

            interactionProcessed = true; // Set the flag to true after processing the object interaction
        }
    }

    private IEnumerator CheckProximity()
    {
        while (true)
        {
            // Check for NPCs within their specified range
            Collider[] npcColliders = Physics.OverlapSphere(transform.position, npcInteractRange);
            bool foundNPC = false;
            foreach (Collider collider in npcColliders)
            {
                if (collider.CompareTag("NPC"))
                {
                    foundNPC = true;
                    currentNPC = collider.GetComponent<NPCInteractable>(); // Cache the current NPC
                    break; // Exit the loop once we find a valid NPC
                }
            }

            if (foundNPC != isNearNPC)
            {
                isNearNPC = foundNPC;
                defaultButton.gameObject.SetActive(!isNearNPC);
                talkButton.gameObject.SetActive(isNearNPC);
            }

            // Check for interactable objects within their specified range
            Collider[] objectColliders = Physics.OverlapSphere(transform.position, objectInteractRange);
            bool foundInteractable = false;
            GameObject nearestInteractable = null;

            foreach (Collider collider in objectColliders)
            {
                if (collider.CompareTag("Interactable"))
                {
                    foundInteractable = true;
                    nearestInteractable = collider.gameObject; // Cache the nearest interactable object
                    break; // Exit the loop once we find a valid interactable
                }
            }

            if (foundInteractable != isNearInteractable)
            {
                isNearInteractable = foundInteractable;

                if (isNearInteractable)
                {
                    currentInteractable = nearestInteractable; // Update the current interactable object
                }
                else
                {
                    currentInteractable = null; // Clear reference if no longer near an interactable object
                }
            }

            yield return new WaitForSeconds(0.2f); // Check every 0.2 seconds, adjust as needed
        }
    }
}