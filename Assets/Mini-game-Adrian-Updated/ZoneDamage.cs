using UnityEngine;

public class ZoneDamage : MonoBehaviour
{
    [Header("References")]
    public GameObject playerObject; // Drag your player GameObject here in Inspector

    [Header("Damage Settings")]
    public int damageAmount = 20;

    [Header("Reset Settings")]
    public Vector3 resetPosition = Vector3.zero;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name); // Should show up now

        if (other.gameObject == playerObject)
        {
            Debug.Log("Player matched. Applying damage.");

            PlayerDetective playerScript = playerObject.GetComponent<PlayerDetective>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(damageAmount);
                playerObject.transform.position = resetPosition;
            }
            else
            {
                Debug.LogWarning("PlayerDetective component not found!");
            }
        }
    }
}