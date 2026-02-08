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

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("BiomeDoor triggered by: " + other.name + " tag=" + other.tag);

        if (!onTrigger) return;

        if (LevelManager.instance == null)
        {
            Debug.LogError("LevelManager not found!");
            return;
        }

        if (forceBiome)
            LevelManager.instance.currentArea = forcedBiome;

        Debug.Log("Current biome = " + LevelManager.instance.currentArea);

        switch (LevelManager.instance.currentArea)
        {
            case LevelManager.AreaType.City:
                Debug.Log("Loading CityStart");
                SceneManager.LoadScene(cityStartScene);
                break;
            case LevelManager.AreaType.Swamp:
                Debug.Log("Loading SwampStart");
                SceneManager.LoadScene(swampStartScene);
                break;
            default:
                Debug.Log("Loading DesertStart");
                SceneManager.LoadScene(desertStartScene);
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("COLLISION with: " + col.collider.name);
    }
}

