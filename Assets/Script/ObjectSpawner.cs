using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefab;
    public float spawnInterval = 2f;

    private float spawnTimer;

    private void Update()
    {
        // Tidak spawn jika prefab belum diset
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
        Instantiate(prefab, transform.position, transform.rotation);
    }
}
