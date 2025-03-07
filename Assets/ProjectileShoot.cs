using System.Collections;
using System.Collections.Generic; // Add this line to include the System.Collections.Generic namespace
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    public GameObject projectilePrefab; // Prefab for the projectile
    public Transform shootPoint;       // Where the projectile spawns
    public float projectileSpeed = 10f; // Speed of the projectile

    public float shootInterval = 0.5f; // Time between each shot
    public Transform player;           // Reference to the player's Transform

    private bool isShootingEnabled = false; // Controls whether shooting is active
    private List<GameObject> activeProjectiles = new List<GameObject>(); // Track active projectiles

    void Start()
    {
        // Start the shooting coroutine, but it will only shoot when isShootingEnabled is true
        StartCoroutine(ShootAtPlayer());
    }

    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            // Only shoot if shooting is enabled
            if (isShootingEnabled && player != null)
            {
                ShootProjectileAtPlayer();
            }

            // Wait for the next shot
            yield return new WaitForSeconds(shootInterval);
        }
    }

    private void ShootProjectileAtPlayer()
    {
        // Instantiate the projectile at the shoot point
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);

        // Add the projectile to the list of active projectiles
        activeProjectiles.Add(projectile);

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

    // Enable shooting (called when switching to top-down camera)
    public void EnableShooting()
    {
        isShootingEnabled = true;
    }

    // Disable shooting (called when switching to close-up camera)
    public void DisableShooting()
    {
        isShootingEnabled = false;

        // Destroy all active projectiles
        DestroyAllProjectiles();
    }

    // Destroy all active projectiles
    private void DestroyAllProjectiles()
    {
        foreach (var projectile in activeProjectiles)
        {
            if (projectile != null)
            {
                Destroy(projectile);
            }
        }

        // Clear the list of active projectiles
        activeProjectiles.Clear();
    }
}