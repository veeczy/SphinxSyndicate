using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Random Rooms (build indexes)")]
    public List<int> randomRoomIndexes = new List<int>();  // Assign rooms here

    [Header("Secret Room (optional)")]
    public int secretRoomIndex = -1;

    [Header("Boss Room")]
    public int bossRoomIndex = -1;

    [Header("Rooms Before Boss")]
    public int maxRoomsBeforeBoss = 5;   // NEW: how many rooms before boss

    public int roomsCompleted = 0;       // NEW: counter for completed rooms

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

    // NEW: lets doors force scene 1 or 2 instead of using random rooms
    public void LoadSceneByTrigger(int sceneIndex)
    {
        // If sceneIndex is 1 or 2 → override everything
        if (sceneIndex == 1 || sceneIndex == 2)
        {
            SceneManager.LoadScene(sceneIndex);
            return;
        }

        // Otherwise, follow normal progression rules
        LoadNextRoom();
    }

    // Called by Door.cs
    public void LoadNextRoom()
    {
        roomsCompleted++;  // NEW: count each completed room

        // NEW: if we’ve reached the room limit, go to boss
        if (roomsCompleted >= maxRoomsBeforeBoss)
        {
            SceneManager.LoadScene(bossRoomIndex);
            return;
        }

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

    // NEW: reset room progress for a new run
    public void ResetRun()
    {
        roomsCompleted = 0;
        // you can also rebuild randomRoomIndexes here if you need to
    }

    public void LoadSecretRoom()
    {
        if (secretRoomIndex >= 0)
            SceneManager.LoadScene(secretRoomIndex);
        else
            Debug.LogWarning("Secret room index not set!");
    }
}

