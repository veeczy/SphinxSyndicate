
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomDoor : MonoBehaviour
{
    [Header("Use trigger to activate")]
    public bool onTrigger = true;

    [Header("If false, this door uses a fixed scene instead of random")]
    public bool useRandomRoom = true;

    [Header("Optional fixed scene name (if useRandomRoom = false)")]
    public string fixedSceneName;  // e.g. "Cave_Room"

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!onTrigger) return;
        if (!other.CompareTag("Player")) return;

        if (useRandomRoom)
        {
            if (LevelManager.instance == null)
            {
                Debug.LogError("LevelManager not found in scene!");
                return;
            }

            //int nextScene = LevelManager.instance.GetNextRandomRoom();
            //SceneManager.LoadScene(nextScene);
        }
        else
        {
            // Fixed path scene
            if (!string.IsNullOrEmpty(fixedSceneName))
                SceneManager.LoadScene(fixedSceneName);
            else
                Debug.LogWarning("RandomDoor has useRandomRoom = false but no fixedSceneName set.");
        }
    }
}

