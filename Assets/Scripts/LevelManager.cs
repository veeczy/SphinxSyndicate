using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Random Rooms (build indexes)")]
    public List<int> randomRoomIndexes = new List<int>();

    [Header("Secret Room (optional)")]
    public int secretRoomIndex = -1;

    [Header("Boss Room")]
    public int bossRoomIndex = -1;

    [Header("Rooms Before Boss")]
    public int maxRoomsBeforeBoss = 5;   // how many rooms before boss

    public int roomsCompleted = 0;       // counter for completed rooms


    void Awake()
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


    // NEW: If a trap or trigger sends 1 or 2, we load that scene instead
    public void LoadSceneByTrigger(int sceneIndex)
    {
        // NEW: priority scenes override everything
        if (sceneIndex == 1 || sceneIndex == 2)
        {
            SceneManager.LoadScene(sceneIndex);
            return;
        }

        // Otherwise, run normal random progression
        LoadNextRoom();
    }


    // Called by Door.cs or by LoadSceneByTrigger()
    public void LoadNextRoom()
    {
        roomsCompleted++;  // NEW: count each completed room

        // NEW: Go to boss early if room limit reached
        if (roomsCompleted >= maxRoomsBeforeBoss)
        {
            SceneManager.LoadScene(bossRoomIndex);
            return;
        }

        // ORIGINAL: if we ran out of random rooms
        if (randomRoomIndexes.Count == 0)
        {
            SceneManager.LoadScene(bossRoomIndex);
            return;
        }

        // RANDOM ROOM LOGIC
        int randomIndex = Random.Range(0, randomRoomIndexes.Count);
        int selectedRoom = randomRoomIndexes[randomIndex];
        randomRoomIndexes.RemoveAt(randomIndex);

        SceneManager.LoadScene(selectedRoom);
    }


    // NEW: reset everything on death or restart
    public void ResetRun()
    {
        roomsCompleted = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void LoadSecretRoom()
    {
        if (secretRoomIndex >= 0)
            SceneManager.LoadScene(secretRoomIndex);
        else
            Debug.LogWarning("Secret room index not set!");
    }
}

