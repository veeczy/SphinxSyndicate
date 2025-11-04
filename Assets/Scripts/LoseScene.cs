using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScene : MonoBehaviour
{
    public string mainMenuScene = "MainMenu";
    public float delay = 5f;

    void Start()
    {
        Invoke("LoadMainMenu", delay);
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}