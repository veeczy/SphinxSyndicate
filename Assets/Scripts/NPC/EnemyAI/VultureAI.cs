using UnityEngine;

public class VultureAI : EnemyAI
{
    [Header("Vulture Settings")]
    public float safeDistance = 6f;
    public float attackRange = 10f; // max distance vulture will shoot
    public float shootInterval = 2f;
    public GameObject projectilePrefab;

    private float nextShootTime = 0f;
    private SpriteRenderer sr;

    private Animator anim;
    private bool isAttacking = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (!CheckAggro()) return;
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        // Flip sprite
        if (sr != null)
        {
            if (direction.x > 0) sr.flipX = false;
            else if (direction.x < 0) sr.flipX = true;
        }

        bool isMoving = false;

        // Hover movement (but no movement during shoot)
        if (!isAttacking)
        {
            if (distance < safeDistance * 0.8f)
            {
                Vector2 retreatDir = (transform.position - player.position).normalized;
                transform.position += (Vector3)retreatDir * moveSpeed * Time.deltaTime;
                isMoving = true;
            }
            else if (distance > safeDistance * 1.2f)
            {
                Vector2 approachDir = (player.position - transform.position).normalized;
                transform.position += (Vector3)approachDir * moveSpeed * Time.deltaTime;
                isMoving = true;
            }
        }

        anim.SetBool("isWalking", isMoving && !isAttacking);

        // Shooting logic (only if player is within attack range)
        if (distance <= attackRange && Time.time >= nextShootTime && !isAttacking)
        {
            StartCoroutine(ShootAnim());
            nextShootTime = Time.time + shootInterval;
        }

        if (!isAttacking)
            anim.SetBool("isAttacking", false);

        CheckHealth();
    }

    private System.Collections.IEnumerator ShootAnim()
    {
        isAttacking = true;

        anim.SetBool("isWalking", false);
        anim.SetBool("isAttacking", true);

        yield return new WaitForSeconds(0.2f);

        Shoot();

        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
        anim.SetBool("isAttacking", false);
        anim.SetBool("isWalking", false);
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
