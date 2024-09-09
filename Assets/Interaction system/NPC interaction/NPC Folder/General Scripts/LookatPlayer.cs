using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform player; // Assign the player transform in the Inspector
    private Quaternion originalRotation; // Store the original rotation
    private Vector3 originalPosition; // Store the original position
    private bool isFacingPlayer = false; // Whether the NPC should face the player
    private float resetDistance = 5f; // Distance threshold to reset NPC

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
        direction = -direction; // Reverse direction if needed

        // Create a rotation to look at the player
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Extract the y rotation from the target rotation
        Vector3 eulerAngles = targetRotation.eulerAngles;
        eulerAngles.x = transform.eulerAngles.x; // Preserve the original x rotation

        // Create the final rotation to apply
        Quaternion finalRotation = Quaternion.Euler(eulerAngles);

     
        // Smoothly rotate the NPC towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, finalRotation, Time.deltaTime * 5f);
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

        // Immediately reset to the original position
        transform.position = originalPosition;
    }
}
