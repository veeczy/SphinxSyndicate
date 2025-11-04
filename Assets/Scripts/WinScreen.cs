using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public string mainMenuScene = "MainMenu"; // Type your main menu scene name here
    public float delay = 5f;                  // Time before returning to menu

    void Start()
    {
        Invoke("LoadMainMenu", delay);
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}