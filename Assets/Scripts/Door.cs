using UnityEngine;



public class Door : MonoBehaviour
{
    [Header("Trigger Settings")]
    public bool onTrigger = true;

    [Header("Is this a secret door?")]
    public bool isSecretDoor = false;

    [Header("Force Scene Index (1 or 2 overrides randomizer)")]
    public int forcedSceneIndex = -1;  // NEW

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!onTrigger) return;
        if (!other.CompareTag("Player")) return;

        // NEW: if door should send player to scene 1 or 2
        if (forcedSceneIndex == 1 || forcedSceneIndex == 2)
        {
            LevelManager.instance.LoadSceneByTrigger(forcedSceneIndex);
            return; // NEW
        }

        if (isSecretDoor)
        {
            LevelManager.instance.LoadSecretRoom();
        }
        else
        {
            LevelManager.instance.LoadNextRoom();
        }
    }
}





