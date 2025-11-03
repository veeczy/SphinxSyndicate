using UnityEngine;

public class VultureAI : EnemyAI
{
    [Header("Vulture Settings")]
    public float safeDistance = 6f;         // How far it tries to stay from the player
    public float shootInterval = 2f;        // How often it shoots
    public GameObject projectilePrefab;     // Assign in Inspector

    private float nextShootTime = 0f;

    protected override void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Face the player
        Vector2 direction = (player.position - transform.position).normalized;
        transform.right = direction;

        // Move away if too close, otherwise hover roughly in place
        if (distance < safeDistance * 0.8f)
        {
            Vector2 retreatDir = (transform.position - player.position).normalized;
            transform.position += (Vector3)retreatDir * moveSpeed * Time.deltaTime;
        }
        else if (distance > safeDistance * 1.2f)
        {
            // Move slightly toward player if too far away
            Vector2 approachDir = (player.position - transform.position).normalized;
            transform.position += (Vector3)approachDir * moveSpeed * Time.deltaTime;
        }

        // Shooting logic
        if (Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootInterval;
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null)
        {
            GameObject bullet = Instantiate(projectilePrefab, transform.position, transform.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Fire toward the player
                Vector2 shootDir = (player.position - transform.position).normalized;
                rb.AddForce(shootDir * 8f, ForceMode2D.Impulse);
            }
        }
    }
}
