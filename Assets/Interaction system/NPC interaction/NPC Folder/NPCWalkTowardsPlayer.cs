using UnityEngine;

public class NPCWalkTowardsPlayer : MonoBehaviour
{
    public float detectionRadius = 10f; // Radius within which the NPC detects the player
    public float stopDistance = 2f; // Distance at which the NPC stops moving towards the player
    public float moveSpeed = 2f; // Speed at which the NPC moves towards the player
    public Transform player; // Reference to the player's transform

    private Animator animator; // Reference to the NPC's Animator

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(player.position.x, 0, player.position.z));
        UnityEngine.Debug.Log("Distance to player: " + distanceToPlayer);

        if (distanceToPlayer <= detectionRadius && distanceToPlayer > stopDistance)
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
        Vector3 direction = (new Vector3(player.position.x, 0, player.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;
        newPosition.y = transform.position.y; // Keep the same Y position to prevent clipping
        transform.position = newPosition;

        UnityEngine.Debug.Log("Moving towards player");

        if (animator != null)
        {
            animator.SetTrigger("Walking");
        }
    }

    private void Idle()
    {
        UnityEngine.Debug.Log("Idling");

        if (animator != null)
        {
            animator.SetTrigger("Idle");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
