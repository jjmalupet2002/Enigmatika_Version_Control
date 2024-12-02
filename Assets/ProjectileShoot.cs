using System.Collections;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour
{
    // Projectile Prefab
    public GameObject projectilePrefab;

    // Shooting Parameters
    public float shootInterval = 0.2f; // Time between each shot
    public float shootDuration = 2.0f; // Duration of continuous shooting
    public float pauseDuration = 1.0f; // Pause duration after shooting

    // Projectile Speed
    public float projectileSpeed = 10f;

    // Shooting Direction
    public Transform shootPoint;

    private bool isShooting = true;

    void Start()
    {
        // Start the shooting cycle
        StartCoroutine(ShootingCycle());
    }

    private IEnumerator ShootingCycle()
    {
        while (true)
        {
            // Shooting phase (shoot for shootDuration)
            float shootEndTime = Time.time + shootDuration;
            while (isShooting && Time.time < shootEndTime)
            {
                ShootProjectile();
                yield return new WaitForSeconds(shootInterval);
            }

            // Pause phase (pause for pauseDuration)
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    private void ShootProjectile()
    {
        // Instantiate the projectile at the shoot point
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

        // Add velocity to the projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootPoint.forward * projectileSpeed;
        }

        // Destroy the projectile after a certain time to prevent clutter
        Destroy(projectile, 5f); // Adjust time if needed
    }
}