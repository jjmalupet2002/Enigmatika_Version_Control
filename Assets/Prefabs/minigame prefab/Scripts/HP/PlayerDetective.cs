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

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        playerhealthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
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

        Debug.Log($"Player took damage: {damage}");
        currentHealth -= damage;

        // Clamp health to avoid negative values
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Update the health bar
        playerhealthBar.SetHealth(currentHealth);

        // Check if the player is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player Died!");

        gameObject.SetActive(false); // Disable player
            
        gameOverManager.ShowGameOver(); // Shows Game Over Screen
       
    }

}