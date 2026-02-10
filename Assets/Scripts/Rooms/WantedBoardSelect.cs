using UnityEngine;
using UnityEngine.SceneManagement;

public class WantedBoardSceneSelect : MonoBehaviour
{
    [Header("Scene to return to after picking")]
    public string tavernSceneName = "Tavern Upstairs";
    public GameObject button1; //button for desert level
    public GameObject button2; //button for city level
    public GameObject button3; //button for swamp level
    public int desertBoss;
    public int cityBoss;
    public int swampBoss;

    void Start()
    {
        desertBoss = PlayerPrefs.GetInt("desertBoss");
        cityBoss = PlayerPrefs.GetInt("cityBoss");
        swampBoss = PlayerPrefs.GetInt("swampBoss");

        if(desertBoss == 1) { button1.SetActive(false); }
        if(cityBoss == 1) { button2.SetActive(false); }
        if(swampBoss == 1) { button3.SetActive(false); }
    }


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



