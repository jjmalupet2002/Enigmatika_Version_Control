using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSimonBattle : MonoBehaviour
{
    [Header("Components")]
    [Tooltip("Reference to the SimonSaysGrid script")]
    public SimonSaysGrid simonGrid;
    [Tooltip("Reference to the Boss script")]
    public Boss boss;
    [Tooltip("The player GameObject")]
    public GameObject playerObject;

    [Header("Boss Attack Settings")]
    [Tooltip("Damage dealt when player succeeds at Simon Says")]
    public int damagePerSuccess = 10;
    [Tooltip("Visual effect for successful hit")]
    public GameObject hitEffectPrefab;

    [Header("Player Settings")]
    [Tooltip("Layer that detects if player is on safe tile")]
    public LayerMask safeTileLayer;
    [Tooltip("Effect shown when player succeeds")]
    public GameObject playerSuccessEffect;
    [Tooltip("Effect shown when player fails")]
    public GameObject playerFailEffect;
    [Tooltip("Auto-damage boss when player succeeds (no button press needed)")]
    public bool autoDamageBoss = true;

    [Header("UI")]
    [Tooltip("Status text to show messages")]
    public Text statusText;

    // Private variables
    private bool playerWasOnSafeTile = false;
    private bool canDamageBoss = false;
    private AudioSource audioSource;
    
    void Start()
    {
        // Initialize components
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
            
        if (boss == null)
            boss = FindObjectOfType<Boss>();
            
        // Subscribe to SimonSaysGrid events
        if (simonGrid != null)
        {
            // Hook into the SimonSaysGrid's logic by extending it
            StartCoroutine(MonitorSimonSaysEvents());
        }
        else
        {
            Debug.LogError("SimonSaysGrid reference not set in BossSimonBattle!");
        }
    }
    
    IEnumerator MonitorSimonSaysEvents()
    {
        while (true)
        {
            // Wait for the tiles to start disappearing (when player is frozen)
            // This happens after showTextDelay + postCountdownPause in SimonSaysGrid
            yield return new WaitForSeconds(simonGrid.showTextDelay + simonGrid.postCountdownPause + 0.5f);
            
            // Check if player is on the safe tile
            CheckPlayerOnSafeTile();
            
            // Wait until tiles reappear and player can move again
            yield return new WaitForSeconds(simonGrid.hideDuration - 1f);
            
            
            // Process the result and allow damage if player was on safe tile
            ProcessSimonSaysResult();
            
            // If auto-damage is enabled and player survived (was on safe tile), damage the boss immediately
            if (autoDamageBoss && playerWasOnSafeTile)
            {
                AttemptDamageBoss();
            }
            else
            {
                // Give the player a brief window to manually attack the boss
                yield return new WaitForSeconds(3.0f);
            }
            
            // Reset for next round
            ResetSimonSaysRound();
            
            // Small delay before next round
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    void CheckPlayerOnSafeTile()
    {
        if (playerObject == null) return;
        
        // Perform a small overlap check at player's position to see if they're on a safe tile
        Collider[] hitColliders = Physics.OverlapSphere(playerObject.transform.position, 0.1f, safeTileLayer);
        
        playerWasOnSafeTile = hitColliders.Length > 0;
        
        if (playerWasOnSafeTile)
        {
            // Player is on the safe tile - they avoided damage
            if (statusText != null)
                statusText.text = "You found the safe tile!";
                
            if (playerSuccessEffect != null)
                Instantiate(playerSuccessEffect, playerObject.transform.position, Quaternion.identity);
                
            // Player avoided damage, so they can now damage the boss
            canDamageBoss = true;
            
            if (statusText != null)
                statusText.text = "Boss is vulnerable! Attack now!";
                
            // Optional: Add visual indicator that boss can be damaged
            if (boss != null)
            {
                // Change color or add particle effect to boss
                Renderer bossRenderer = boss.GetComponent<Renderer>();
                if (bossRenderer != null)
                    StartCoroutine(FlashVulnerable(bossRenderer));
            }
        }
        else
        {
            // Player is not on the safe tile - they took damage
            if (statusText != null)
                statusText.text = "You missed the safe tile!";
                
            if (playerFailEffect != null)
                Instantiate(playerFailEffect, playerObject.transform.position, Quaternion.identity);
                
            // Player took damage, so they cannot damage the boss
            canDamageBoss = false;
            
            // Apply damage to player if needed
            // You could add player damage logic here or use your existing ZoneDamage system
        }
    }
    
    void ProcessSimonSaysResult()
    {
        // This function is now simplified since we handle the vulnerability logic directly in CheckPlayerOnSafeTile
        if (!playerWasOnSafeTile)
        {
            // Player failed, boss is not vulnerable
            if (statusText != null)
                statusText.text = "You cannot damage the boss!";
        }
    }
    
    void ResetSimonSaysRound()
    {
        // Reset for next round
        canDamageBoss = false;
        playerWasOnSafeTile = false;
        
        if (statusText != null)
            statusText.text = "Get ready for next round!";
            
        // Reset boss visual state if needed
        if (boss != null)
        {
            Renderer bossRenderer = boss.GetComponent<Renderer>();
            if (bossRenderer != null)
            {
                // Reset color or other visual indicators
                bossRenderer.material.color = Color.white;
            }
        }
    }
    
    IEnumerator FlashVulnerable(Renderer renderer)
    {
        Color originalColor = renderer.material.color;
        
        // Flash between red and original color
        for (int i = 0; i < 6; i++)
        {
            renderer.material.color = (i % 2 == 0) ? Color.red : originalColor;
            yield return new WaitForSeconds(0.5f);
            
            // Stop flashing if no longer vulnerable
            if (!canDamageBoss)
            {
                renderer.material.color = originalColor;
                yield break;
            }
        }
        
        renderer.material.color = originalColor;
    }
    
    // Call this method when the player attacks the boss (e.g., through button press or collision)
    public void AttemptDamageBoss()
    {
        if (canDamageBoss && boss != null)
        {
            // Use the boss's existing TakeDamage method
            boss.TakeDamage(damagePerSuccess);
            
            // Visual feedback
            PlayHitEffect();
            
            // Reset the damage flag so player can't keep hitting
            canDamageBoss = false;
            
            if (statusText != null)
                statusText.text = "Great hit! Boss took damage!";
        }
        else
        {
            if (statusText != null)
                statusText.text = "Boss is not vulnerable!";
        }
    }
    
    void PlayHitEffect()
    {
        // Spawn hit effect at boss position
        if (hitEffectPrefab != null && boss != null)
        {
            Instantiate(hitEffectPrefab, boss.transform.position, Quaternion.identity);
        }
    }
    
    // Unity editor gizmo to visualize the detection radius
    void OnDrawGizmosSelected()
    {
        if (playerObject != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerObject.transform.position, 0.1f);
        }
    }
}