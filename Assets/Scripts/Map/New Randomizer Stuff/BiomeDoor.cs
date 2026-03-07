using UnityEngine;
using UnityEngine.SceneManagement;

public class BiomeDoor : MonoBehaviour
{
    public bool onTrigger = true;

    [Header("Optional: force a biome (for testing / until wanted board exists)")]
    public bool forceBiome = true;
    public LevelManager.AreaType forcedBiome = LevelManager.AreaType.Desert;

    [Header("Start Zone Scene Names")]
    public string desertStartScene = "DesertStart";
    public string cityStartScene = "CityStart";
    public string swampStartScene = "SwampStart";

    [Header("Boss Progress Check")]
    public int desertBoss;
    public int swampBoss;
    public int cityBoss;

    private void Start()
    {
        if (LevelManager.instance == null)
        {
            Debug.LogError("LevelManager not found!");
            return;
        }

        desertBoss = PlayerPrefs.GetInt("desertBoss");
        cityBoss = PlayerPrefs.GetInt("cityBoss");
        swampBoss = PlayerPrefs.GetInt("swampBoss");

        if (forceBiome) { LevelManager.instance.currentArea = forcedBiome; }

        Debug.Log("Current biome = " + LevelManager.instance.currentArea);

        GoToScene();
    }

    public void DesertLoad()
    {
        Debug.Log("Loading DesertStart");
        LevelManager.instance.currentArea = LevelManager.AreaType.Desert;
        SceneManager.LoadScene(desertStartScene);
    }

    public void CityLoad()
    {
        Debug.Log("Loading CityStart");
        LevelManager.instance.currentArea = LevelManager.AreaType.City;
        SceneManager.LoadScene(cityStartScene);
    }

    public void SwampLoad()
    {
        Debug.Log("Loading SwampStart");
        LevelManager.instance.currentArea = LevelManager.AreaType.Swamp;
        SceneManager.LoadScene(swampStartScene);
    }

    public void GoToScene() //check if player has beaten boss and not changed wanted board, if so change default **THIS IS FOR BETA/ UNTIL EACH TOWN HAS A WAY TO GO BACK TO WANTED BOARD**
    {
        switch (LevelManager.instance.currentArea)
        {
            case LevelManager.AreaType.City:
                if(cityBoss != 1) { CityLoad(); break; }
                if(cityBoss == 1 && desertBoss != 1)
                {
                    DesertLoad();
                    break;
                }
                if(cityBoss == 1 && desertBoss == 1 && swampBoss != 1)
                {
                    SwampLoad();
                    break;
                }
                if(cityBoss == 1 && desertBoss == 1 && swampBoss == 1)
                {
                    Debug.Log("You beat every run, just go to win screen for now.");
                    SceneManager.LoadScene("WinScene");
                }
                break;
            case LevelManager.AreaType.Swamp:
                if (swampBoss != 1) { SwampLoad(); break; }
                if (swampBoss == 1 && desertBoss != 1)
                {
                    DesertLoad();
                    break;
                }
                if (swampBoss == 1 && desertBoss == 1 && cityBoss != 1)
                {
                    CityLoad();
                    break;
                }
                if (cityBoss == 1 && desertBoss == 1 && swampBoss == 1)
                {
                    Debug.Log("You beat every run, just go to win screen for now.");
                    SceneManager.LoadScene("WinScene");
                }
                break;
            case LevelManager.AreaType.Desert:
                if (desertBoss != 1) { DesertLoad(); break; }
                if (desertBoss == 1 && swampBoss != 1)
                {
                    SwampLoad();
                    break;
                }
                if (desertBoss == 1 && swampBoss == 1 && cityBoss != 1)
                {
                    CityLoad();
                    break;
                }
                if (desertBoss == 1 && swampBoss == 1 && cityBoss == 1)
                {
                    Debug.Log("You beat every run, just go to win screen for now.");
                    SceneManager.LoadScene("WinScene");
                }
                break;
        }
    }
}

