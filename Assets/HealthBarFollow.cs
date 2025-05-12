using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarFollow : MonoBehaviour
{
    [Header("Tracking Settings")]
    public Transform target; // Reference to the player
    public Vector3 offset = new Vector3(0, 2, 0); // Offset above the player

    [Header("HP UI References")]
    public Image hpFillImage;         // Reference to the fill image (disable in inspector)
    public RawImage hpBorderImage;    // Reference to the border image (disable in inspector)
    public PlayerHealthBar playerHealthBar; // Reference to the PlayerHealthBar script

    [Header("Black Background Fade Settings")]
    public CanvasGroup blackBackground;  // Reference to the black background image with CanvasGroup

    [Header("Checkpoint Settings")]
    public Transform playerDieCheckpoint; // Player's death checkpoint position
    public float interactRange = 5f;      // Distance at which HP UI appears

    private bool isDead = false;          // Flag to check if the player is dead

    void LateUpdate()
    {
        // Position health bar above player
        if (target != null)
        {
            transform.position = target.position + offset;
        }

        // Check player distance to checkpoint using overlap sphere
        bool isPlayerInRange = false;
        if (playerDieCheckpoint != null)
        {
            Collider[] hits = Physics.OverlapSphere(playerDieCheckpoint.position, interactRange);
            foreach (var hit in hits)
            {
                if (hit.transform == target)
                {
                    isPlayerInRange = true;
                    break;
                }
            }
        }

        // Show or hide HP UI based on proximity
        if (hpFillImage != null) hpFillImage.enabled = isPlayerInRange;
        if (hpBorderImage != null) hpBorderImage.enabled = isPlayerInRange;

        // If health reaches 0, trigger death logic
        if (playerHealthBar != null && playerHealthBar.slider.value <= 0 && !isDead)
        {
            isDead = true;  // Flag that the player is dead

            // Fade out the black background (simulating death screen fade)
            if (blackBackground != null)
            {
                StartCoroutine(FadeOutBlackBackground());
            }

            // Reset player position to the checkpoint (simultaneously with fade)
            ResetPlayerPosition();
        }
    }

    // Coroutine to fade out the black background
    private IEnumerator FadeOutBlackBackground()
    {
        float fadeDuration = 2f; // Duration for the fade
        float startAlpha = blackBackground.alpha;
        float targetAlpha = 1f; // Fully visible (black background)

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            blackBackground.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blackBackground.alpha = targetAlpha; // Ensure it's fully faded
    }

    // Method to reset the player position to the checkpoint
    private void ResetPlayerPosition()
    {
        if (target != null && playerDieCheckpoint != null)
        {
            target.position = playerDieCheckpoint.position; // Reset to checkpoint
            playerHealthBar.SetHealth((int)playerHealthBar.slider.maxValue); // Reset health to max
        }
    }

    // Optional: draw the interaction range in the scene view
    private void OnDrawGizmosSelected()
    {
        if (playerDieCheckpoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(playerDieCheckpoint.position, interactRange);
        }
    }
}
