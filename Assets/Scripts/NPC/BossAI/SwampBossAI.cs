using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SwampBossAI : MonoBehaviour
{
    
    public float moveSpeed = 3f;
    public SpriteRenderer bossSprite;
    public Animator bossAnimator;
    protected Transform player;
    public Rigidbody2D rb;
    public int damage = 1;

    [Header("Health")]
    public Slider healthUI;
    public int health;
    public int maxHealth = 150;

    [Header("Movement / Combat")]
    public float minDistance = 0.0f;
    public float attackRange;
    public float attackTime = 5;
    public float meleeCooldownTime;
    public bool meleeCooldown;
    public bool onLand = true;
    public bool isGrounded = true;
    public bool meleeMode = true;
    public bool isContacting;
    public Transform[] waterJumpPos;//Where the boss can jump into the water
    public Transform[] landJumpPos;//Where the boss can jump back onto land

    [Header("Phase 2")]
    public bool phase2 = false;

    [Header("Projectile Attack")]
    public GameObject projectilePrefab;
    public bool waterAttacking;
    public float projectileVelocity;
    public float projectileDelay;
    public int projectileCounter = 0;

    public Vector2 direction;
    private float distance;
    public float jumpTimer;
    public bool isDamaging;

    [Header("Boss Progress Tracking")]
    public int bossLevel; // 0 = desert, 1 = city, 2 = swamp
    private bool hasDied = false;

    [Header("DEBUG")]
    public KeyCode debugDamageKey = KeyCode.Alpha8;
    public int debugDamageAmount = 50;
    [Header("Melee")]
    private bool isMelee = false;

    private float nextBurstTime = 0f;
    private float nextMeleeTime = 0f;

    private float meleeTimer = 0f;
    public float meleeDelay = 2f;
    public float burstDelay = 0.5f;
    public float meleeRange = 1.5f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        health = maxHealth;
        healthUI.maxValue = maxHealth;
        healthUI.value = maxHealth;
    }

    void FixedUpdate()
    {
        // DEBUG DAMAGE
        if (Input.GetKeyDown(debugDamageKey))
        {
            health -= debugDamageAmount;
            healthUI.value = health;
            Debug.Log("DEBUG: Boss took " + debugDamageAmount + " damage. Health = " + health);
        }

        // DEATH CHECK (runs once)
        if (health <= 0 && !hasDied)
        {
            hasDied = true;
            HandleBossDefeated();
            return;
        }

        distance = Vector2.Distance(transform.position, player.position);
        direction = (player.position - transform.position);
        bossSprite.flipX = direction.x < 0;

        if(!meleeMode && isGrounded && !meleeCooldown)//(!meleeMode && isGrounded && !meleeCooldown && isContacting && !isDamaging && !isMelee)
        {
            StartCoroutine("closeAttack");//START MELEE MODE
        }
        else if(meleeMode && isGrounded)
        {
            if(!onLand)
            {
                StartCoroutine("jump");//JUMP TO LAND
            }
            if(!isMelee)
                StartCoroutine("meleeAttack");//HANDLE MELEE DAMAGE
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);//MOVE TOWARDS PLAYER POSITION
        }
        else if(!meleeMode && isGrounded && meleeCooldown && !waterAttacking)
        {
            if(onLand)
            {
                StartCoroutine("jump");
            }
            StartCoroutine("waterAttack");//WATER ATTACK
        }

        if (health <= maxHealth / 2 && !phase2)
        {
            phase2 = true;
            attackTime *= 2f;
            meleeCooldownTime /= 2f;
            projectileDelay /= 2f;
            projectileVelocity *= 2f;
            moveSpeed *= 2f;
        }
    }

    void HandleBossDefeated()
    {
        string bossKey = "desertBoss";
        if (bossLevel == 1) bossKey = "cityBoss";
        if (bossLevel == 2) bossKey = "swampBoss";

        if (PlayerPrefs.GetInt(bossKey, 0) == 0)
        {
            PlayerPrefs.SetInt(bossKey, 1);
            PlayerPrefs.SetInt("bossCounter", PlayerPrefs.GetInt("bossCounter", 0) + 1);
        }

        PlayerPrefs.Save();
        Destroy(gameObject);
    }
   private IEnumerator meleeAttack()
    {
        isMelee = true;
        for(int i = 0; i < 3; i++)
        {
            float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= meleeRange && isContacting)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);
                yield return new WaitForSeconds(burstDelay);
        }
        }  
        yield return new WaitForSeconds(meleeDelay);
        isMelee = false;
    }
    IEnumerator closeAttack()
    {
        meleeMode = true;
        yield return new WaitForSeconds(attackTime);
        meleeMode = false;
        meleeCooldown = true;
        yield return new WaitForSeconds(meleeCooldownTime + Random.Range(-2.5f, 2.5f));
        meleeCooldown = false;
    }

    IEnumerator waterAttack()
    {
        waterAttacking = true;
        Rigidbody2D sheep = Instantiate(projectilePrefab, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
        sheep.AddForce(direction * projectileVelocity, ForceMode2D.Impulse);
        yield return new WaitForSeconds(projectileDelay);
        projectileCounter++;
        waterAttacking = false;
    }

    IEnumerator jump()
    { 
        isGrounded = false;
        projectileCounter = 0;
        if(onLand)
        {
            int waterIndex = Random.Range(0, waterJumpPos.Length);
            transform.position = waterJumpPos[waterIndex].position;//Relocate Boss to random preset position in the water
            onLand = false;
        }
        else
        {
            int landIndex = Random.Range(0, landJumpPos.Length);
            transform.position = landJumpPos[landIndex].position;//Relocate Boss to random preset position on land
            onLand = true;
        }
        yield return new WaitForSeconds(jumpTimer);
        isGrounded = true;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bullet") && col.GetComponent<BulletId>())
        {
            health -= col.GetComponent<BulletId>().dmg;
            healthUI.value = health;
        }
    }
    void OnTriggerStay2D(Collider2D col)
    {
            if (col.CompareTag("Player"))
            {
                isContacting = true;
            }
            else
            {
                isContacting = false;
            }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            isDamaging = false;
        }
    }
}
