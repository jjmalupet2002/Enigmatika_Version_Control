using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetective : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;

    public PlayerHealthBar playerhealthBar;


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
            TakeDamage(20);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        playerhealthBar.SetHealth(currentHealth);
    }
}
