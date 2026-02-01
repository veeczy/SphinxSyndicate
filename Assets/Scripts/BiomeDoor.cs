using UnityEngine;
using UnityEngine.SceneManagement;

public class BiomeDoor : MonoBehaviour
{
    public bool onTrigger = true;

    public string desertStartScene = "DesertStart";
    public string cityStartScene = "CityStart";
    public string swampStartScene = "SwampStart";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!onTrigger) return;
        if (!other.CompareTag("Player")) return;

        if (LevelManager.instance == null)
        {
            Debug.LogError("LevelManager not found!");
            return;
        }

        switch (LevelManager.instance.currentArea)
        {
            case LevelManager.AreaType.City:
                SceneManager.LoadScene(cityStartScene);
                break;
            case LevelManager.AreaType.Swamp:
                SceneManager.LoadScene(swampStartScene);
                break;
            default:
                SceneManager.LoadScene(desertStartScene);
                break;
        }
    }
}

