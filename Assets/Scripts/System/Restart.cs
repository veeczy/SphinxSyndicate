using UnityEngine;
using UnityEngine.SceneManagement; 

public class Restart : MonoBehaviour
{
    void Update()
    {
        // Check every frame if the player pressed the R key
        if (Input.GetButtonDown("Restart"))
        {
            RestartLevel();
        }
    }

    // This method reloads the current active scene
    void RestartLevel()
    {
        // Get the currently active scene by its build index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reset Health on restart
        PlayerPrefs.SetInt("health", 3); //at restart player prefs memory of health is reset to max health

        // Reload the scene using its index
        SceneManager.LoadScene(currentSceneIndex);

        // Optional: Log to console for debugging
        Debug.Log("Level restarted.");
    }
}
