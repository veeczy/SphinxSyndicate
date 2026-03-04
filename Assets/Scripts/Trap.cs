using UnityEngine;

public class Trap : MonoBehaviour
{
    public bool armed;
    public GameObject trapWall;
    public GameObject sceneTrigger;
    public GameObject[] enemies;
    public int enemyCount;

    public int roomIndex;
    public int roomsCompleted;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //retrieve variables from level manager
        roomIndex = LevelManager.instance.currentRoomIndex; 
        roomsCompleted = LevelManager.instance.roomsCompleted;

        if(roomIndex < roomsCompleted) //if the room youre in is less than total rooms completed (checking if you are backtracking
        {  armed = false; }
        else { armed = true; } //if not backtracking, then arm the traps

        trapWall.SetActive(armed); //trap wall set to status of if armed
        sceneTrigger.SetActive(!armed); 
    }

    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = enemies.Length;

        if (enemyCount <= 0 && armed)
        {
            LevelManager.instance.RoomCounter();
            armed = false;
        }

        trapWall.SetActive(armed);
        sceneTrigger.SetActive(!armed);
    }

    public void ConditionMet() //for if you want to leave room after a different condition is met, seperate from killing enemies, you can call this
    {
        LevelManager.instance.RoomCounter();
    }
}

