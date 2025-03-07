using UnityEngine;

public class Theguythrowanimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to the model
        animator = GetComponent<Animator>();

        // Subscribe to the projectile spawn event
        ProjectileShooter.OnProjectileSpawned += TriggerThrowAnimation;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to avoid memory leaks
        ProjectileShooter.OnProjectileSpawned -= TriggerThrowAnimation;
    }

    private void TriggerThrowAnimation()
    {
        // Trigger the throw animation
        if (animator != null)
        {
            animator.SetTrigger("ThrowTrigger"); // Replace with your trigger parameter name
        }
        else
        {
            Debug.LogWarning("Animator component not found!");
        }
    }
}