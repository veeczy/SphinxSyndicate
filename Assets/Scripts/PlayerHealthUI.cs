using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;

    public Image[] hearts; // should be length 3

    // 5 states per heart
    public Sprite fullHeart;        // 4/4
    public Sprite threeQuarterHeart;// 3/4
    public Sprite halfHeart;        // 2/4
    public Sprite quarterHeart;     // 1/4
    public Sprite emptyHeart;       // 0/4


    void Update()
    {
        if (playerHealth == null) return;

        int hp = playerHealth.currentHealth; // now 0–12

        for (int i = 0; i < hearts.Length; i++)
        {
            // each heart represents 4 hp
            int heartHP = hp - (i * 4);
            heartHP = Mathf.Clamp(heartHP, 0, 4);

            if (heartHP == 4) hearts[i].sprite = fullHeart;
            else if (heartHP == 3) hearts[i].sprite = threeQuarterHeart;
            else if (heartHP == 2) hearts[i].sprite = halfHeart;
            else if (heartHP == 1) hearts[i].sprite = quarterHeart;
            else hearts[i].sprite = emptyHeart;
        }
    }
}
