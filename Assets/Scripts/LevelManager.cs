using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Random Rooms (build indexes)")]
    public List<int> randomRoomIndexes = new List<int>();  // Assign 26 rooms

    [Header("Secret Room (optional)")]
    public int secretRoomIndex = -1;

    [Header("Boss Room")]
    public int bossRoomIndex = -1;

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

    // Called by Door.cs
    public void LoadNextRoom()
    {
        // If all rooms used → go to boss
        if (randomRoomIndexes.Count == 0)
        {
            SceneManager.LoadScene(bossRoomIndex);
            return;
        }

        // Pick a random room
        int randomIndex = Random.Range(0, randomRoomIndexes.Count);
        int selectedRoom = randomRoomIndexes[randomIndex];

        // Remove it so we never load it again
        randomRoomIndexes.RemoveAt(randomIndex);

        // Load the room
        SceneManager.LoadScene(selectedRoom);
    }

    public void LoadSecretRoom()
    {
        if (secretRoomIndex >= 0)
            SceneManager.LoadScene(secretRoomIndex);
        else
            Debug.LogWarning("Secret room index not set!");
    }
}
