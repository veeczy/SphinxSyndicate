using UnityEngine;

public class HornedToadAI : EnemyAI
{
    public float attackRange = 2f;       // Distance to stop and attack
    public float attackCooldown = 1.5f;  // Time between attacks
    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    protected override void Update()
    {
        if (player == null) return;

        // Measure distance to player
        float distance = Vector2.Distance(transform.position, player.position);

        // If outside attack range, move toward player
        if (distance > attackRange && !isAttacking)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
        else if (distance <= attackRange)
        {
            // Stop moving and face the player
            Vector2 direction = (player.position - transform.position).normalized;
            transform.right = direction;

            // Attack if cooldown is ready
            if (Time.time >= nextAttackTime && !isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
    }

    private System.Collections.IEnumerator Attack()
    {
        isAttacking = true;

        // Optional: placeholder for attack animation delay
        yield return new WaitForSeconds(0.3f);

        // Deal damage once in range
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);
        }

        // Set cooldown
        nextAttackTime = Time.time + attackCooldown;
        isAttacking = false;
    }
}
