using UnityEngine;

public class SpiderAI : EnemyAI
{
    [Header("Spider Settings")]
    public GameObject webProjectilePrefab;
    public float shootCooldown = 2f;
    private float nextShootTime = 0f;

    private SpriteRenderer sr;

    // NEW
    private Animator anim;
    private bool isAttacking = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();   // NEW
    }

    protected override void Update()
    {
        if (player == null) return;

        // Direction to player
        Vector2 direction = (player.position - transform.position).normalized;

        // Flip left/right only
        if (direction.x > 0)
            sr.flipX = false;
        else if (direction.x < 0)
            sr.flipX = true;

        // Shooting logic with animation
        if (Time.time >= nextShootTime && !isAttacking)
        {
            StartCoroutine(ShootAnim(direction));   // NEW
            nextShootTime = Time.time + shootCooldown;
        }

        // Make sure idle plays when not attacking
        if (!isAttacking)
            anim.SetBool("isAttacking", false);   // NEW

        CheckHealth();
    }

    // NEW — attack animation coroutine
    private System.Collections.IEnumerator ShootAnim(Vector2 direction)
    {
        isAttacking = true;
        anim.SetBool("isAttacking", true);

        // Small delay to sync with animation timing
        yield return new WaitForSeconds(0.2f);

        ShootWeb(direction);

        // Allow animation to finish before returning to idle
        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
        anim.SetBool("isAttacking", false);
    }

    void ShootWeb(Vector2 direction)
    {
        if (webProjectilePrefab != null)
        {
            GameObject web = Instantiate(webProjectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D rb = web.GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.AddForce(direction * 10f, ForceMode2D.Impulse);
        }
    }
}
