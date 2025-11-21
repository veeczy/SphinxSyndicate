using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Random Rooms (type scene names here)")]
    public List<string> randomRooms = new List<string>();

    [Header("Secret Room (optional)")]
    public string secretRoomName = "";

    [Header("Boss Room (final scene)")]
    public string bossSceneName = "DV_Boss";

    private HashSet<string> usedRooms = new HashSet<string>();


    // ------------------------------
    //  Singleton Setup
    // ------------------------------
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


    // ------------------------------
    //  Random Room Selection
    // ------------------------------
    public string GetNextRandomRoom()
    {
        return GetRandomRoom();
    }

    public string GetRandomRoom()
    {
        List<string> availableRooms = new List<string>();

        foreach (string room in randomRooms)
        {
            if (!usedRooms.Contains(room))
                availableRooms.Add(room);
        }

        // If out of random rooms, go to boss
        if (availableRooms.Count == 0)
        {
            Debug.Log("No more random rooms. Sending to boss.");
            return bossSceneName;
        }

        string chosen = availableRooms[Random.Range(0, availableRooms.Count)];
        usedRooms.Add(chosen);

        return chosen;
    }


    // ------------------------------
    //  Secret Room
    // ------------------------------
    public string GetSecretRoom()
    {
        if (!string.IsNullOrEmpty(secretRoomName))
            return secretRoomName;

        Debug.LogWarning("Secret room not set — returning random instead.");
        return GetRandomRoom();
    }


    // ------------------------------
    //  Load Room Wrapper
    // ------------------------------
    public void LoadRoom(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    // ------------------------------
    //  Reset Room History
    // ------------------------------
    public void ResetUsedRooms()
    {
        usedRooms.Clear();
    }
}
