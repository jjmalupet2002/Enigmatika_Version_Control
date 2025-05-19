using UnityEngine;

[RequireComponent(typeof(ProjectileShooter))]
public class ShootingToggle : MonoBehaviour
{
    private ProjectileShooter projectileShooter;

    [SerializeField]
    private bool enableShooting;

    private void Awake()
    {
        projectileShooter = GetComponent<ProjectileShooter>();
    }

    private void Update()
    {
        if (enableShooting)
        {
            projectileShooter.EnableShooting();
        }
        else
        {
            projectileShooter.DisableShooting();
        }
    }

    // Public method to toggle shooting cleanly
    public void SetShootingEnabled(bool enabled)
    {
        enableShooting = enabled;
    }
}
