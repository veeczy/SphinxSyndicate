using UnityEngine;

public class SpiderAI : EnemyAI
{
    [Header("Spider Settings")]
    public GameObject webProjectilePrefab;  // Assign in Inspector
    public float shootCooldown = 2f;        // Delay between shots
    private float nextShootTime = 0f;

    protected override void Update()
    {
        if (player == null) return;

        // Rotate to face the player
        Vector2 direction = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Shoot automatically on cooldown
        if (Time.time >= nextShootTime)
        {
            ShootWeb(direction);
            nextShootTime = Time.time + shootCooldown;
        }
    }

    void ShootWeb(Vector2 direction)
    {
        if (webProjectilePrefab != null)
        {
            GameObject web = Instantiate(webProjectilePrefab, transform.position, transform.rotation);
            Rigidbody2D rb = web.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Launch the projectile toward the player
                rb.AddForce(direction * 10f, ForceMode2D.Impulse);
            }
        }
    }
}