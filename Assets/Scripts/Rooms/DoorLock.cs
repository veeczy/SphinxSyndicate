using UnityEngine;

public class DoorLock : MonoBehaviour
{
    public bool startLocked = true;

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        if (col != null) col.enabled = !startLocked;
    }

    public void UnlockDoor()
    {
        if (col != null) col.enabled = true;
    }

    public void LockDoor()
    {
        if (col != null) col.enabled = false;
    }
}

