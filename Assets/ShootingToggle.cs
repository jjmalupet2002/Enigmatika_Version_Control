using UnityEngine;

[RequireComponent(typeof(ProjectileShooter))]
public class ShootingToggle : MonoBehaviour
{
    private ProjectileShooter projectileShooter;

    // This variable will be shown as a checkbox in the inspector
    [SerializeField]
    private bool enableShooting;

    private void Awake()
    {
        projectileShooter = GetComponent<ProjectileShooter>();
    }

    private void Update()
    {
        // Enable or disable shooting based on the checkbox
        if (enableShooting)
        {
            projectileShooter.EnableShooting();
        }
        else
        {
            projectileShooter.DisableShooting();
        }
    }
}