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
        armed = false;
        trapWall.SetActive(false);
        sceneTrigger.SetActive(false);
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
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            armed = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player") && armed)
        {
            trapWall.SetActive(true);
        }
    }
}
