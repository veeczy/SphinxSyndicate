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
    public bool enemiesDefeated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //retrieve variables from level manager
        roomIndex = LevelManager.instance.currentRoomIndex; 
        roomsCompleted = LevelManager.instance.roomsCompleted;
        enemiesDefeated = LevelManager.instance.enemiesDefeated;

        if(enemiesDefeated) //if the enemies are previously defeated, disarm traps
        {  armed = false; }
        else { armed = true; } //if ehnemies were not previously defeated, arm traps

        trapWall.SetActive(armed); //trap wall set to status of if armed
        sceneTrigger.SetActive(!armed); //doors status set opposite of armed, if trapped then doors should not be present and vice versa
    }

    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = enemies.Length;

        if (enemyCount <= 0 && armed)
        {
            LevelManager.instance.RoomCounter(); //increase counter for rooms cleared
            LevelManager.instance.EnemiesDefeated(); //update the state of enemies being defeated for the room
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

