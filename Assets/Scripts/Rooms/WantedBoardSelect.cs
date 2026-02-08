using UnityEngine;
using UnityEngine.SceneManagement;

public class WantedBoardSceneSelect : MonoBehaviour
{
    public string tavernSceneName = "Tavern"; // set to your tavern scene name

    public void PickDesert()
    {
        LevelManager.instance.currentArea = LevelManager.AreaType.Desert;
        SceneManager.LoadScene(tavernSceneName);
    }

    public void PickCity()
    {
        LevelManager.instance.currentArea = LevelManager.AreaType.City;
        SceneManager.LoadScene(tavernSceneName);
    }

    public void PickSwamp()
    {
        LevelManager.instance.currentArea = LevelManager.AreaType.Swamp;
        SceneManager.LoadScene(tavernSceneName);
    }
}


