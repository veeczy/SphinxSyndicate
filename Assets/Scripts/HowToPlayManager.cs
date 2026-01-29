using UnityEngine;
using UnityEngine.UI;

public class HowToPlayManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject Buttons;          // Parent of Retry / HowToPlay / Quit
    public GameObject HowToPlayPanel;   // Panel shown when HowToPlay is clicked
    public Button HowToPlayButton;      // The HowToPlay button inside Buttons
    public Button BackButton;           // The BackToPause button inside HowToPlayPanel

    private void Start()
    {
        // Ensure panel is hidden at start
        if (HowToPlayPanel != null)
            HowToPlayPanel.SetActive(false);

        // Wire button clicks
        if (HowToPlayButton != null)
            HowToPlayButton.onClick.AddListener(ShowHowToPlayPanel);

        if (BackButton != null)
            BackButton.onClick.AddListener(HideHowToPlayPanel);
    }

    // Show the How To Play panel and hide main buttons
    public void ShowHowToPlayPanel()
    {
        if (Buttons != null) Buttons.SetActive(false);
        if (HowToPlayPanel != null) HowToPlayPanel.SetActive(true);
    }

    // Hide the How To Play panel and show main buttons
    public void HideHowToPlayPanel()
    {
        if (HowToPlayPanel != null) HowToPlayPanel.SetActive(false);
        if (Buttons != null) Buttons.SetActive(true);
    }
}
