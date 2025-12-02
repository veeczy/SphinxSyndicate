using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Trigger Settings")]
    public bool onTrigger = true;

    [Header("Is this a secret door?")]
    public bool isSecretDoor = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!onTrigger) return;
        if (!other.CompareTag("Player")) return;

        if (isSecretDoor)
            LevelManager.instance.LoadSecretRoom();
        else
            LevelManager.instance.LoadNextRoom();
    }
}




