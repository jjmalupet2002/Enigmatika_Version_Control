using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    public Button defaultButton; // Assign in the Inspector
    public Button talkButton;    // Assign in the Inspector
    public Button interactButton; // On-Screen Button for interaction

    private PlayerInput playerInput; // Reference to PlayerInput component
    private InputAction interactAction; // Action reference
    private bool isNearNPC = false; // Track NPC proximity
    private NPCInteractable currentNPC; // Reference to the current NPC

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>(); // Get PlayerInput component attached to the GameObject
        if (playerInput == null)
        {
            UnityEngine.Debug.LogError("PlayerInput component not found!!");
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

        StartCoroutine(CheckNPCProximity()); // Start coroutine for NPC proximity check

        // Add event listener for on-screen button
        if (interactButton != null)
        {
            interactButton.onClick.AddListener(OnInteractButtonPressed);
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.Disable();
            interactAction.performed -= OnInteractPerformed;
        }

        StopCoroutine(CheckNPCProximity()); // Stop coroutine when disabled

        // Remove event listener for on-screen button
        if (interactButton != null)
        {
            interactButton.onClick.RemoveListener(OnInteractButtonPressed);
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (isNearNPC)
        {
            UnityEngine.Debug.Log("Interact action performed");
            PerformInteraction();
        }
    }

    public void OnInteractButtonPressed()
    {
        if (isNearNPC)
        {
            UnityEngine.Debug.Log("Interact button pressed");
            PerformInteraction();
        }
    }

    private void PerformInteraction()
    {
        if (currentNPC != null)
        {
            UnityEngine.Debug.Log("Interacting with NPC");
            currentNPC.Interact();

            // Start making the NPC face the player
            LookAtPlayer npcLookAtPlayer = currentNPC.GetComponent<LookAtPlayer>();
            if (npcLookAtPlayer != null)
            {
                npcLookAtPlayer.StartFacingPlayer();
            }
        }
    }

    private IEnumerator CheckNPCProximity()
    {
        while (true)
        {
            float interactRange = 2f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);

            bool foundNPC = false;
            foreach (Collider collider in colliderArray)
            {
                if (collider.CompareTag("NPC"))
                {
                    foundNPC = true;
                    currentNPC = collider.GetComponent<NPCInteractable>(); // Cache the current NPC
                    break;
                }
            }

            if (foundNPC != isNearNPC)
            {
                isNearNPC = foundNPC;
                defaultButton.gameObject.SetActive(!isNearNPC);
                talkButton.gameObject.SetActive(isNearNPC);
            }

            yield return new WaitForSeconds(0.2f); // Check every 0.2 seconds, adjust as needed
        }
    }
}
