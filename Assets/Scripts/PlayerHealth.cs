using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    // Total number of lives the player starts with
    public int maxHealth = 3;

    // Tracks current lives left
    public int currentHealth;

    void Start()
    {
        // Set current health to max at the start of the level
        currentHealth = maxHealth;
    }

    // Detects collisions with 2D colliders (like enemies)
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided object is tagged "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1); // Lose one life
        }
    }

    // Reduces health and restarts the level when health reaches zero
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Subtract damage
        Debug.Log("Player Health: " + currentHealth); // For testing

        if (currentHealth <= 0)
        {
            RestartLevel(); // Reset scene when out of lives
        }
    }

    // Returns the current health value (used by UI scripts)
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Reloads the current scene
    void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
