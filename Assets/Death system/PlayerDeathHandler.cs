using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeathHandler : MonoBehaviour
{
    public CanvasGroup deathBackgroundGroup;  // Attach CanvasGroup component here
    public float fadeDuration = 1f;  // Duration for fading
    public GameObject player;  // The player object to reset

    public AudioClip deathSoundClip;  // Drag and drop the death sound clip here in the inspector

    private bool isDead = false;

    // List of traps that the player can die from
    // Each trap object will have a corresponding respawn point
    [System.Serializable]
    public class Trap
    {
        public GameObject trapObject;  // The trap object that kills the player
        public Transform respawnPoint; // The respawn point for this specific trap
    }

    public List<Trap> trapObjects;  // List of trap objects and their respawn points

    private Transform currentRespawnPoint;  // The current respawn point assigned to the trap

    void Start()
    {
        // Optionally set initial conditions (if necessary)
    }

    void Update()
    {
        // Check for the "D" key press (for debugging purposes, remove or replace later)
        if (Input.GetKeyDown(KeyCode.D))
        {
            HandleDeath();  // Trigger death effect
        }

        // Check if the player is interacting with any trap
        foreach (var trap in trapObjects)
        {
            if (trap.trapObject == null) continue;  // Skip if trap is not set

            // Add your condition here to check if player touches or dies from this trap
            // Example: if player collides with trap object, set the respawn point
            // This is an example and should be replaced with the actual collision logic
            if (playerIsTouchingTrap(trap.trapObject))
            {
                currentRespawnPoint = trap.respawnPoint;
                HandleDeath();
                break;
            }
        }
    }

    private bool playerIsTouchingTrap(GameObject trap)
    {
        // Implement collision detection logic here, for example, using triggers
        // Return true if the player is touching the trap
        return false;  // Placeholder for actual trap interaction check
    }

    public void HandleDeath()
    {
        if (isDead) return;  // Prevent multiple death triggers

        isDead = true;

        // Start fading the background to black and reset player position at the same time
        StartCoroutine(FadeToBlackAndResetPlayer());

        // Wait for both the fade and reset to finish before starting the glitch effect
        StartCoroutine(WaitAndTriggerGlitch());
    }

    private IEnumerator FadeToBlackAndResetPlayer()
    {
        // Start fading the background to black
        float elapsedTime = 0f;
        deathBackgroundGroup.alpha = 0;  // Start with fully transparent

        // Fade in (black background appears)
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            deathBackgroundGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            yield return null;
        }

        // Simultaneously, reset player position (without waiting for fade-out)
        StartCoroutine(ResetPlayerPosition());

        // Wait before fading out (1 second of black screen)
        yield return new WaitForSeconds(1f);

        // Fade out (black background disappears)
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            deathBackgroundGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        // After fade out, mark the player as alive
        isDead = false;
    }

    private IEnumerator WaitAndTriggerGlitch()
    {
        // Wait for fade-in and reset player position to finish
        yield return new WaitForSeconds(fadeDuration + 1f);  // Wait for fade-in time + 1 second of black screen

        // Trigger glitch effect after both actions are done
        StartCoroutine(GlitchEffect());
    }

    private IEnumerator GlitchEffect()
    {

        // Play the death sound at the player's position when the glitch effect starts
        if (deathSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(deathSoundClip, player.transform.position);  // Play the death sound clip at the player's position
        }

        // A simple glitch effect (for example, shaking)
        Vector3 originalPosition = player.transform.position;
        float glitchDuration = 0.5f;
        float timer = 0f;

        while (timer < glitchDuration)
        {
            float xOffset = UnityEngine.Random.Range(-0.1f, 0.1f); // Explicitly use UnityEngine.Random
            float yOffset = UnityEngine.Random.Range(-0.1f, 0.1f); // Explicitly use UnityEngine.Random
            player.transform.position = originalPosition + new Vector3(xOffset, yOffset, 0);

            timer += Time.deltaTime;
            yield return null;
        }

        // Reset position after glitch effect
        player.transform.position = originalPosition;
    }

    private IEnumerator ResetPlayerPosition()
    {
        yield return new WaitForSeconds(0.5f); // Wait a moment before respawning
        if (currentRespawnPoint != null)
        {
            UnityEngine.Debug.Log("Teleporting player to respawn point at: " + currentRespawnPoint.position);  // Debug log
            player.transform.position = currentRespawnPoint.position;  // Teleport player to respawn point
        }
    }
}
