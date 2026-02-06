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
        if (!onTrigger) return;
        if (!other.CompareTag("Player")) return;

        if (LevelManager.instance == null)
        {
            Debug.LogError("LevelManager not found!");
            return;
        }

        if (forceBiome)
            LevelManager.instance.currentArea = forcedBiome;

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
