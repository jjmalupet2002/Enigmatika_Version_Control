using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public BossHealthBar bosshealthBar;
    public MissionComplete missioncompleteManager;

    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        bosshealthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            TakeDamage(20); // Debugging: Damage boss with 'B' key
        }
    }

    // Make TakeDamage public so it can be called by other scripts
    public void TakeDamage(int damage)
    {
        audioManager.PlaySFX(audioManager.oof); // Play damage sound effect
        currentHealth -= damage;

        // Ensure health doesn't go below zero
        currentHealth = Mathf.Max(currentHealth, 0);

        bosshealthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Boss Defeated!");

            Die();
        }
    }

    public void HealDamage(int damage)
    {
        
        currentHealth += damage;

        // Ensure health doesn't go below zero
        currentHealth = Mathf.Max(currentHealth, 0);

        bosshealthBar.SetHealth(currentHealth);
    }

    void Die()
    {
        Debug.Log("Boss Died!");

        missioncompleteManager.ShowMissionComplete(); // Shows Game Over Screen

    }
}