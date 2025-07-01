using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Daftar semua prefab item yang bisa muncul")]
    public GameObject[] itemPrefabs;

    [Tooltip("Waktu jeda antar spawn (dalam detik)")]
    public float spawnInterval = 15f;

    private BoxCollider2D spawnArea;

    private void Awake()
    {
        // Ambil komponen BoxCollider2D yang akan menjadi area spawn
        spawnArea = GetComponent<BoxCollider2D>();
    }

    private void Start()
    {
        // Mulai coroutine untuk spawn item secara berulang
        StartCoroutine(SpawnItemRoutine());
    }

    private IEnumerator SpawnItemRoutine()
    {
        // Loop ini akan berjalan selamanya selama game aktif
        while (true)
        {
            // Tunggu sesuai interval waktu
            yield return new WaitForSeconds(spawnInterval);

            // Pilih posisi acak di dalam area BoxCollider2D
            Vector2 randomPosition = GetRandomPositionInBounds();

            // Pilih prefab item acak dari daftar
            GameObject randomItemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];

            // Munculkan item di posisi acak
            Instantiate(randomItemPrefab, randomPosition, Quaternion.identity);

            Debug.Log("Spawned " + randomItemPrefab.name + " at " + randomPosition);
        }
    }

    private Vector2 GetRandomPositionInBounds()
    {
        Bounds bounds = spawnArea.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        return new Vector2(randomX, randomY);
    }
}