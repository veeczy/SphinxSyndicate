using UnityEngine;
using System.Collections;

public class HornedToadAI : EnemyAI
{
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        // --- MOVEMENT ---
        if (distance > attackRange && !isAttacking)
        {
            // Move toward player
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

            // NEW: walking animation on
            anim.SetBool("isWalking", true);

            // ensure attack animation stays off
            anim.SetBool("isAttacking", false);
        }
        else if (distance <= attackRange)
        {
            // NEW: stop walking when in attack range
            anim.SetBool("isWalking", false);

            if (Time.time >= nextAttackTime && !isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
        else
        {
            // NEW: idle when not moving
            anim.SetBool("isWalking", false);
        }

        CheckHealth();
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        // Stop walking when attacking
        anim.SetBool("isWalking", false); // NEW

        anim.SetBool("isAttacking", true);

        yield return new WaitForSeconds(0.3f);

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);
        }

        nextAttackTime = Time.time + attackCooldown;

        isAttacking = false;
        anim.SetBool("isAttacking", false);

        // NEW: after attack, idle (walking will turn on next frame if moving)
        anim.SetBool("isWalking", false);
    }
}

