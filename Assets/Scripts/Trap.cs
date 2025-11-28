using UnityEngine;

public class Trap : MonoBehaviour
{
    public bool armed;
    public GameObject trapWall;
    public GameObject sceneTrigger;
    public GameObject[] enemies;
    public int enemyCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        armed = true;
        // NEW: trap is now armed instantly when scene loads

        trapWall.SetActive(true);
        // NEW: trap wall closes immediately when scene loads

        // OLD (removed): trap started deactivated
        // trapWall.SetActive(false);

        sceneTrigger.SetActive(false);
        // unchanged
    }

    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = enemies.Length;

        if (enemyCount <= 0)
        {
            trapWall.SetActive(false);
            sceneTrigger.SetActive(true);
        }
    }

    /*
    REMOVED: No longer needed because trap arms instantly in Start()
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            armed = true;
        }
    }
    */

    /*
    REMOVED: Player leaving the trigger no longer controls trap activation
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player") && armed)
        {
            trapWall.SetActive(true);
        }
    }
    */
}

