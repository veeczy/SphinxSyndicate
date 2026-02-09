using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string sceneToLoad;


    public void PlayGame()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene("Tavern Upstairs");
        }
        else
        {
            Debug.LogError("No scene name set in MainMenu script!");
        }
    }
    public void Settings()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene("Settings");
        }
        else
        {
            Debug.LogError("No scene name set in MainMenu script!");
        }
    }
    public void HowToPlay()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene("HowToPlay");
        }
        else
        {
            Debug.LogError("No scene name set in MainMenu script!");
        }
    }
    public void QuitGame()
    {
        Debug.Log("Game is quitting...");
        Application.Quit();
    }
}