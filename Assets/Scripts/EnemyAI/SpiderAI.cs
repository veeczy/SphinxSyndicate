using UnityEngine;
using System.Collections;

public class SpiderAI : EnemyAI
{
    [Header("Lunge Settings")]
    public float detectionRadius = 4f;
    public float lungeForce = 8f;
    public float restTime = 2f;

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
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Activate when player enters radius
        if (!hasActivated && distance <= detectionRadius)
        {
            hasActivated = true;
            StartCoroutine(LungeLoop());
        }

        // Flip sprite
        Vector2 direction = (player.position - transform.position).normalized;
        if (direction.x > 0) sr.flipX = false;
        else if (direction.x < 0) sr.flipX = true;

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

        anim.SetTrigger("Lunge"); // Use trigger instead of bool

        yield return new WaitForSeconds(0.15f); // windup timing

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction * lungeForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.3f); // movement time

        rb.linearVelocity = Vector2.zero;

        isResting = false;
    }
}