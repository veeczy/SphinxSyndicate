using UnityEngine;

public class HornedToadAI : EnemyAI
{
    public float attackRange = 2f;   // How close the player must be
    public float attackCooldown = 1f;
    private float nextAttackTime = 0f;

    protected override void Update()
    {
        if (player == null) return;

        // Move toward player
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        // If close enough, try to attack
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null)
            ph.TakeDamage(damage);
    }
}
