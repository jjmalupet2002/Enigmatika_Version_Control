using UnityEngine;

public class ProximityOutline : MonoBehaviour
{
    public float detectionRadius = 5f; // The radius within which the player must be to enable the outline

    private GameObject player;
    private Outline outline;
    private bool isOutlineForcedOff = false; // Flag to force the outline off

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
        if (player != null && !isOutlineForcedOff)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            outline.enabled = distance <= detectionRadius; // Enable/disable the outline based on the player's distance
        }
    }

    public void ForceDisableOutline(bool forceOff)
    {
        isOutlineForcedOff = forceOff;
        if (isOutlineForcedOff)
        {
            outline.enabled = false; // Ensure the outline is disabled if forced off
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to visualize the detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
