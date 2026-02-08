using UnityEngine;
using UnityEngine.SceneManagement;

public class WantedBoardSceneSelect : MonoBehaviour
{
    [Header("Scene to return to after picking")]
    public string tavernSceneName = "Tavern Upstairs";

    public void PickDesert()
    {
        if (LevelManager.instance == null) return;
        LevelManager.instance.currentArea = LevelManager.AreaType.Desert;
        SceneManager.LoadScene(tavernSceneName);
    }

    public void PickCity()
    {
        if (LevelManager.instance == null) return;
        LevelManager.instance.currentArea = LevelManager.AreaType.City;
        SceneManager.LoadScene(tavernSceneName);
    }

    public void PickSwamp()
    {
        if (LevelManager.instance == null) return;
        LevelManager.instance.currentArea = LevelManager.AreaType.Swamp;
        SceneManager.LoadScene(tavernSceneName);
    }
}



