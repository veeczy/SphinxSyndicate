using UnityEngine;
using System.Collections;

public class RedCrayfishAI : EnemyAI
{
    [Header("Activation")]
    public float detectionRadius = 8f;
    private bool hasActivated = false;

    [Header("Ranges")]
    public float shootRange = 6f;
    public float meleeRange = 1.5f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public float projectileForce = 8f;

    public int burstCount = 3;
    public float timeBetweenShots = 0.2f;
    public float burstCooldown = 2f;

    [Header("Melee")]
    public float meleeHoldTime = 1f;
    public float meleeCooldown = 1.25f;

    [Header("Animation (optional)")]
    public Animator anim;
    public string shootTrigger = "Shoot";
    public string meleeTrigger = "Melee";
    public string walkingBool = "isWalking";

    private bool isFiring = false;
    private bool isMelee = false;

    private float nextBurstTime = 0f;
    private float nextMeleeTime = 0f;

    private float meleeTimer = 0f;

    void Awake()
    {
        if (anim == null) anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (player == null) return;

        // Activate when player enters radius (like SpiderAI)
        if (!hasActivated)
        {
            float activateDist = Vector2.Distance(transform.position, player.position);
            if (activateDist > detectionRadius)
            {
                // stay idle until activated
                SetWalkingAnim(false);
                return;
            }

            hasActivated = true;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        FacePlayer();

        // If firing or meleeing, do nothing else
        if (isFiring || isMelee)
        {
            SetWalkingAnim(false);
            CheckHealth();
            return;
        }

        // ----- MELEE -----
        if (distance <= meleeRange && Time.time >= nextMeleeTime)
        {
            meleeTimer += Time.deltaTime;

            if (meleeTimer >= meleeHoldTime)
            {
                StartCoroutine(MeleeAttack());
                meleeTimer = 0f;
                CheckHealth();
                return;
            }
        }
        else
        {
            meleeTimer = 0f;
        }

        // ----- SHOOT BURST -----
        if (distance <= shootRange && Time.time >= nextBurstTime)
        {
            StartCoroutine(ShootBurst());
            CheckHealth();
            return;
        }

        // ----- CHASE -----
        SetWalkingAnim(true);
        HandleMovement();

        CheckHealth();
    }

    private IEnumerator ShootBurst()
    {
        isFiring = true;
        SetWalkingAnim(false);

        for (int i = 0; i < burstCount; i++)
        {
            FacePlayer();

            if (anim != null && !string.IsNullOrEmpty(shootTrigger))
                anim.SetTrigger(shootTrigger);

            FireProjectile();

            yield return new WaitForSeconds(timeBetweenShots);
        }

        nextBurstTime = Time.time + burstCooldown;
        isFiring = false;
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null) return;

        GameObject proj = Instantiate(projectilePrefab, transform.position, transform.rotation);
        Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();

        if (projRb != null)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            projRb.AddForce(dir * projectileForce, ForceMode2D.Impulse);
        }
    }

    private IEnumerator MeleeAttack()
    {
        isMelee = true;
        SetWalkingAnim(false);

        FacePlayer();

        if (anim != null && !string.IsNullOrEmpty(meleeTrigger))
            anim.SetTrigger(meleeTrigger);

        yield return new WaitForSeconds(0.15f);

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= meleeRange)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);
        }

        nextMeleeTime = Time.time + meleeCooldown;
        isMelee = false;
    }

    private void FacePlayer()
    {
        if (player == null) return;

        Vector2 toPlayer = player.position - transform.position;

        if (toPlayer.x > 0) sr.flipX = false;
        else if (toPlayer.x < 0) sr.flipX = true;
    }

    private void SetWalkingAnim(bool walking)
    {
        if (anim == null) return;
        if (string.IsNullOrEmpty(walkingBool)) return;

        anim.SetBool(walkingBool, walking);
    }
}