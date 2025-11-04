using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class Obstacle : MonoBehaviour
{
    public bool contactDamage;
    public int dmg = 1;
    public bool destructible;
    public int obstacleHealth;
    public Sprite destroyedSprite;
    public AudioSource soundSource;
    public AudioClip contactSound;
    public AudioClip destroySound;
    private bool destroyed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!destroyed && obstacleHealth <= 0)
        {
            destroyed = true;
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (contactDamage && col.CompareTag("Player") && col.GetComponent<PlayerHealth>())
        {
            col.GetComponent<PlayerHealth>().TakeDamage(dmg);//DO DAMAGE TO PLAYER ON CONTACT
            if (contactSound)
                soundSource.PlayOneShot(contactSound);
        }
        else if (destructible && col.GetComponent<BulletId>())//TAKE BULLET DAMAGE
        {
            obstacleHealth -= col.GetComponent<BulletId>().dmg;
            if (destroySound)
                soundSource.PlayOneShot(destroySound);
            if (destroyed)
            {
                //GetComponent<SpriteRenderer>().sprite = destroyedSprite;//Show new sprite for when object is destroyed
                //GetComponent<BoxCollider2D>().enabled = false;//Disable collision when destroyed
            }
        }
    }
}
