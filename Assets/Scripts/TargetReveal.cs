using UnityEngine;

public class TargetReveal : MonoBehaviour
{
    public GameObject hiddenWall;  // Assign the wall to hide/reveal
    public bool destroyWall = true; // If true, the wall disappears; if false, it toggles on/off

    void OnTriggerEnter2D(Collider2D col)
    {
        // Check if the object that hit it has a BulletId component
        if (col.GetComponent<BulletId>() != null)
        {
            if (hiddenWall != null)
            {
                if (destroyWall)
                    Destroy(hiddenWall); // Make the wall vanish
                else
                    hiddenWall.SetActive(!hiddenWall.activeSelf); // Toggle visibility
            }

            Destroy(gameObject); // Optional: destroy the target after being shot
        }
    }
}
