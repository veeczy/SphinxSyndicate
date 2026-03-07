using UnityEngine;

public class StartRunDoor : MonoBehaviour
{
    public bool onTrigger = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!onTrigger) return;
        if (!other.CompareTag("Player")) return;

        if (LevelManager.instance == null)
        {
            Debug.LogError("LevelManager not found!");
            return;
        }

        // starts random rooms for the currently selected biome
        LevelManager.instance.StartRunInCurrentArea();
    }
}

