using UnityEngine;
using System.Collections;

public class DogEnemyAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    protected Transform player;
    public Transform owner;//Automatically assigned by CityBossAI.cs
    public int damage = 1;
    public int health = 3;
    public int maxHealth = 3;
    public SpriteRenderer healthBar;
    private Vector2 hbScale;
    public bool isRetreating = false;
    public float retreatTimer;
    public float stunTimer = 5f;//Time which player is stunned after contact
    protected Rigidbody2D rb;
    public Rigidbody2D playerRb;
    protected SpriteRenderer sr;

    [Header("Audio")]
    public AudioClip hurtSound;
    public float hurtVolume = 1f;

    private AudioSource audioSource;
    public bool canRun = true;

    // PUSH ZONE
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
        canRun = true;
    }

    protected virtual void Update()
    {
        CheckHealth();
    }
    void FixedUpdate()
    {
        HandleMovement();
    }

    protected void HandleMovement()
    {
        if (player == null) return;

        Vector2 direction;

        // If inside a push zone, move exactly that way (overrides chasing)
        if (inPushZone && pushDir != Vector2.zero)
        {
            direction = pushDir.normalized;
        } 
        else if(isRetreating)
        {
            direction = ((Vector2)owner.position - (Vector2)transform.position).normalized;
        }
        else
        {
            direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        }
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);

        // Flip left/right only
        if (direction.x > 0) sr.flipX = false;
        else if (direction.x < 0) sr.flipX = true;
    }

    protected void CheckHealth()
    {
        if (health <= 0)
        {
            playerRb.simulated = true;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // PushZone enter
        PushZone zone = col.GetComponent<PushZone>();
        if (zone != null)
        {
            inPushZone = true;
            pushDir = zone.pushDirection;
            pushStrength = Mathf.Max(0.01f, zone.strength);
            return; // zone only
        }
        // Bullet damage
        BulletId bullet = col.GetComponent<BulletId>();
        if (bullet != null)
        {
            health -= bullet.dmg;
            if (healthBar)
                healthBar.transform.localScale = new Vector2(hbScale.x * ((float)health / (float)maxHealth), hbScale.y);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        // Keep updating while inside zone (works even if multiple zones overlap)
        PushZone zone = col.GetComponent<PushZone>();
        if (zone != null)
        {
            inPushZone = true;
            pushDir = zone.pushDirection;
            pushStrength = Mathf.Max(0.01f, zone.strength);
        }
        
        // Player damage
        if (col.CompareTag("Player") && canRun && !isRetreating)
        {
            playerRb = col.GetComponent<Rigidbody2D>();
            StartCoroutine("dogAttack");
        }
        if(col.CompareTag("Boss") && isRetreating)
        {
            if(canRun)
            {
                Destroy(gameObject);
                playerRb.simulated = true;
                owner.GetComponent<CityBossAI>().dogReleased = false;
            }
            else
            {
                healthBar.gameObject.SetActive(false);
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
            }
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
    IEnumerator dogAttack()
    {
        if (playerRb != null)
        {
            playerRb.simulated = false;
            isRetreating = true;
            canRun = false;
            yield return new WaitForSeconds(stunTimer);
            canRun = true;
            playerRb.simulated = true;
        }
        isRetreating = true;
        yield return new WaitForSeconds(retreatTimer);
        owner.GetComponent<CityBossAI>().dogReleased = false;
        playerRb.simulated = true;
        Destroy(gameObject);
    }
}