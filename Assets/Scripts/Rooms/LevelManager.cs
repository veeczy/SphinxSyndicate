using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public enum AreaType { Desert, City, Swamp }

    [Header("Current Area (set by wanted board)")]
    public AreaType currentArea = AreaType.Desert;

    // MASTER LISTS (edit in Inspector, never modified)
    [Header("Random Rooms MASTER (build indexes)")]
    public List<int> desertRoomIndexes = new List<int>();
    public List<int> cityRoomIndexes = new List<int>();
    public List<int> swampRoomIndexes = new List<int>();

    // RUNTIME POOLS (auto-built, these get modified)
    private List<int> desertRoomPool = new List<int>();
    private List<int> cityRoomPool = new List<int>();
    private List<int> swampRoomPool = new List<int>();

    [Header("Secret Rooms (optional)")]
    public List<int> desertSecretRoomIndexes = new List<int>();
    public List<int> citySecretRoomIndexes = new List<int>();
    public List<int> swampSecretRoomIndexes = new List<int>();

    [Header("Boss Rooms")]
    public int desertBossRoomIndex = -1;
    public int cityBossRoomIndex = -1;
    public int swampBossRoomIndex = -1;

    [Header("Rooms Before Boss")]
    public int maxRoomsBeforeBoss = 5;

    public int roomsCompleted = 0;

    [Header("Reset when entering this scene index")]
    public int resetSceneIndex = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            RebuildAllRoomPools(); // build pools once at game start
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == resetSceneIndex)
        {
            roomsCompleted = 0;
            RebuildAllRoomPools(); // KEY: restart random pools too
        }
    }

    public void LoadSceneByTrigger(int sceneIndex)
    {
        if (sceneIndex == 1 || sceneIndex == 2)
        {
            SceneManager.LoadScene(sceneIndex);
            return;
        }

        LoadNextRoom();
    }

    // Call this when leaving the biome start zone
    public void StartRunInCurrentArea()
    {
        roomsCompleted = 0;
        RebuildPoolForCurrentArea(); // fresh rooms for that biome
        LoadNextRoom();
    }

    public void LoadNextRoom()
    {
        roomsCompleted++;

        if (roomsCompleted >= maxRoomsBeforeBoss)
        {
            SceneManager.LoadScene(GetBossRoom());
            return;
        }

        List<int> pool = GetCurrentRoomPool();

        if (pool.Count == 0)
        {
            SceneManager.LoadScene(GetBossRoom());
            return;
        }

        int randomIndex = Random.Range(0, pool.Count);
        int selectedRoom = pool[randomIndex];
        pool.RemoveAt(randomIndex);

        SceneManager.LoadScene(selectedRoom);
    }

    public void ResetRun()
    {
        roomsCompleted = 0;
        RebuildAllRoomPools();
    }

    public void LoadSecretRoom()
    {
        List<int> secrets = GetSecretRoomPool();

        if (secrets.Count == 0)
        {
            Debug.LogWarning("No secret rooms set for this area!");
            return;
        }

        int pick = Random.Range(0, secrets.Count);
        SceneManager.LoadScene(secrets[pick]);
    }

    private void RebuildAllRoomPools()
    {
        desertRoomPool = new List<int>(desertRoomIndexes);
        cityRoomPool = new List<int>(cityRoomIndexes);
        swampRoomPool = new List<int>(swampRoomIndexes);
    }

    private void RebuildPoolForCurrentArea()
    {
        switch (currentArea)
        {
            case AreaType.City:
                cityRoomPool = new List<int>(cityRoomIndexes);
                break;
            case AreaType.Swamp:
                swampRoomPool = new List<int>(swampRoomIndexes);
                break;
            default:
                desertRoomPool = new List<int>(desertRoomIndexes);
                break;
        }
    }

    private List<int> GetCurrentRoomPool()
    {
        return currentArea switch
        {
            AreaType.City => cityRoomPool,
            AreaType.Swamp => swampRoomPool,
            _ => desertRoomPool,
        };
    }

    private int GetBossRoom()
    {
        return currentArea switch
        {
            AreaType.City => cityBossRoomIndex,
            AreaType.Swamp => swampBossRoomIndex,
            _ => desertBossRoomIndex,
        };
    }

    private List<int> GetSecretRoomPool()
    {
        return currentArea switch
        {
            AreaType.City => citySecretRoomIndexes,
            AreaType.Swamp => swampSecretRoomIndexes,
            _ => desertSecretRoomIndexes,
        };
    }

    public void SetArea(AreaType area)
    {
        currentArea = area;
    }
}
