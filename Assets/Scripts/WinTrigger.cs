using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    public MonoBehaviour target;   // drag BossAI or any enemy script component
    public string winSceneName;

    void Update()
    {
        if (target == null)
        {
            SceneManager.LoadScene(winSceneName);
        }
    }
}


