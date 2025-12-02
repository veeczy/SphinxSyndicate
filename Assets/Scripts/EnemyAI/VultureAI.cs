using UnityEngine;

public class VultureAI : EnemyAI
{
    [Header("Vulture Settings")]
    public float safeDistance = 6f;
    public float shootInterval = 2f;
    public GameObject projectilePrefab;

    private float nextShootTime = 0f;
    private SpriteRenderer sr;

    // NEW
    private Animator anim;
    private bool isAttacking = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>(); // NEW
    }

    protected override void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        // Flip sprite only
        if (sr != null)
        {
            if (direction.x > 0) sr.flipX = false;
            else if (direction.x < 0) sr.flipX = true;
        }

        // Hover movement
        if (!isAttacking) // NEW → don’t slide during shoot anim
        {
            if (distance < safeDistance * 0.8f)
            {
                Vector2 retreatDir = (transform.position - player.position).normalized;
                transform.position += (Vector3)retreatDir * moveSpeed * Time.deltaTime;
            }
            else if (distance > safeDistance * 1.2f)
            {
                Vector2 approachDir = (player.position - transform.position).normalized;
                transform.position += (Vector3)approachDir * moveSpeed * Time.deltaTime;
            }
        }

        // Shooting logic
        if (Time.time >= nextShootTime && !isAttacking)
        {
            StartCoroutine(ShootAnim()); // NEW → use attack animation
            nextShootTime = Time.time + shootInterval;
        }

        // Always idle when not attacking
        if (!isAttacking)
            anim.SetBool("isAttacking", false); // NEW

        CheckHealth();
    }

    // NEW — replace shoot logic with shoot + animation coroutine
    private System.Collections.IEnumerator ShootAnim()
    {
        isAttacking = true;
        anim.SetBool("isAttacking", true);

        // Delay before bullet fires (sync with your attack anim)
        yield return new WaitForSeconds(0.2f);

        Shoot();

        // Cooldown to finish animation smoothly
        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
        anim.SetBool("isAttacking", false);
    }

    void Shoot()
    {
        if (projectilePrefab != null)
        {
            GameObject bullet = Instantiate(projectilePrefab, transform.position, transform.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 shootDir = (player.position - transform.position).normalized;
                rb.AddForce(shootDir * 8f, ForceMode2D.Impulse);
            }
        }
    }
}
