using System.Collections;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab for the projectile
    public Transform shootPoint;       // Where the projectile spawns
    public float projectileSpeed = 10f; // Speed of the projectile

    public float shootInterval = 0.5f; // Time between each shot
    public float restPeriod = 5f;      // Rest period after shooting for 5 seconds
    public Transform player;           // Reference to the player's Transform

    void Start()
    {
        StartCoroutine(ShootAtPlayer());
    }

    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            float startTime = Time.time;

            // Shoot for 5 seconds
            while (Time.time - startTime < 5f)
            {
                if (player != null) // Ensure the player exists
                {
                    ShootProjectileAtPlayer();
                }
                yield return new WaitForSeconds(shootInterval); // Wait before shooting again
            }

            // Rest period
            yield return new WaitForSeconds(restPeriod);
        }
    }

    private void ShootProjectileAtPlayer()
    {
        // Instantiate the projectile at the shoot point
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        // Calculate the direction to the player on the same horizontal plane (ignore height)
        Vector3 directionToPlayer = (player.position - shootPoint.position).normalized;
        directionToPlayer.y = 0; // Ignore the Y-axis to keep the projectile on the same horizontal level
        directionToPlayer.Normalize();

        // Set the projectile's velocity
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = directionToPlayer * projectileSpeed;
        }

        // Destroy the projectile after a certain time to prevent clutter
        Destroy(projectile, 5f); // Adjust time as needed
    }
}