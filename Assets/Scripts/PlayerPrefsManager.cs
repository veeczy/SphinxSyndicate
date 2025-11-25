using UnityEngine;

public class PlayerPrefsManager : MonoBehaviour
{
    //variables for updating player prefs at start
    public int playerHealth;
    public int playerMaxHealth = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerPrefs.SetInt("health", playerMaxHealth); //at start player prefs memory of health is reset to max health
    }

    // Update is called once per frame
    void Update()
    {
        playerHealth = PlayerPrefs.GetInt("health");
    }
}
