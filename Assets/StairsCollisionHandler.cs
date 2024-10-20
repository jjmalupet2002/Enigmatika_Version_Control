using System.Diagnostics;
using UnityEngine;

public class StairCollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Stairs"))
        {
            HandleStairCollision();
        }
    }

    private void HandleStairCollision()
    {
        // Implement logic to handle stair collision
        UnityEngine.Debug.Log("Player collided with stairs");
        // Example: Adjust player's Rigidbody properties, play sound, etc.
    }

    private void Update()
    {
        // Example raycast handling logic for detecting stairs
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider.CompareTag("Stairs"))
            {
                HandleStairCollision();
            }
        }
    }
}
