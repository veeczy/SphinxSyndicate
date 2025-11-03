using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;        // How fast the enemy moves
    protected Transform player;         // Reference to the player's position (made protected so child scripts can use it)
    public int damage = 1;              // How much damage the enemy does
    public int health = 3;

    void Start()
    {
        // Find the player by tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Marked as virtual so child classes can override it
    protected virtual void Update()
    {
        // Move toward the player's position
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
            if (health <= 0)
            {
                Destroy(gameObject);// Kil the enemy
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Damage the player
        if (col.CompareTag("Player"))
        {
            PlayerHealth playerHealth = col.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }

        // Detect bullet hits
        if (col.GetComponent<BulletId>() != null)
        {
            health -= col.GetComponent<BulletId>().dmg;
            //Destroy(col.gameObject); // Destroy the bullet
        }
    }
}
