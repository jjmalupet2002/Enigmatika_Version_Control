using UnityEngine;

public class StairCollisionHandler : MonoBehaviour
{
    public Rigidbody playerRigid; // Reference to Rigidbody
    public float slowedMoveSpeed = 3f; // Slowed down speed for stairs
    private float originalMoveSpeed;

    private bool isGrounded; // Set this appropriately in your existing logic

    private void Start()
    {
        // Store the original move speed if needed
        originalMoveSpeed = slowedMoveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stairs"))
        {
            HandleStairCollision();
        }
    }

    private void HandleStairCollision()
    {
        // Debug message for confirmation
        UnityEngine.Debug.Log("Player collided with stairs");

        // Adjust the player's Rigidbody properties to simulate slowed movement
        if (playerRigid != null)
        {
            playerRigid.drag = 5f; // Increase drag to slow down the player
        }

        // Call the step climbing logic
        stepClimb();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stairs"))
        {
            HandleStairExit();
        }
    }

    private void HandleStairExit()
    {
        // Reset player's Rigidbody properties to original state if necessary
        if (playerRigid != null)
        {
            playerRigid.drag = 0f; // Reset drag
        }
    }

    void FixedUpdate()
    {
        // Check if the player is grounded; this should be set in your existing movement logic
        isGrounded = CheckIfGrounded(); // Implement this based on your existing logic

        // Call stepClimb in FixedUpdate to constantly check for climbing
        stepClimb();
    }

    void stepClimb()
    {
        if (playerRigid != null)
        {
            RaycastHit hitLower;

            // Perform a raycast downwards from the player's position
            Vector3 origin = transform.position + Vector3.down * 0.1f; // Adjust the offset as needed
            if (Physics.Raycast(origin, Vector3.down, out hitLower, 2f))
            {
                // Check if the player is grounded and the step height is within range
                if (isGrounded && (transform.position.y - hitLower.point.y <= 0.5f)) // Adjust step height as necessary
                {
                    Vector3 targetVector = new Vector3(playerRigid.position.x, hitLower.point.y, playerRigid.position.z);
                    playerRigid.position = Vector3.Lerp(playerRigid.position, targetVector, Time.deltaTime / 0.1f);
                    playerRigid.velocity = new Vector3(playerRigid.velocity.x, 0, playerRigid.velocity.z);
                }

                // Check if the player is not grounded and the step height is within range
                if (!isGrounded && (transform.position.y - hitLower.point.y <= 0.5f)) // Adjust step height as necessary
                {
                    playerRigid.position = new Vector3(playerRigid.position.x, hitLower.point.y, playerRigid.position.z);
                    playerRigid.velocity = new Vector3(playerRigid.velocity.x, 0, playerRigid.velocity.z);
                }
            }
        }
    }

    private bool CheckIfGrounded()
    {
        // Define a layer mask for the ground to optimize raycasting
        LayerMask groundLayer = LayerMask.GetMask("Ground"); // Assuming your ground objects are tagged with "Ground"

        // Cast a ray from the player's position downward
        RaycastHit hit;

        // Adjust the origin point to the bottom of the player, adding an offset
        Vector3 origin = transform.position + Vector3.down * 0.1f; // Adjust the offset as needed
        if (Physics.Raycast(origin, Vector3.down, out hit, 0.2f, groundLayer)) // 0.2f is the ray distance
        {
            // Optional: Log to confirm it's grounded
            UnityEngine.Debug.Log("Player is grounded");
            return true; // The player is grounded
        }
        return false; // The player is not grounded
    }
}
