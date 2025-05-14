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
    public Transform hpTriggerCheckpoint; // New checkpoint for proximity detection to enable HP UI
    public float interactRange = 5f;      // Distance at which HP UI appears

    [Header("Music References")]
    public AudioSource mainBackgroundMusic;
    public AudioSource challengeMusic;

    private bool isPlayerInRange = false;
    private bool wasPlayerInRangeLastFrame = false;
    private bool isDead = false; // Flag to check if the player is dead
    private bool isRespawning = false; // NEW flag

    void LateUpdate()
    {
        // Position health bar above player
        if (target != null)
        {
            transform.position = target.position + offset;
        }

        // Check player distance to hpTriggerCheckpoint using overlap sphere
        isPlayerInRange = false;
        if (hpTriggerCheckpoint != null)
        {
            Collider[] hits = Physics.OverlapSphere(hpTriggerCheckpoint.position, interactRange);
            foreach (var hit in hits)
            {
                if (hit.transform == target)
                {
                    isPlayerInRange = true;
                    break;
                }
            }
        }

        // Show or hide HP UI based on proximity to hpTriggerCheckpoint
        if (hpFillImage != null) hpFillImage.enabled = isPlayerInRange;
        if (hpBorderImage != null) hpBorderImage.enabled = isPlayerInRange;

        // Play or stop challenge music based on proximity
        HandleMusicSwitching();

        // If health reaches 0, trigger death logic
        if (playerHealthBar != null && playerHealthBar.slider.value <= 0 && !isDead && !isRespawning)
        {
            isDead = true;
            isRespawning = true;
            StartCoroutine(FadeOutBlackBackground());
        }

        // Update last frame flag
        wasPlayerInRangeLastFrame = isPlayerInRange;
    }

    private void HandleMusicSwitching()
    {
        if (isPlayerInRange && !wasPlayerInRangeLastFrame)
        {
            if (mainBackgroundMusic != null && mainBackgroundMusic.isPlaying)
                mainBackgroundMusic.Stop();

            if (challengeMusic != null && !challengeMusic.isPlaying)
                challengeMusic.Play();
        }
        else if (!isPlayerInRange && wasPlayerInRangeLastFrame)
        {
            if (challengeMusic != null && challengeMusic.isPlaying)
                challengeMusic.Stop();

            if (mainBackgroundMusic != null && !mainBackgroundMusic.isPlaying)
                mainBackgroundMusic.Play();
        }
    }

    // Coroutine to fade out and in the black background (total 1.5 seconds)
    private IEnumerator FadeOutBlackBackground()
    {
        float halfFadeDuration = 0.75f;

        // Fade to black
        float elapsedTime = 0f;
        while (elapsedTime < halfFadeDuration)
        {
            blackBackground.alpha = Mathf.Lerp(0f, 1f, elapsedTime / halfFadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blackBackground.alpha = 1f;

        // Respawn
        ResetPlayerPosition();

        // Fade back to transparent
        elapsedTime = 0f;
        while (elapsedTime < halfFadeDuration)
        {
            blackBackground.alpha = Mathf.Lerp(1f, 0f, elapsedTime / halfFadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blackBackground.alpha = 0f;

        // Safe to reset both flags here
        isDead = false;
        isRespawning = false;
    }

    private void ResetPlayerPosition()
    {
        if (target != null && playerDieCheckpoint != null)
        {
            target.gameObject.SetActive(true); // Reactivate player if disabled
            target.position = playerDieCheckpoint.position;
            playerHealthBar.SetHealth((int)playerHealthBar.slider.maxValue);

            // Reset death flag
            isDead = false;
        }
    }

    // Optional: draw the interaction range in the scene view
    private void OnDrawGizmosSelected()
    {
        if (hpTriggerCheckpoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(hpTriggerCheckpoint.position, interactRange);
        }
    }
}
