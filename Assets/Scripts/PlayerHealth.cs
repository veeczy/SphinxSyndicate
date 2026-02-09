using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth = 3;

    public bool bypassPlayerPrefs;//Allow bypass PlayerPrefs
    public bool invincible = false; //immune to damage
    public GameObject Arnold;
    PlayerMovement playerMovement;

    void Start()
    {
        if(!bypassPlayerPrefs)
        {
            currentHealth = PlayerPrefs.GetInt("health"); //at start health is set to value, it cannot be set to max health here or it will overwrite health on 'start' of each new scene
        }
        if(Arnold == null)  
            Arnold = GameObject.Find("Player");
        playerMovement = Arnold.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement.chargeDodgeStart) { invincible = true; } //if charge dodge then invincible
        if (playerMovement.isDodging) { invincible = true; } //if dodge then invincible
        if (!playerMovement.isDodging && !playerMovement.chargeDodgeStart) {invincible = false; } //if neither dodge then not invincible

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !invincible)
        {
            TakeDamage(1);
        }
        else if (collision.gameObject.CompareTag("Bullet") && !invincible)
        {
            TakeDamage(collision.gameObject.GetComponent<BulletId>().dmg);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        PlayerPrefs.SetInt("health", currentHealth); //update health in playerprefs when damage is taken
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            GoToLoseScene();
        }
    }

    public int GetCurrentHealth()
    {
        currentHealth = PlayerPrefs.GetInt("health"); //set health to whatever is saved in player prefs
        return currentHealth; //return updated health value when called
    }

    void GoToLoseScene()
    {
        SceneManager.LoadScene("LoseScene"); // Replace with your Lose scene name
    }
}
