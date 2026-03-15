using UnityEngine;
using System.Collections;

public class SpiderAI : EnemyAI
{
    [Header("Lunge Settings")]
    public float lungeForce = 8f;
    public float restTime = 2f;
    public float damageRadius = 1.2f;

    private Rigidbody2D rb;
    private Animator anim;

    private bool hasActivated = false;
    private bool isResting = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        if (!CheckAggro()) return;
        if (player == null) return;

        if (!hasActivated)
        {
            hasActivated = true;
            StartCoroutine(LungeLoop());
        }

        Vector2 direction = (player.position - transform.position).normalized;

        if (direction.x > 0)
            sr.flipX = false;
        else if (direction.x < 0)
            sr.flipX = true;

        CheckHealth();
    }

    private IEnumerator LungeLoop()
    {
        while (true)
        {
            if (!isResting)
            {
                yield return StartCoroutine(Lunge());
                yield return new WaitForSeconds(restTime);
            }
        }
    }

    private IEnumerator Lunge()
    {
        isResting = true;

        // Play lunge animation
        anim.SetBool("isLunging", true);

        yield return new WaitForSeconds(0.15f);

        // Direction toward player
        Vector2 direction = (player.position - transform.position).normalized;

        // Apply lunge force
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * lungeForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.15f);

        // Damage player if close enough during lunge
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= damageRadius)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);
        }

        yield return new WaitForSeconds(0.15f);

        // Stop movement
        rb.linearVelocity = Vector2.zero;

        // Return to idle animation
        anim.SetBool("isLunging", false);

        isResting = false;
    }
}
