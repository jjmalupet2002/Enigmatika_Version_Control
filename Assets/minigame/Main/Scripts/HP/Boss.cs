using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public BossHealthBar bosshealthBar;

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
        currentHealth -= damage;

        // Ensure health doesn't go below zero
        currentHealth = Mathf.Max(currentHealth, 0);

        bosshealthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("Boss Defeated!");
            // Add any additional logic for when the boss is defeated
        }
    }
}