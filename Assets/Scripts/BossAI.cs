using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public SpriteRenderer bossSprite;
    public Animator bossAnimator;
    protected Transform player;
    public int damage = 1;

    [Header("Health")]
    public Slider healthUI;
    public int health;
    public int maxHealth = 150;

    [Header("Movement / Combat")]
    public float minDistance = 0.0f;
    public float stalkMaxDistance = 10.0f;
    public float stalkMinDistance = 8.0f;
    public bool stalkMode;
    public float attackRange;
    public float attackTime = 5;
    public float attackCooldownTime;
    public bool attackCooldown;
    public int contactDamage = 1;
    public bool isAttacking;

    [Header("Phase 2")]
    public bool phase2 = false;

    [Header("Sheep Attack")]
    public GameObject sheepPrefab;
    public bool sheepAttacking;
    public float sheepVelocity;
    public float sheepDelay;
    public int sheepCounter = 0;

    public Vector2 direction;
    private float distance;
    public float stalkTimer;

    [Header("Boss Progress Tracking")]
    public int bossLevel; // 0 = desert, 1 = city, 2 = swamp
    private bool hasDied = false;

    [Header("DEBUG")]
    public KeyCode debugDamageKey = KeyCode.Alpha8;
    public int debugDamageAmount = 50;

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

        if (distance > minDistance && !isAttacking && !stalkMode && !attackCooldown)
        {
            StartCoroutine("closeAttack");
        }
        else if (distance > minDistance && isAttacking && !attackCooldown)
        {
            bossAnimator.SetBool("isWalking", true);
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
        else if (distance <= stalkMaxDistance && !isAttacking && !sheepAttacking && sheepCounter < 5)
        {
            bossAnimator.SetBool("isWalking", false);
            StartCoroutine("sheepAttack");
            StartCoroutine("stalk");
        }

        if (health <= maxHealth / 2 && !phase2)
        {
            phase2 = true;
            attackTime *= 2f;
            attackCooldownTime /= 2f;
            sheepDelay /= 2f;
            sheepVelocity *= 2f;
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

    IEnumerator closeAttack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
        attackCooldown = true;
        yield return new WaitForSeconds(attackCooldownTime + Random.Range(-2.5f, 2.5f));
        attackCooldown = false;
    }

    IEnumerator sheepAttack()
    {
        sheepAttacking = true;
        Rigidbody2D sheep = Instantiate(sheepPrefab, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
        sheep.AddForce(direction * sheepVelocity, ForceMode2D.Impulse);
        yield return new WaitForSeconds(sheepDelay);
        sheepCounter++;
        sheepAttacking = false;
    }

    IEnumerator stalk()
    {
        stalkMode = true;
        sheepCounter = 0;
        yield return new WaitForSeconds(stalkTimer);
        stalkMode = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bullet"))
        {
            health -= col.GetComponent<BulletId>().dmg;
            healthUI.value = health;
        }
        else if (col.CompareTag("Player"))
        {
            player.GetComponent<PlayerHealth>().currentHealth -= contactDamage;
        }
    }
}
