using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    // Reference to the PlayerHealth script on the Player object
    public PlayerHealth playerHealth;

    // Array of Image components representing hearts on the UI
    public Image[] hearts;

    // Sprites for full and empty hearts
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Update()
    {
        // Safety check: make sure playerHealth is assigned
        if (playerHealth == null)
            return;

        // Loop through all hearts and update the image based on current health
        for (int i = 0; i < hearts.Length; i++)
        {
            // If this heart index is below current health, show full heart
            if (i < playerHealth.GetCurrentHealth())
                hearts[i].sprite = fullHeart;
            // Otherwise show empty heart
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
