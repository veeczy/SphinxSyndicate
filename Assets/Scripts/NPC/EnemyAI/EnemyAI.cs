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

    [Header("Aggro")]
    public float aggroRange = 6f; // player must enter this range before enemy activates
    protected bool hasAggro = false;

    [Header("Audio")]
    public AudioClip hurtSound;
    public float hurtVolume = 1f;

    private AudioSource audioSource;

    // push zone
    private bool inPushZone = false;
    private Vector2 pushDir = Vector2.zero;
    private float pushStrength = 1f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (healthBar)
            hbScale = healthBar.transform.localScale;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    protected virtual void Update()
    {
        // stay idle until player enters aggro range
        if (!CheckAggro()) return;

        HandleMovement();
        CheckHealth();
    }

    // check if player has entered aggro range
    protected bool CheckAggro()
    {
        if (player == null) return false;

        if (!hasAggro)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= aggroRange)
                hasAggro = true;
        }

        return hasAggro;
    }

    protected void HandleMovement()
    {
        if (player == null) return;

        Vector2 direction;

        // If inside a push zone, move exactly that way (overrides chasing)
        if (inPushZone && pushDir != Vector2.zero)
            direction = pushDir.normalized;
        else
            direction = ((Vector2)player.position - (Vector2)transform.position).normalized;

        float speed = inPushZone ? moveSpeed * pushStrength : moveSpeed;
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        // Flip left/right only
        if (direction.x > 0) sr.flipX = false;
        else if (direction.x < 0) sr.flipX = true;
    }

    protected void CheckHealth()
    {
        if (health <= 0)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // pushZone enter
        PushZone zone = col.GetComponent<PushZone>();
        if (zone != null)
        {
            inPushZone = true;
            pushDir = zone.pushDirection;
            pushStrength = Mathf.Max(0.01f, zone.strength);
            return; // zone only
        }

        // Player damage
        if (col.CompareTag("Player"))
        {
            PlayerHealth playerHealth = col.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }

        // Bullet damage
        BulletId bullet = col.GetComponent<BulletId>();
        if (bullet != null)
        {
            health -= bullet.dmg;

            if (healthBar)
                healthBar.transform.localScale =
                    new Vector2(hbScale.x * ((float)health / (float)maxHealth), hbScale.y);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // keep updating while inside zone (works even if multiple zones overlap)
        PushZone zone = col.GetComponent<PushZone>();
        if (zone != null)
        {
            inPushZone = true;
            pushDir = zone.pushDirection;
            pushStrength = Mathf.Max(0.01f, zone.strength);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        PushZone zone = col.GetComponent<PushZone>();
        if (zone != null)
        {
            inPushZone = false;
            pushDir = Vector2.zero;
            pushStrength = 1f;
        }
    }

    // draws a red circle around the enemy which show aggro range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
