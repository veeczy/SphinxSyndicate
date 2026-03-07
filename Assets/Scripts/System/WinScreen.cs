using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSceneButtons : MonoBehaviour
{
    // Call this on Main Menu button
    public void GoToMainMenu()
    {
        Debug.Log("[WinScene] Main Menu button pressed.");
        SceneManager.LoadScene("MainMenu");
    }

    // Call this on Quit Game button
    public void QuitGame()
    {
        Debug.Log("[WinScene] Quit Game button pressed.");

#if UNITY_EDITOR
        Debug.Log("[WinScene] Application.Quit() would run in build.");
#else
        Application.Quit();
#endif
    }
}