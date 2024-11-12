using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestButtonHandler : MonoBehaviour
{
    [Header("Quest Button Settings")]
    public GameObject openQuestButton; // Reference to the Quest button
    public float interactRange = 5f;   // Interaction range for the NPC (visualization)

    private Transform playerTransform;
    private Collider npcCollider;

    void Start()
    {
        npcCollider = GetComponent<Collider>(); // Get the collider attached to the NPC
    }

    void Update()
    {
        if (playerTransform != null)
        {
            // Check if the player is within the interact range
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance <= interactRange)
            {
                openQuestButton.SetActive(true);  // Show the quest button when within range
            }
            else
            {
                openQuestButton.SetActive(false); // Hide the quest button when out of range
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player enters the collision range
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            openQuestButton.SetActive(true); // Show the quest button when the player enters the collider
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Hide the quest button and clear player reference when the player exits the range
        if (collision.gameObject.CompareTag("Player"))
        {
            playerTransform = null;
            openQuestButton.SetActive(false); // Hide the quest button immediately when the player exits the collider
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the interaction range in the scene view (for visualization purposes only)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
