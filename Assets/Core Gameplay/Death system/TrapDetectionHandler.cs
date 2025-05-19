using System.Diagnostics;
using UnityEngine;

public class TrapDetectionHandler : MonoBehaviour
{
    public PlayerDeathHandler playerDeathHandler;  // Reference to PlayerDeathHandler script

    void Start()
    {
        if (playerDeathHandler == null)
        {
            UnityEngine.Debug.LogError("PlayerDeathHandler not assigned!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))  // Check if the player collided with the trap
        {
            // Notify the PlayerDeathHandler to handle death effect and player reset
            playerDeathHandler.HandleDeath();
        }
    }
}
