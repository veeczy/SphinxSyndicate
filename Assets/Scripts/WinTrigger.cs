using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    public enum EnemyTypes//Overkill, but here for possible expansion of variety in win scenerios.
    {
        Enemy, 
        Boss
    }
    public EnemyTypes enemyType;
    public EnemyAI targetEnemy;  // Drag the enemy you must defeat
    public BossAI targetBoss;
    public string winSceneName;  // Type the name of your win scene here

    void Update()
    {
        // When the target enemy is destroyed, load the win scene
        if(enemyType == EnemyTypes.Enemy)
        {
            if (targetEnemy == null && winSceneName != null)
            {
                SceneManager.LoadScene(winSceneName);
            }
            else if(targetBoss == null)
            {
                Debug.Log("Scene Name Invalid!");
            }
        }
        else
        {
            if (targetBoss == null && winSceneName != null)
            {
            SceneManager.LoadScene(winSceneName);
            }
            else if(targetBoss == null)
            {
                Debug.Log("Scene Name Invalid!");
            }
        }
    }
}

