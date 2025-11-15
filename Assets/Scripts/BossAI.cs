using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossAI : MonoBehaviour
{
    public float moveSpeed = 3f;        // How fast the enemy moves
    protected Transform player;         // Reference to the player's position (made protected so child scripts can use it)
    public int damage = 1;              // How much damage the enemy does
    public Slider healthUI;
    public int health = 3;
    public float minDistance = 0.0f;
    public float stalkMaxDistance = 10.0f;
    public float stalkMinDistance = 8.0f;
    public bool stalkMode;
    public float attackRange;
    public float attackCooldown;
    public bool isAttacking;
    public float nextAttackTime;
    public bool phase2 = false;
    public GameObject sheepPrefab;
    public bool sheepAttacking;
    public float sheepDelay;
    public int sheepCounter = 0;
    private Vector2 direction;
    private float distance;
    private float stalkTimer;

    void Start()
    {
        // Find the player by tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Measure distance to player
        distance = Vector2.Distance(transform.position, player.position);
        direction = (player.position - transform.position).normalized;
        // If outside attack range, move toward player
        if (distance > attackRange && !isAttacking && distance >= stalkMinDistance)
        {
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
        else if(stalkMode && distance <= stalkMaxDistance &&!sheepAttacking && sheepCounter < 5)
        {
            transform.position -= (Vector3)direction * moveSpeed * Time.deltaTime;
            StartCoroutine("sheepAttack");
        }
        else if (distance <= attackRange)//If distance from enemy is less than attackRange, Attack()
        {
            // Stop moving and face the player
            transform.right = direction;

            // Attack if cooldown is ready
            if (Time.time >= nextAttackTime && !isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
        if (health <= 0 && phase2)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        // Optional: placeholder for attack animation delay
        yield return new WaitForSeconds(0.3f);
        // Deal damage once in range
        PlayerHealth ph = player.GetComponent<PlayerHealth>();
        if (ph != null)
            ph.TakeDamage(damage);
        // Set cooldown
        nextAttackTime = Time.time + attackCooldown;
        isAttacking = false;
    }
    IEnumerator sheepAttack()
    {
        sheepAttacking = true;
        Instantiate(sheepPrefab, transform.position, new Quaternion(direction.x, direction.y, 0, 1));
        yield return new WaitForSeconds(sheepDelay);
        sheepCounter++;
        sheepAttacking = false;
    }
    IEnumerator stalk()
    {
        stalkMode = true;
        yield return new WaitForSeconds(stalkTimer);
        stalkMode = false;
    }
}
