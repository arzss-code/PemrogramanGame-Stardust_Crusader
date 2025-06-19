using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefab; // Prefab objek yang akan di-spawn
    public float spawnInterval = 2f; // Interval waktu antar spawn

    private float spawnTimer; // Timer untuk menghitung waktu spawn berikutnya

    [SerializeField] private Transform spawn1; // Posisi atas/titik awal Y untuk spawn
    [SerializeField] private Transform spawn2; // Posisi bawah/titik akhir Y untuk spawn

    private void Update()
    {
        // Jika prefab tidak diatur, keluar dari fungsi
        if (prefab == null) return;

        // Tambahkan waktu yang berlalu ke spawnTimer
        spawnTimer += Time.deltaTime;

        // Jika spawnTimer melebihi atau sama dengan spawnInterval, spawn objek baru
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f; // Reset spawnTimer
            SpawnObject(); // Panggil fungsi untuk spawn objek
        }
    }

    private void SpawnObject()
    {
        // Tentukan posisi spawn acak
        Vector2 spawnPosition = RandomSpawnPoint();
        // Spawn objek prefab pada posisi yang ditentukan dengan rotasi default
        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    private Vector2 RandomSpawnPoint()
    {
        // Tentukan posisi Y acak antara spawn1 dan spawn2
        float randomY = Random.Range(spawn1.position.y, spawn2.position.y);
        // Kembalikan posisi spawn dengan X tetap dan Y acak
        return new Vector2(spawn1.position.x, randomY);
    }
}