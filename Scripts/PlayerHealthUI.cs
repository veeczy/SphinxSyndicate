using UnityEngine;
using UnityEngine.UI; 

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;    // Reference to the PlayerHealth script
    public Image[] hearts;               // Array to hold the 3 heart images
    public Sprite fullHeart;             // Heart when filled
    public Sprite emptyHeart;            // Heart when lost

    void Update()
    {
        // Loop through hearts and update based on current health
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < playerHealth.GetCurrentHealth())
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
