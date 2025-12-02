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
        Collider2D closestWall = FindClosestWall(spider.transform.position);

        if (closestWall == null)
            return;

        StickSpiderSafely(spider, closestWall);
    }

    Collider2D FindClosestWall(Vector2 origin)
    {
        float searchRadius = 40f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, searchRadius, wallLayer);

        if (hits.Length == 0) return null;

        // Find nearest wall collider
        Collider2D best = hits[0];
        float bestDist = Vector2.Distance(origin, best.ClosestPoint(origin));

        foreach (Collider2D h in hits)
        {
            float d = Vector2.Distance(origin, h.ClosestPoint(origin));
            if (d < bestDist)
            {
                best = h;
                bestDist = d;
            }
        }

        return best;
    }

    void StickSpiderSafely(GameObject spider, Collider2D wall)
    {
        Vector2 spiderPos = spider.transform.position;
        Vector2 wallPoint = wall.ClosestPoint(spiderPos);

        // Direction pointing FROM wall → to spider
        Vector2 dir = (spiderPos - wallPoint).normalized;

        // If spider spawned on exact wallPoint, push upward
        if (dir == Vector2.zero)
            dir = Vector2.up;

        // Push it 0.5 units OUTSIDE the wall (safe offset)
        Vector2 safePosition = wallPoint + dir * 0.5f;

        // Lock rotation facing AWAY from the wall
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;

        spider.transform.SetPositionAndRotation(safePosition, Quaternion.Euler(0, 0, angle));
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnWidth, spawnHeight, 0));
    }
}
