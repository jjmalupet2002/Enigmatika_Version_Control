using UnityEngine;

public class ProximityOutline : MonoBehaviour
{
    public float detectionRadius = 5f; // The radius within which the player must be to enable the outline

    private GameObject player;
    private Outline outline;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // Get the Outline component (already attached to the prefab)
        outline = gameObject.GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        // Disable the outline at start
        outline.enabled = false;
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= detectionRadius)
            {
                outline.enabled = true; // Enable the outline when the player is within the detection radius
            }
            else
            {
                outline.enabled = false; // Disable the outline when the player is outside the detection radius
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to visualize the detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
