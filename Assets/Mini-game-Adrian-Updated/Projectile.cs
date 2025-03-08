using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 20; // The amount of damage the projectile deals

    private void OnCollisionEnter(Collision collision)
    {

        PlayerDetective player = collision.collider.GetComponent<PlayerDetective>();
        if (player != null)
        {
            Debug.Log("Projectile hit the player!");
            player.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}