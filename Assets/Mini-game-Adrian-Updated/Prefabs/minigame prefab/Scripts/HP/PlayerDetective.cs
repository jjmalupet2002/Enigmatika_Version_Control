using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDetective : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public PlayerHealthBar playerhealthBar;
    
    // Replace specific GameOver reference with a more flexible system
    public UnityEvent onGameOver;  // Unity event that will be triggered on game over
    
    // Optional reference for GameObject that handles game over
    public MonoBehaviour gameOverHandler;  // Can be any component/script
    
    // Method name to call on the gameOverHandler
    public string gameOverMethodName = "ShowGameOver";

    AudioManager audioManager;

    public bool canDie = false;

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
        
        // Initialize UnityEvent if null
        if (onGameOver == null)
            onGameOver = new UnityEvent();
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
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.bonecrack); // Play damage sound effect

        UnityEngine.Debug.Log($"Player took damage: {damage}");
        currentHealth -= damage;

        // Clamp health to avoid negative values
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the health bar
        if (playerhealthBar != null)
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

        // Trigger game over with multiple options
        TriggerGameOver();
        
        gameObject.SetActive(false); // Disable player
    }
    
    private void TriggerGameOver()
    {
        // Option 1: Invoke the Unity Event
        onGameOver.Invoke();
        
        // Option 2: Use the gameOverHandler if provided
        if (gameOverHandler != null && !string.IsNullOrEmpty(gameOverMethodName))
        {
            gameOverHandler.SendMessage(gameOverMethodName, SendMessageOptions.DontRequireReceiver);
        }
    }
}