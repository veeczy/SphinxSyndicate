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
    public float attackTime = 5;
    public float attackCooldownTime;
    public bool attackCooldown;
    public int contactDamage = 1;
    public bool isAttacking;
    public float nextAttackTime;
    public bool phase2 = false;
    public GameObject sheepPrefab;
    public bool sheepAttacking;
    public float sheepVelocity;
    public float sheepDelay;
    public int sheepCounter = 0;
    private Vector2 direction;
    private float distance;
    public float stalkTimer;

    void Start()
    {
        // Find the player by tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Measure distance to player
        distance = Vector2.Distance(transform.position, player.position);
        direction = (player.position - transform.position);//Target direction
        healthUI.value = health;//Update health UI
        // If outside attack range, move toward player
        if (distance > minDistance && !isAttacking && !stalkMode && !attackCooldown)
        {
            StartCoroutine("closeAttack");//Follow player
        }
        else if (distance > minDistance && isAttacking && !attackCooldown)
        {
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
        else if (distance <= stalkMaxDistance && !isAttacking && !sheepAttacking && sheepCounter < 5)
        {
            transform.position -= (Vector3)direction * moveSpeed * Time.deltaTime;
            //transform.rotation = Quaternion.LookRotation(direction);
            StartCoroutine("sheepAttack");
            StartCoroutine("stalk");
        }
        if (health <= 0 && phase2)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator closeAttack()
    {
        isAttacking = true;
        Debug.Log("CLOSE ATTACKING!");
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
        attackCooldown = true;
        yield return new WaitForSeconds(attackCooldownTime + Random.Range(-2.5f, 2.5f));
        attackCooldown = false;
    }
    IEnumerator sheepAttack()
    {
        sheepAttacking = true;
        //float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        //Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
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
        }
        else if (col.CompareTag("Player"))
        {
            player.GetComponent<PlayerHealth>().currentHealth -= contactDamage;
        }
    }
}
