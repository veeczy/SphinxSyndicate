using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType { Random, Fixed, Secret }
    public DoorType doorType;

    [Header("Fixed Room Name (only used for Fixed doors)")]
    public string fixedRoomName;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        switch (doorType)
        {
            case DoorType.Fixed:
                LevelManager.instance.LoadRoom(fixedRoomName);
                break;

            case DoorType.Secret:
                LevelManager.instance.LoadRoom(LevelManager.instance.GetSecretRoom());
                break;

            case DoorType.Random:
                LevelManager.instance.LoadRoom(LevelManager.instance.GetRandomRoom());
                break;
        }
    }
}

