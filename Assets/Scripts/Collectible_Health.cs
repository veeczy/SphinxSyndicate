using UnityEngine;

public class Collectible_Health : MonoBehaviour
{
    public int healAmount = 1;
    private PlayerHealth healthScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player") && col.GetComponent<PlayerHealth>())
        {
            healthScript = col.GetComponent<PlayerHealth>();
            if (healthScript.currentHealth <= (healthScript.maxHealth - healAmount))
            {
                healthScript.currentHealth += healAmount;
                Destroy(gameObject);
            }
        }
    }
}
