using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 20; // The amount of damage the projectile deals

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Projectile hit: {collision.collider.name}");

        PlayerDetective player = collision.collider.GetComponent<PlayerDetective>();
        if (player != null)
        {
            Debug.Log("Projectile hit the player!");
            player.TakeDamage(damage);
        }
        else
        {
            Debug.Log("Hit object does not have PlayerDetective.");
        }

        Destroy(gameObject);
    }
}