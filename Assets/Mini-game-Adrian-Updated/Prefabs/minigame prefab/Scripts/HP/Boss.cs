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

        // Ensure health does not exceed max health
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        bosshealthBar.SetHealth(currentHealth);
    }


    void Die()
    {
        // Reset Leitner system
        if (WordManager.instance != null)
        {
            WordManager.instance.ResetLeitnerSystem();
        }

        missioncompleteManager.ShowMissionComplete(); // Shows Game Over Screen
    }

}