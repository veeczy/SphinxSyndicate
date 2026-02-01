using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public enum AreaType { Desert, City, Swamp }

    [Header("Current Area (set by wanted board)")]
    public AreaType currentArea = AreaType.Desert;

    // ----------------------------
    // RANDOM ROOMS BY AREA
    // ----------------------------
    [Header("Random Rooms (build indexes)")]
    public List<int> desertRoomIndexes = new List<int>();
    public List<int> cityRoomIndexes = new List<int>();
    public List<int> swampRoomIndexes = new List<int>();

    [Header("Secret Rooms (optional)")]
    public int desertSecretRoomIndex = -1;
    public int citySecretRoomIndex = -1;
    public int swampSecretRoomIndex = -1;

    [Header("Boss Rooms")]
    public int desertBossRoomIndex = -1;
    public int cityBossRoomIndex = -1;
    public int swampBossRoomIndex = -1;

    [Header("Rooms Before Boss")]
    public int maxRoomsBeforeBoss = 5;

    public int roomsCompleted = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // unchanged override behavior
    public void LoadSceneByTrigger(int sceneIndex)
    {
        if (sceneIndex == 1 || sceneIndex == 2)
        {
            SceneManager.LoadScene(sceneIndex);
            return;
        }

        LoadNextRoom();
    }

    // ----------------------------
    // CORE RANDOM LOGIC (same flow)
    // ----------------------------
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
        // designer can repopulate lists if needed
    }

    public void LoadSecretRoom()
    {
        int secret = GetSecretRoom();
        if (secret >= 0)
            SceneManager.LoadScene(secret);
        else
            Debug.LogWarning("Secret room index not set for this area!");
    }

    // ----------------------------
    // HELPERS (NEW, SIMPLE)
    // ----------------------------
    private List<int> GetCurrentRoomPool()
    {
        return currentArea switch
        {
            AreaType.City => cityRoomIndexes,
            AreaType.Swamp => swampRoomIndexes,
            _ => desertRoomIndexes,
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

    private int GetSecretRoom()
    {
        return currentArea switch
        {
            AreaType.City => citySecretRoomIndex,
            AreaType.Swamp => swampSecretRoomIndex,
            _ => desertSecretRoomIndex,
        };
    }
}
