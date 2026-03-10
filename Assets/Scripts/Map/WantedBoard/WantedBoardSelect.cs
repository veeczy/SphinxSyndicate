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

        if (desertBoss == 1) { button1.SetActive(false); } //if beat desert boss, disable poster to choose it

        if (cityBoss == 1) { button2.SetActive(false); } //if beat city boss, disable poster to choose it

        if (swampBoss == 1) { button3.SetActive(false); } //if beat swamp boss, disable poster to choose it

        if(desertBoss == 1 && cityBoss == 1 && swampBoss == 1)
        {
            AllClear();
        }
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

    public void AllClear()
    {
        //ui that offers option to reset everything needs to be put here
        //ui that indicates on wanted board that you beat everything needs to be set active, possibly a star
        SceneManager.LoadScene(tavernSceneName);
    }
}



