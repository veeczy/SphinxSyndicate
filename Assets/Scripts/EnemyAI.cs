using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;        // How fast the enemy moves
    private Transform player;           // Reference to the player's position
    public int damage = 1;              // How much damage the enemy does

    void Start()
    {
        // Find the player by tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Move toward the player's position
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Damage the player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }

        // Works with BulletId system instead of relying only on "Bullet" tag
        if (collision.gameObject.GetComponent<BulletId>() != null)
        {
            Destroy(gameObject);              // Enemy dies
            Destroy(collision.gameObject);    // Bullet disappears
        }
    }
}
