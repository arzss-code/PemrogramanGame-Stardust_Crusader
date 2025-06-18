using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefab;
    public float spawnInterval = 2f;

    private float spawnTimer;

    

    [SerializeField] private Transform spawn1;  // Posisi atas/titik awal Y
    [SerializeField] private Transform spawn2;  // Posisi bawah/titik akhir Y

    private void Update()
    {
        if (prefab == null) return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnObject();
        }
    }

    private void SpawnObject()
    {
        Vector2 spawnPosition = RandomSpawnPoint();
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    private Vector2 RandomSpawnPoint()
    {
        float randomY = Random.Range(spawn1.position.y, spawn2.position.y);
        return new Vector2(spawn1.position.x, randomY);  // X tetap, Y acak
    }
}
