using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    //variables for updating player prefs at start
    public int playerHealth;
    public int playerMaxHealth = 3;

    //variables for updating progress in game
    public int bossCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPrefs.SetInt("health", playerMaxHealth); //at start player prefs memory of health is reset to max health

        PlayerPrefs.SetInt("bossCounter", 0); //reset amount of bosses beaten to 0
    }

    // Update is called once per frame
    void Update()
    {
        playerHealth = PlayerPrefs.GetInt("health");
        bossCounter = PlayerPrefs.GetInt("bossCounter");
    }
}
