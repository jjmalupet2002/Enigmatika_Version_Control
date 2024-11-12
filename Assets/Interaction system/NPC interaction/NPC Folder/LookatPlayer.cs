using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform player; // Assign the player transform in the Inspector
    private Quaternion originalRotation; // Store the original rotation
    private Vector3 originalPosition; // Store the original position
    private bool isFacingPlayer = false; // Whether the NPC should face the player
    private float resetDistance = 1f; // Distance threshold to reset NPC

    private void Start()
    {
        // Store the original rotation and position of the NPC
        originalRotation = transform.rotation;
        originalPosition = transform.position;
    }

    private void Update()
    {
        if (isFacingPlayer && player != null)
        {
            FacePlayer();

            // Calculate the distance between the player and the NPC
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Check if the player is beyond the reset distance
            if (distanceToPlayer > resetDistance)
            {
                // Reset NPC position and rotation
                StopFacingPlayer();
            }
        }
    }

    private void FacePlayer()
    {
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

        // Immediately reset to the original rotation
        transform.rotation = originalRotation;

        
    }
}
