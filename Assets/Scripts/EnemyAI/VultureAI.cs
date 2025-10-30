using UnityEngine;

public class VultureAI : EnemyAI
{
    public GameObject projectilePrefab;   // Assign in Inspector
    public float shootInterval = 2f;
    public float safeDistance = 5f;
    private float nextShotTime;

    protected override void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Move away if player too close
        if (distance < safeDistance)
        {
            Vector2 retreat = (transform.position - player.position).normalized;
            transform.position += (Vector3)retreat * moveSpeed * Time.deltaTime;
        }

        // Face the player
        Vector2 direction = (player.position - transform.position).normalized;
        transform.right = direction;

        // Shoot at intervals
        if (Time.time >= nextShotTime)
        {
            Shoot();
            nextShotTime = Time.time + shootInterval;
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null)
        {
            GameObject bullet = Instantiate(projectilePrefab, transform.position, transform.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.AddForce(transform.right * 8f, ForceMode2D.Impulse);
        }
    }
}
