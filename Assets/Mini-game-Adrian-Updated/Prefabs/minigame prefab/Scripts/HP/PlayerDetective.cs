using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetective : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public PlayerHealthBar playerhealthBar;
    public GameOver gameOverManager;

    AudioManager audioManager;

    public bool canDie = false; // <--- Add this flag to control death behavior

    private void Awake()
    {
        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject != null)
        {
            audioManager = audioObject.GetComponent<AudioManager>();
            if (audioManager == null)
            {
                UnityEngine.Debug.LogError("AudioManager component not found on the object with tag 'Audio'.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("No GameObject with tag 'Audio' found.");
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        playerhealthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20); // Example: Manual damage for testing
        }
    }

    public void TakeDamage(int damage)
    {
        audioManager.PlaySFX(audioManager.bonecrack); // Play damage sound effect

        UnityEngine.Debug.Log($"Player took damage: {damage}");
        currentHealth -= damage;

        // Clamp health to avoid negative values
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the health bar
        playerhealthBar.SetHealth(currentHealth);

        // Check if the player is dead
        if (currentHealth <= 0 && canDie)
        {
            Die();
        }
    }

    void Die()
    {
        UnityEngine.Debug.Log("Player Died!");

        gameObject.SetActive(false); // Disable player

        gameOverManager.ShowGameOver(); // Shows Game Over Screen
    }
}
