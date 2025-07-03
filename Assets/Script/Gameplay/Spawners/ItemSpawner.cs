using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] itemPrefabs;
    public float spawnInterval = 15f;

    private BoxCollider2D spawnArea;
    private Coroutine spawnRoutine;

    private void Awake()
    {
        spawnArea = GetComponent<BoxCollider2D>();
    }

    // Panggil ini dari LevelIntroManager saat cutscene selesai
    public void BeginSpawning()
    {
        spawnRoutine = StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator SpawnItemRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            Vector2 randomPosition = GetRandomPositionInBounds();
            GameObject randomItemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            Instantiate(randomItemPrefab, randomPosition, Quaternion.identity);
            Debug.Log("Spawned " + randomItemPrefab.name + " at " + randomPosition);
        }
    }

    private Vector2 GetRandomPositionInBounds()
    {
        Bounds bounds = spawnArea.bounds;
        return new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnItemRoutine());
    }

}
