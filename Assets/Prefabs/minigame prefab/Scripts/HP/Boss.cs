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
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            TakeDamage(20);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        bosshealthBar.SetHealth(currentHealth);
    }
}
