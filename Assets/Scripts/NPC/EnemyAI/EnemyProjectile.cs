using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float lifeTime = 5f;     // Destroy after 5 seconds instead of 3
    public int damage = 1;          // Damage dealt to the player

    void Start()
    {
        // Destroy automatically after its lifetime expires
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Damage the player on contact
        if (col.CompareTag("Player"))
        {
            PlayerHealth playerHealth = col.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Projectile hit player, dealing damage: " + damage);
            }

            // Destroy projectile after hitting the player
            Destroy(gameObject);
        }
        else if (!col.CompareTag("Enemy"))
        {
            // Destroy on hitting anything that isn't an enemy
            Destroy(gameObject);
        }
    }
}

