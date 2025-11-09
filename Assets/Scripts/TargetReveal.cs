using UnityEngine;

public class TargetReveal : MonoBehaviour
{
    [Header("Walls to Destroy or Disable")]
    public GameObject visualWall;   // The visible wall (for art)
    public GameObject colliderWall; // The invisible collider wall

    [Header("Settings")]
    public bool destroyWalls = true; // True = destroy, False = toggle on/off

    private static bool wallsDestroyed = false; // Tracks if already destroyed

    void Start()
    {
        // If player already hit the target before dying, keep the walls gone
        if (wallsDestroyed)
        {
            if (visualWall != null) visualWall.SetActive(false);
            if (colliderWall != null) colliderWall.SetActive(false);
            Destroy(gameObject); // Target stays gone
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<BulletId>() != null && !wallsDestroyed)
        {
            wallsDestroyed = true;

            if (visualWall != null)
            {
                if (destroyWalls) Destroy(visualWall);
                else visualWall.SetActive(!visualWall.activeSelf);
            }

            if (colliderWall != null)
            {
                if (destroyWalls) Destroy(colliderWall);
                else colliderWall.SetActive(!colliderWall.activeSelf);
            }

            Destroy(gameObject); // Target disappears after being shot
        }
    }
}
