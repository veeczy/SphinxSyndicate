using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public enum AreaType { Desert, City, Swamp }

    [Header("Current Area (set by wanted board)")]
    public AreaType currentArea = AreaType.Desert;

    [Header("Random Rooms (build indexes)")]
    public List<int> desertRoomIndexes = new List<int>();
    public List<int> cityRoomIndexes = new List<int>();
    public List<int> swampRoomIndexes = new List<int>();

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

    public void LoadSceneByTrigger(int sceneIndex)
    {
        if (sceneIndex == 1 || sceneIndex == 2)
        {
            SceneManager.LoadScene(sceneIndex);
            return;
        }

        LoadNextRoom();
    }

    // NEW: call this from the door that exits the biome start zone
    public void StartRunInCurrentArea()
    {
        roomsCompleted = 0;
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
