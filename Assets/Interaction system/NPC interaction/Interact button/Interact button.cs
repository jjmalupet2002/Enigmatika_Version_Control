using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TalkandInteract : MonoBehaviour
{
    public Button defaultButton; // Assign in the Inspector
    public Button talkButton;    // Assign in the Inspector
    public Button interactButton; // On-Screen Button for interaction

    private PlayerInput playerInput; // Reference to PlayerInput component
    private InputAction interactAction; // Action reference
    private bool isNearNPC = false; // Track NPC proximity
    private bool isNearInteractable = false; // Track interactable proximity
    private NPCInteractable currentNPC; // Reference to the current NPC
    private GameObject currentInteractable; // Reference to the current interactable object

    public float interactCooldown = 0.5f; // 0.5 seconds cooldown
    private float lastInteractTime = 0f;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>(); // Get PlayerInput component attached to the GameObject
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found!");
        }
    }

    private void OnEnable()
    {
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component is not assigned.");
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
            Debug.LogError("Interact action not found!");
        }

        // Always show the interact button
        if (interactButton != null)
        {
            interactButton.gameObject.SetActive(true);
            interactButton.onClick.AddListener(OnInteractButtonPressed);
        }

        // Start the coroutine only if it's not already running
        if (!IsInvoking("CheckProximity"))
        {
            StartCoroutine(CheckProximity());
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.Disable();
            interactAction.performed -= OnInteractPerformed;
        }

        StopCoroutine(CheckProximity()); // Stop coroutine when disabled

        // Remove event listener for on-screen button
        if (interactButton != null)
        {
            interactButton.onClick.RemoveListener(OnInteractButtonPressed);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        HandleInteraction();
    }

    public void OnInteractButtonPressed()
    {
        if (Time.time - lastInteractTime >= interactCooldown)
        {
            HandleInteraction();
            lastInteractTime = Time.time; // Update the last interaction time
        }
    
    }

    private void HandleInteraction()
    {
        if (isNearNPC)
        {
            Debug.Log("Interact button pressed with NPC");
            PerformNPCInteraction();
        }
        else if (isNearInteractable)
        {
            Debug.Log("Interact button pressed with Interactable Object");
            PerformObjectInteraction();
        }
    }

    private void PerformNPCInteraction()
    {
        if (currentNPC != null)
        {
            Debug.Log("Interacting with NPC");

            // Disable player movement while interacting
            GameStateManager.Instance.SetPlayerMovementState(false);

            currentNPC.Interact();

            // Start making the NPC face the player
            LookAtPlayer npcLookAtPlayer = currentNPC.GetComponent<LookAtPlayer>();
            if (npcLookAtPlayer != null)
            {
                npcLookAtPlayer.StartFacingPlayer();
            }

            // Re-enable player movement after interaction is done
            GameStateManager.Instance.SetPlayerMovementState(true);
        }
    }

    private void PerformObjectInteraction()
    {
        if (currentInteractable != null)
        {
            Debug.Log("Interacting with Object. Current Interactable: " + currentInteractable);
            SwitchCamera switchCam = currentInteractable.GetComponent<SwitchCamera>();
            if (switchCam != null)
            {
                Debug.Log("SwitchCamera component found.");
                switchCam.SwitchToNextCamera(); // Or whichever method is appropriate
            }
            else
            {
                Debug.LogError("SwitchCamera component not found on interactable object.");
            }
        }
    }

    private IEnumerator CheckProximity()
    {
        while (true)
        {
            float interactRange = 2f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

            bool foundNPC = false;
            bool foundInteractable = false;
            GameObject nearestInteractable = null;

            foreach (Collider collider in colliderArray)
            {
                if (collider.CompareTag("NPC"))
                {
                    foundNPC = true;
                    currentNPC = collider.GetComponent<NPCInteractable>(); // Cache the current NPC
                }
                else if (collider.CompareTag("Interactable"))
                {
                    foundInteractable = true;
                    nearestInteractable = collider.gameObject; // Cache the nearest interactable object
                }
            }

            if (foundNPC != isNearNPC)
            {
                isNearNPC = foundNPC;
                defaultButton.gameObject.SetActive(!isNearNPC);
                talkButton.gameObject.SetActive(isNearNPC);
            }

            // Update the state of interactButton only when needed
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
