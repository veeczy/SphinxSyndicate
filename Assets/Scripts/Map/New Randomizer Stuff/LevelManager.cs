using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public enum AreaType { Desert, City, Swamp }

    [Header("Current Area (set by wanted board)")]
    public AreaType currentArea = AreaType.Desert; //defaults to desert

    // MASTER arrays (edit in Inspector, never modified)
    [Header("Random Rooms MASTER (build indexes)")] 
    public string[] desertRoomMaster;
    public string[] cityRoomMaster;
    public string[] swampRoomMaster;

    public string[] remainingdesertRoom;
    public string[] remainingcityRoom;
    public string[] remainingswampRoom;

    [Header("Secret Rooms MASTER (optional)")]
    public string[] desertSecretRooms;
    public string[] citySecretRooms;
    public string[] swampSecretRooms;

    [Header("Boss Rooms")]
    public string desertBossRoom;
    public string cityBossRoom;
    public string swampBossRoom;
    public string bossScene;

    [Header("Random Rooms (when you start Run)")]
    // RUNTIME POOLS (auto-built, these get modified) 
    public List<string> desertRoomPool = new List<string>();
    public List<string> cityRoomPool = new List<string>();
    public List<string> swampRoomPool = new List<string>();
    public int randomRoomIndex;
    public string randomRoomValue;

    [Header("Rooms Before Boss / Progress")]
    public int maxRoomsBeforeBoss; //set in inspector, defaulted to 5 before
    public int roomsCompleted = 0; //measures rooms that enemies been defeated in
    public string currentRoom; //records which room youre in by name
    public int currentRoomIndex; //records which room youre in by index
    public bool firstRoom;
    bool runStarted;

    [Header("Enemies Defeated / Progress")]
    public bool enemiesDefeated;
    public bool[] roomsCleared;

    [Header("Reset when entering this scene index")]
    public string resetSceneName;

    [Header("Boss Defeat Save")]
    public int desertBossClear;
    public int cityBossClear;
    public int swampBossClear;
    public bool AllClear;

    private void Awake()
    {
        if (instance == null) //if there is not an instance of level manager
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //bring level manager to next room

            RebuildAllRoomPools(); // build pools once at game start
        }
        else //if there is an instance of level manager already
        {
            Destroy(gameObject); //destroy duplicate
        }

        //INITIALIZE SAVE DATA FROM PLAYERPREFS
        desertBossClear = PlayerPrefs.GetInt("desertBoss");
        cityBossClear = PlayerPrefs.GetInt("cityBoss");
        swampBossClear = PlayerPrefs.GetInt("swampBoss");
    }

    private void Update()
    {
        if(runStarted) // if in current run check if enemies defeated
        {
            enemiesDefeated = roomsCleared[currentRoomIndex]; //enemies defeated is whatever the state of the bool says currently
        }

        //CHECK PLAYER PREFS FOR SAVE DATA
        desertBossClear = PlayerPrefs.GetInt("desertBoss");
        cityBossClear = PlayerPrefs.GetInt("cityBoss");
        swampBossClear = PlayerPrefs.GetInt("swampBoss");
    }

    public void LoadSceneByTrigger(string sceneName)
    {
        if (sceneName != "") //if sceneName is not empty
        {
            //old
            //SceneManager.LoadScene(sceneName);

            FadeManager.Instance.FadeAndLoadScene(sceneName); //new calls fade then load scene
            return;
        }

        LoadNextRoom(); //if sceneName is null just load next room
    }

    public void ReturnToTown()
    {
        //old
        //SceneManager.LoadScene("LoadingScreen"); //will go to biomedoor on loading screen and return to whatever town is starting area

        FadeManager.Instance.FadeAndLoadScene("LoadingScreen"); //new calls fade then load scene
    }

    private void RebuildAllRoomPools()
    {
        //RESET ROOMS THAT YOU CAN PULL FROM TO MATCH THE MASTER ARRAYS
        remainingdesertRoom = desertRoomMaster; 
        remainingcityRoom = cityRoomMaster;
        remainingswampRoom = swampRoomMaster;
    }

    private void AddRoomToList(List<string> list, string room)
    {
        list.Add(room); //add room to scene list
    }

    public void RandomRoom(string[] roomArray)
    {
        randomRoomIndex = Random.Range(0, roomArray.Length);
    }

    public void RollRooms(string[] remainingRooms, List<string> roomList)
    {
        RandomRoom(remainingRooms); //pick random room for master array that doesnt change
        if (remainingRooms[randomRoomIndex] != "") //if remaining rooms array does not show that room taken
        {
            randomRoomValue = remainingRooms[randomRoomIndex]; //save value of room so it can be referenced
            remainingRooms[randomRoomIndex] = ""; //null that room space so it can't be chosen again
            AddRoomToList(roomList, randomRoomValue); //save value of room to list for player run
        }
        else RollRooms(remainingRooms, roomList); //if room was already chosen roll again for new room that wasn't chosen
    }

    private void RebuildPoolForCurrentArea() //this sets up your run
    {
        switch (currentArea)
        {
            case AreaType.City: //if city
                while(cityRoomPool.Count < maxRoomsBeforeBoss) //while list of rooms picked are less than amount needed to get to boss
                {
                    RollRooms(remainingcityRoom, cityRoomPool); //add room
                }
                bossScene = cityBossRoom;
                ResetRoomClear(cityRoomPool);
                break;
            case AreaType.Swamp: //if swamp
                while (swampRoomPool.Count < maxRoomsBeforeBoss) //while list of rooms picked are less than amount needed to get to boss
                {
                    RollRooms(remainingswampRoom, swampRoomPool); //add room
                }
                bossScene = swampBossRoom;
                ResetRoomClear(swampRoomPool);
                break;
            default: //if desert
                while (desertRoomPool.Count < maxRoomsBeforeBoss) //while list of rooms picked are less than amount needed to get to boss
                {
                    RollRooms(remainingdesertRoom, desertRoomPool); //add room
                }
                bossScene = desertBossRoom;
                ResetRoomClear(desertRoomPool);
                break;
        }
    }

    //*TRIGGERS WHEN SCENE IS LOADED*//
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
        if (scene.name == resetSceneName)
        {
            roomsCompleted = 0;
            RebuildAllRoomPools(); // KEY: restart random pools too
        }
    }
    //*END CHECK FOR RESET SCENE*//


    // Call this when leaving the biome start zone
    public void StartRunInCurrentArea()
    {
        currentRoomIndex = 0;
        if(!runStarted) //if run was not already started
        {
            roomsCompleted = 0;
            RebuildPoolForCurrentArea(); // fresh rooms for that biome
            runStarted = true;
            LoadRoom();
        }
        if(runStarted)
        {
            LoadRoom();
        }
    }

    public void RoomCounter() //if condition met to proceed then room counter for completion goes up **WANT THIS CALLED IN TRAP SCRIPT**
    {
        roomsCompleted++;
    }

    public void ResetRoomClear(List<string> currentRoomPool) //creates array for current run, and initializes it
    {
        roomsCleared = new bool[currentRoomPool.Count]; //new array to count how many enemies are defeated, matches the length of the run rooms

        for (int i = 0; i < roomsCleared.Length; i++) //initialize array to say every room is not cleared yet
        {
            roomsCleared[i] = false;
        }
    }

    public void EnemiesDefeated() //called when all enemies are defeated, updates state of room to be cleared
    {
        roomsCleared[currentRoomIndex] = true;
    }

    public void LoadRoom() //loads scene for whichever room youre moving to
    {
        if (roomsCompleted >= maxRoomsBeforeBoss) //if met condition to go to boss
        {
            //old
           // SceneManager.LoadScene(bossScene); //load boss

            FadeManager.Instance.FadeAndLoadScene(bossScene); //new calls fade then load scene
            return;
        }
        switch (currentArea)
        {
            case AreaType.City: //if city
                //old
               // if (cityBossClear == 1) { SceneManager.LoadScene("LoadingScreen"); } //if city boss already cleared, go back to town
                //if (cityBossClear != 1) { SceneManager.LoadScene(cityRoomPool[currentRoomIndex]); } //load next scene
               
               if (cityBossClear == 1) { FadeManager.Instance.FadeAndLoadScene("LoadingScreen"); }
                if (cityBossClear != 1) { FadeManager.Instance.FadeAndLoadScene(cityRoomPool[currentRoomIndex]); } //load next scene
                break;
            case AreaType.Swamp: //if swamp
                //old
              //  if (swampBossClear == 1) { SceneManager.LoadScene("LoadingScreen"); } //if swamp boss already cleared, go back to town
                //if (swampBossClear != 1) { SceneManager.LoadScene(swampRoomPool[currentRoomIndex]); } //load next scene
               
               if (swampBossClear == 1) { FadeManager.Instance.FadeAndLoadScene("LoadingScreen"); }
                if (swampBossClear != 1) { FadeManager.Instance.FadeAndLoadScene(swampRoomPool[currentRoomIndex]); } //load next scene
                break;
            default: //if desert
                //old
                //if (desertBossClear == 1) { SceneManager.LoadScene("LoadingScreen"); } //if desert boss already cleared, go back to town
                // if (desertBossClear != 1) { SceneManager.LoadScene(desertRoomPool[currentRoomIndex]); } //load next scene
               if (desertBossClear == 1) { FadeManager.Instance.FadeAndLoadScene("LoadingScreen"); }
                if (desertBossClear != 1) { FadeManager.Instance.FadeAndLoadScene(desertRoomPool[currentRoomIndex]); } //load next scene
                break;
        }
    }

    public void LoadNextRoom() //if progressing further
    {
        if (roomsCompleted >= maxRoomsBeforeBoss) //if met condition to go to boss
        {
            //old
           // SceneManager.LoadScene(bossScene); //load boss

            FadeManager.Instance.FadeAndLoadScene(bossScene); //new calls fade then load scene
            return;
        }
        else //if not yet met boss condition
        {
            currentRoomIndex++;
            LoadRoom();
        }
    }

    public void LoadPreviousRoom() //if backtracking
    {
        currentRoomIndex = currentRoomIndex - 1; //go to previous index

        if (roomsCompleted >= maxRoomsBeforeBoss) //if met condition to go to boss
        {
            //old
           // SceneManager.LoadScene(bossScene); //load boss

            FadeManager.Instance.FadeAndLoadScene(bossScene); //new calls fade then load scene
            return;
        }
        else //if not yet met boss condition
        {
            LoadRoom();
        }
    }

    public void ResetRun()
    {
        roomsCompleted = 0;
        runStarted = false;
        RebuildAllRoomPools();
    }

    private string GetSecretRoomPool() //retrieve secret room
    {
        switch (currentArea)
        {
            case AreaType.City: //if city
                if (citySecretRooms.Length == 0)
                {
                    Debug.LogWarning("No secret rooms set for this area!");
                    randomRoomValue = null;
                }
                else
                {
                    RandomRoom(citySecretRooms); //retrieve randomroomIndex for secret rooms
                    randomRoomValue = citySecretRooms[randomRoomIndex];
                }
                return randomRoomValue;
            case AreaType.Swamp: //if swamp
                if (swampSecretRooms.Length == 0)
                {
                    Debug.LogWarning("No secret rooms set for this area!");
                    randomRoomValue = null;
                }
                else
                {
                    RandomRoom(swampSecretRooms); //retrieve randomroomIndex for secret rooms
                    randomRoomValue = swampSecretRooms[randomRoomIndex];
                }
                return randomRoomValue;
            default: //if desert
                if (desertSecretRooms.Length == 0)
                {
                    Debug.LogWarning("No secret rooms set for this area!");
                    randomRoomValue = null;
                }
                else
                {
                    RandomRoom(desertSecretRooms); //retrieve randomroomIndex for secret rooms
                    randomRoomValue = desertSecretRooms[randomRoomIndex];
                }
                return randomRoomValue;
        }
    }

    public void LoadSecretRoom()
    {
        randomRoomValue = GetSecretRoomPool(); //retrieve random secret room
        if (randomRoomValue != null) //if secret room is picked, next room is then set to secret area
        {
            switch (currentArea)
            {
                case AreaType.City:
                    cityRoomPool[currentRoomIndex + 1] = randomRoomValue;
                    break;
                case AreaType.Swamp:
                    swampRoomPool[currentRoomIndex + 1] = randomRoomValue;
                    break;
                default:
                    desertRoomPool[currentRoomIndex + 1] = randomRoomValue;
                    break;
            }
        }

        LoadNextRoom(); //move to next room, if secret room was added it will be the scene you go to
    }

    public void SetArea(AreaType area)
    {
        currentArea = area;
    }
}
