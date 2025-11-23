using UnityEngine;

public class SpiderAI : EnemyAI
{
    [Header("Spider Settings")]
    public GameObject webProjectilePrefab;
    public float shootCooldown = 2f;
    private float nextShootTime = 0f;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();  // Cache the sprite renderer
    }

    protected override void Update()
    {
        if (player == null) return;

        // Direction to player
        Vector2 direction = (player.position - transform.position).normalized;

        // --- REMOVE ROTATION ---
        // transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // --- ADD FLIP ONLY (left/right) ---
        if (direction.x > 0)
            sr.flipX = false;
        else if (direction.x < 0)
            sr.flipX = true;

        // --- Shooting logic ---
        if (Time.time >= nextShootTime)
        {
            ShootWeb(direction);
            nextShootTime = Time.time + shootCooldown;
        }

        CheckHealth();
    }

    void ShootWeb(Vector2 direction)
    {
        if (webProjectilePrefab != null)
        {
            GameObject web = Instantiate(webProjectilePrefab, transform.position, Quaternion.identity);

            Rigidbody2D rb = web.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.AddForce(direction * 10f, ForceMode2D.Impulse);
            }
        }
    }
}
