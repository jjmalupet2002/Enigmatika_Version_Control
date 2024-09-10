using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWalkTowardsPlayer : MonoBehaviour
{
    public float detectionRadius = 10f; // Radius within which the NPC detects the player
    public float moveSpeed = 2f; // Speed at which the NPC moves towards the player
    public Transform player; // Reference to the player's transform

    private Animator animator; // Reference to the NPC's Animator
    private Rigidbody rb; // Reference to the NPC's Rigidbody

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            MoveTowardsPlayer();
        }
        else
        {
            Idle();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        rb.MovePosition(newPosition);

        if (animator != null)
        {
            animator.SetTrigger("Walk");
        }
    }

    private void Idle()
    {
        if (animator != null)
        {
            animator.SetTrigger("Idle");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
