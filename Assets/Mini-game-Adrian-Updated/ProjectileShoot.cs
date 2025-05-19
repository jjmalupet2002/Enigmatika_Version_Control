using System.Collections;
using System.Collections.Generic;
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

    // Define a static event for projectile spawn
    public static System.Action OnProjectileSpawned;

    [Header("Shooting Mode")]
    public bool shootStraight = false; // If true, projectiles shoot forward

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
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        activeProjectiles.Add(projectile);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction;

            if (shootStraight)
            {
                // Shoot straight in the forward direction of the shoot point
                direction = shootPoint.forward;
            }
            else
            {
                // Shoot toward the player (horizontal only)
                direction = (player.position - shootPoint.position);
                direction.y = 0;
                direction.Normalize();
            }

            rb.velocity = direction * projectileSpeed;
        }

        OnProjectileSpawned?.Invoke();
        Destroy(projectile, 5f);
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