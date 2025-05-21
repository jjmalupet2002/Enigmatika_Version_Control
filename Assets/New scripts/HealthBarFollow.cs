using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarFollow : MonoBehaviour
{
    [Header("Tracking Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, 0);

    [Header("HP UI References")]
    public Image hpFillImage;
    public RawImage hpBorderImage;
    public PlayerHealthBar playerHealthBar;

    [Header("Black Background Fade Settings")]
    public CanvasGroup blackBackground;

    [Header("Checkpoint Settings")]
    public Transform playerDieCheckpoint;
    public Transform hpTriggerCheckpoint;
    public float interactRange = 5f;

    [Header("Music References")]
    public AudioSource mainBackgroundMusic;
    public AudioSource challengeMusic;

    private bool isPlayerInRange = false;
    private bool wasPlayerInRangeLastFrame = false;
    private bool isDead = false;
    private bool isRespawning = false;

    private float previousHealth = -1f; // Initialize to -1 to force update on start

    void LateUpdate()
    {
        // Position health bar above player
        if (target != null)
        {
            transform.position = target.position + offset;
        }

        // Check player distance to hpTriggerCheckpoint
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

        // Show or hide HP UI
        if (hpFillImage != null) hpFillImage.enabled = isPlayerInRange;
        if (hpBorderImage != null) hpBorderImage.enabled = isPlayerInRange;

        // Play or stop challenge music
        HandleMusicSwitching();

        // Log health reduction
        if (playerHealthBar != null)
        {
            float currentHealth = playerHealthBar.slider.value;
            if (previousHealth >= 0 && currentHealth < previousHealth)
            {
                UnityEngine.Debug.Log($"[Health Log] Player health reduced: {previousHealth} → {currentHealth}");
            }
            previousHealth = currentHealth;

            // If health reaches 0, trigger death logic
            if (currentHealth <= 0 && !isDead && !isRespawning)
            {
                UnityEngine.Debug.Log("[Health Log] Player has died.");
                isDead = true;
                isRespawning = true;
                StartCoroutine(FadeOutBlackBackground());
            }
        }

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

    private IEnumerator FadeOutBlackBackground()
    {
        float halfFadeDuration = 0.75f;

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

        elapsedTime = 0f;
        while (elapsedTime < halfFadeDuration)
        {
            blackBackground.alpha = Mathf.Lerp(1f, 0f, elapsedTime / halfFadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blackBackground.alpha = 0f;

        isDead = false;
        isRespawning = false;
    }

    private void ResetPlayerPosition()
    {
        if (target != null && playerDieCheckpoint != null)
        {
            UnityEngine.Debug.Log("[Health Log] Player position reset to checkpoint.");
            target.gameObject.SetActive(true);
            target.position = playerDieCheckpoint.position;
            playerHealthBar.SetHealth((int)playerHealthBar.slider.maxValue);

            // Reset health tracker
            previousHealth = playerHealthBar.slider.value;

            isDead = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (hpTriggerCheckpoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(hpTriggerCheckpoint.position, interactRange);
        }
    }
}
