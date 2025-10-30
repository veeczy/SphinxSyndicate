using UnityEngine;

public class SpiderAI : EnemyAI
{
    public GameObject webProjectilePrefab;  // Assign in Inspector
    public float shootCooldown = 2f;
    private float nextShootTime;

    protected override void Update()
    {
        if (player == null) return;

        // Check if the player is in line of sight
        RaycastHit2D hit = Physics2D.Raycast(transform.position, player.position - transform.position);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            // If line of sight, shoot webs on cooldown
            if (Time.time >= nextShootTime)
            {
                ShootWeb();
                nextShootTime = Time.time + shootCooldown;
            }
        }
    }

    void ShootWeb()
    {
        if (webProjectilePrefab != null)
        {
            GameObject web = Instantiate(webProjectilePrefab, transform.position, transform.rotation);
            Rigidbody2D rb = web.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.AddForce((player.position - transform.position).normalized * 10f, ForceMode2D.Impulse);
        }
    }
}