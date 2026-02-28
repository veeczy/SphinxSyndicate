using UnityEngine;
using System.Collections;

public class RatSpawner : MonoBehaviour
{
    [Header("Spawn While This Is Alive")]
    public GameObject anchorObject;          // when this dies (becomes null), spawning stops

    [Header("What To Spawn")]
    public GameObject ratPrefab;

    [Header("Spawn Settings")]
    public float spawnInterval = 10f;
    public Transform[] spawnPoints;          // optional: if empty, spawns at this transform
    public int maxAlive = 8;                 // optional cap

    private Coroutine spawnRoutine;

    void Start()
    {
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            // stop when anchor is dead/destroyed
            if (anchorObject == null)
                yield break;

            if (ratPrefab != null && CountAlive() < maxAlive)
            {
                Vector3 pos = transform.position;

                if (spawnPoints != null && spawnPoints.Length > 0)
                    pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;

                Instantiate(ratPrefab, pos, Quaternion.identity);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private int CountAlive()
    {
        // cheapest simple cap: count by tag (set Rat prefabs to tag "Enemy" or "Rat")
        // If you don’t want a cap, set maxAlive to a huge number.
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }
}