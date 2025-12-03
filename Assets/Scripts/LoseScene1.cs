using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseSceneButtons : MonoBehaviour
{
    // Call this on Main Menu button
    public void GoToMainMenu()
    {
        Debug.Log("[LoseScene] Main Menu button pressed.");
        SceneManager.LoadScene("MainMenu");
    }

    // Call this on Quit Game button
    public void QuitGame()
    {
        Debug.Log("[LoseScene] Quit Game button pressed.");

#if UNITY_EDITOR
        Debug.Log("[LoseScene] Application.Quit() would run in build.");
#else
        Application.Quit();
#endif
    }
}