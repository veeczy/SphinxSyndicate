using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    public EnemyAI targetEnemy;  // Drag the enemy you must defeat
    public string winSceneName;  // Type the name of your win scene here

    void Update()
    {
        // When the target enemy is destroyed, load the win scene
        if (targetEnemy == null)
        {
            SceneManager.LoadScene(winSceneName);
        }
    }
}

