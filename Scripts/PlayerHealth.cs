using UnityEngine;
using UnityEngine.SceneManagement; 

public class PlayerHealth : MonoBehaviour
{
    // Player starts with 3 lives
    public int maxHealth = 3;
    private int currentHealth;

    void Start()
    {
        // Set current health to max at start
        currentHealth = maxHealth;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if collided object is an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player Health: " + currentHealth);

        // Restart level when health runs out
        if (currentHealth <= 0)
        {
            RestartLevel();
        }
    }
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void RestartLevel()
    {
        // Reload the active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
