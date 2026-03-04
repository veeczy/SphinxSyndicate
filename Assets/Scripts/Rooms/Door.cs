using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Trigger Settings")]
    public bool onTrigger = true;

    [Header("Is this a secret door?")]
    public bool isSecretDoor = false;

    [Header("Backwards or Forwards (is this where you entered)")]
    public bool isEntry = true;

    [Header("Force Scene Name (filling this overrides randomizer)")]
    public string sceneName;  // NEW

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!onTrigger) return;
        if (!other.CompareTag("Player")) return;

        if (sceneName != "")
        {
            LevelManager.instance.LoadSceneByTrigger(sceneName);
            return;
        }
        else if (isSecretDoor)
        {
            LevelManager.instance.LoadSecretRoom();
        }
        else if (isEntry)
        {
            LevelManager.instance.LoadPreviousRoom();
        }
        else
        {
            LevelManager.instance.LoadNextRoom();
        }
    }
}





