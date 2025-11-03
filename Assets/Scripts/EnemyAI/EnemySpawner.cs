using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs; // Enemy types to spawn

    [Header("Spawn Settings")]
    public int enemiesToSpawn = 5; // Number of enemies per room
    public float spawnWidth = 10f; // Width of spawn area
    public float spawnHeight = 10f; // Height of spawn area
    public LayerMask wallLayer; // Assign the "Walls" layer in the Inspector

    private bool hasSpawned = false;

    void Start()
    {
        SpawnEnemiesOnce();
    }

    void SpawnEnemiesOnce()
    {
        if (hasSpawned) return;
        hasSpawned = true;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject prefab = enemyPrefabs[randomIndex];
            Vector2 spawnPos = GetSpawnPosition();
            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

            // Only adjust spiders to walls
            if (prefab.name.ToLower().Contains("spider"))
                AlignSpiderToWall(enemy);
        }
    }

    Vector2 GetSpawnPosition()
    {
        float offsetX = Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
        float offsetY = Random.Range(-spawnHeight / 2f, spawnHeight / 2f);
        return new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
    }

    void AlignSpiderToWall(GameObject spider)
    {
        // Always check a wide area, not just near the spawner
        float maxSearchRadius = 50f; // Adjust to match your room size (works even for 10x10 rooms)
        float step = 2f;
        Collider2D closestWall = null;

        for (float radius = 1f; radius <= maxSearchRadius; radius += step)
        {
            // Visual debug line to show search radius
            Debug.DrawRay(spider.transform.position, Vector2.up * radius, Color.red, 1f);

            Collider2D[] hits = Physics2D.OverlapCircleAll(spider.transform.position, radius, wallLayer);
            if (hits.Length > 0)
            {
                // Find the nearest wall
                closestWall = hits[0];
                float closestDist = Vector2.Distance(spider.transform.position, hits[0].ClosestPoint(spider.transform.position));

                foreach (Collider2D h in hits)
                {
                    float dist = Vector2.Distance(spider.transform.position, h.ClosestPoint(spider.transform.position));
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestWall = h;
                    }
                }
                break;
            }
        }

        if (closestWall != null)
        {
            StickToWall(spider, closestWall);
        }
        else
        {
            // Fallback: pick a random direction toward the map edges if no wall detected
            Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            Vector2 randomDir = directions[Random.Range(0, directions.Length)];
            spider.transform.position += (Vector3)(randomDir * 8f);
            Debug.LogWarning("Spider failed to find wall, moved randomly toward one side.");
        }
    }

    void StickToWall(GameObject spider, Collider2D wall)
    {
        // Calculate direction and rotation toward the wall
        Vector2 dirToWall = (wall.ClosestPoint(spider.transform.position) - (Vector2)spider.transform.position).normalized;
        float angle = Mathf.Atan2(dirToWall.y, dirToWall.x) * Mathf.Rad2Deg;

        // Rotate spider so it's properly aligned with the wall
        spider.transform.rotation = Quaternion.Euler(0, 0, angle + 90);

        // Find the wall point and position spider slightly away from it
        Vector2 wallPoint = wall.ClosestPoint(spider.transform.position);
        float distanceToWall = Vector2.Distance(spider.transform.position, wallPoint);

        float spacing = 0.7f; // how far from the wall the spider should rest
        spider.transform.position = wallPoint - (dirToWall * spacing);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnWidth, spawnHeight, 0));
    }
}
