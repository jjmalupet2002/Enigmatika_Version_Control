using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform player; // Assign the player transform in the Inspector
    private Quaternion originalRotation; // Store the original rotation
    private Vector3 originalPosition; // Store the original position
    private bool isFacingPlayer = false; // Whether the NPC should face the player
    private float resetDistance = 3f; // Distance at which the NPC resets its position
    private NPCInteractable npcInteractable; // Reference to the NPCInteractable component

    private void Start()
    {
        // Store the original rotation and position of the NPC
        originalRotation = transform.rotation;
        originalPosition = transform.position;
        npcInteractable = GetComponent<NPCInteractable>(); // Get the NPCInteractable component
    }

    private void Update()
    {
        // Only update rotation if the NPC is facing the player
        if (isFacingPlayer && player != null)
        {
            FacePlayer();
        }

        // Example: Trigger the Interact method when the player presses the 'E' key (or any other interaction button)
        if (Input.GetKeyDown(KeyCode.E) && npcInteractable != null)
        {
            Interact();
        }

        // Check the player's distance from the NPC and reset if necessary
        CheckPlayerDistance();
    }

    private void FacePlayer()
    {
        // If the NPC should face the player, calculate the direction and update rotation
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep direction on the horizontal plane

        // Create a rotation to look at the player
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate the NPC towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }

    // Call this method to make the NPC face the player
    public void StartFacingPlayer()
    {
        isFacingPlayer = true;
    }

    // Call this method to reset the NPC's rotation and position
    public void StopFacingPlayer()
    {
        isFacingPlayer = false;

        // Immediately reset to the original rotation and position
        transform.rotation = originalRotation;
      
    }

    // Method to interact with NPC
    public void Interact()
    {
        // Trigger the NPCInteractable's Interact method
        if (npcInteractable != null)
        {
            npcInteractable.Interact();
        }

        // After interaction, make the NPC face the player
        StartFacingPlayer();
    }

    // Method to check if the player has moved past the reset distance
    private void CheckPlayerDistance()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // If the player moves past the reset distance, reset NPC's position and rotation
            if (distanceToPlayer > resetDistance && isFacingPlayer)
            {
                StopFacingPlayer(); // Reset the NPC when the player is out of range
            }
        }
    }
}
