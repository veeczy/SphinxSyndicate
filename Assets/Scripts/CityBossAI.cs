using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CityBossAI : MonoBehaviour
{
    public float moveSpeed = 3f;
    public SpriteRenderer bossSprite;
    public Rigidbody2D rb;
    public Animator bossAnimator;
    protected Transform player;
    public int damage = 1;

    [Header("Health")]
    public Slider healthUI;
    public int health;
    public int maxHealth = 150;

    [Header("Movement / Combat")]
    public float minDistance = 0.0f;
    public float attackRange;
    public Vector2 smiteTarget;
    public bool meleeCooldown;
    public int contactDamage = 1;
    public bool allowSmite = true;
    public bool spawnDog = true;
    public bool dogReleased = false;
    public bool meleeMode = true;

    [Header("Phases")]
    public int phase = 1;
    public int phase2Health = 100;
    public int phase3Health = 50;


    [Header("Smite Attack")]
    public GameObject smitePrefab;
    public int smiteCounter = 0;
    public int smiteCountMax = 5;
    public float smiteDelay = 3;
    public float smiteCooldownTime;
    public bool smiteCooldown;
    public bool smiteAttacking = false;


    [Header("Dog Attack")]
    public GameObject dogPrefab;
    public float dogTimer = 7.5f;
    public float dogCooldownTime = 10f;
    public bool dogAttacking = false;
    public bool dogCooldown = false;

    public Vector2 direction;
    private float distance;

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
        if(!smiteAttacking)//FREEZE BOSS WHEN SMITING
        {
            if(allowSmite)
                smiteTarget = player.position;//ONLY RECORD PLAYER POSITION WHEN NOT ABOUT TO SMITE
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);//MOVE TOWARD PLAYER
            if(!dogAttacking && !dogReleased && spawnDog && !dogCooldown)
            {
                StartCoroutine("dogAttack");//BEGIN DOG ATTACK
                if(allowSmite && !smiteCooldown)
                {
                    StartCoroutine("smiteAttack");//BEGIN SMITE ATTACK
                }
            }
        }
//START HANDLE BOSS PHASES
        if (health <= phase2Health && phase < 2)
        {
            phase = 2;
            moveSpeed *= 2f;
        }
        else if(health <= phase3Health * 0.33f && phase < 3)
        {
            phase = 3;
            spawnDog = true;
            allowSmite = true;
        }
//END HANDLE BOSS PHASES
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

    IEnumerator smiteAttack()
    {
        smiteAttacking = true;
        yield return new WaitForSeconds(smiteDelay);
        GameObject smiteZone = Instantiate(smitePrefab, smiteTarget, transform.rotation);//SPAWNS TRIGGER THAT DETECTS PLAYER AND DOES LARGE DAMAGE IF PLAYER IS WITHIN TRIGGER AFTER TIMER
        smiteCounter++;
        smiteAttacking = false;
        if(smiteCounter >= smiteCountMax)
        {
            smiteCooldown = true;
            yield return new WaitForSeconds(smiteCooldownTime + Random.Range(-2.5f, 2.5f));
            smiteCooldown = false;
            smiteCounter = 0;
        }
    }

    IEnumerator dogAttack()
    {
        dogReleased = true;
        GameObject dog = Instantiate(dogPrefab, transform.position, transform.rotation);
        DogEnemyAI dogAI = dog.GetComponent<DogEnemyAI>();
        dogAI.owner = this.transform;//Tell dog script what object sent out the dog
        dogCooldown = true;
        yield return new WaitForSeconds(dogCooldownTime);
        dogCooldown = false;
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
