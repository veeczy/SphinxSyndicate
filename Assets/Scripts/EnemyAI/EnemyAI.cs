using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    protected Transform player;
    public int damage = 1;
    public int health = 3;
    public int maxHealth = 3;
    public SpriteRenderer healthBar;
    private Vector2 hbScale;

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (healthBar)
            hbScale = healthBar.transform.localScale;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void Update()
    {
        HandleMovement();
        CheckHealth();
    }

    protected void HandleMovement()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;

        // Move forward
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        // ONLY flip left/right � NO rotation
        if (direction.x > 0)
            sr.flipX = false;
        else if (direction.x < 0)
            sr.flipX = true;
    }

    protected void CheckHealth()
    {
        if (health <= 0)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerHealth playerHealth = col.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }

        BulletId bullet = col.GetComponent<BulletId>();
        if (bullet != null)
        {
            health -= bullet.dmg;
            if (healthBar)
                healthBar.transform.localScale = new Vector2(hbScale.x * (health / maxHealth), hbScale.y);
        }
    }
}
