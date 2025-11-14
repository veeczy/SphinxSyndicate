using UnityEngine;

public class HornedToadAI : EnemyAI
{
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;

    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    private Animator anim;   // NEW

    void Awake()
    {
        anim = GetComponent<Animator>();   // NEW
    }

    protected override void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Vector2 direction = (player.position - transform.position).normalized;

        // Movement when not attacking
        if (distance > attackRange && !isAttacking)
        {
            // Keep old movement
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

            // Ensure idle animation plays
            anim.SetBool("isAttacking", false);   // NEW
        }
        else if (distance <= attackRange)
        {
            // Start the attack if ready
            if (Time.time >= nextAttackTime && !isAttacking)
            {
                StartCoroutine(Attack());
            }
        }

        CheckHealth();
    }

    private System.Collections.IEnumerator Attack()
    {
        isAttacking = true;
        anim.SetBool("isAttacking", true);   // NEW → triggers HornedToad_Attack

        // Play attack for a short time before applying damage
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
        anim.SetBool("isAttacking", false);   // NEW → returns to HornedToad_Idle
    }
}
