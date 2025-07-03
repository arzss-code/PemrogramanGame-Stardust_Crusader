using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{

    [Header("Spawner Reference (Opsional)")]
    [SerializeField] private ObjectSpawner objectSpawner;

    [System.Serializable]

    public class Wave
    {
        public string waveName;
        public GameObject prefab;
        public float spawnInterval = 1f;
        public int spawnCount = 5;
        public bool loopForever = false;
        public float minDuration = 10f;
    }

    [Header("Wave Settings")]
    [SerializeField] private List<Wave> waves;
    [SerializeField] private Transform spawn1;
    [SerializeField] private Transform spawn2;

    [HideInInspector] public bool finishedSpawning = false;

    // Dipanggil oleh LevelIntroManager setelah cutscene selesai
    public void BeginSpawning()
    {
        StartCoroutine(StartWaves());
    }

    private IEnumerator StartWaves()
    {
        for (int i = 0; i < waves.Count; i++)
        {
            Wave wave = waves[i];
            Debug.Log($"🚀 Memulai Wave {i}: {wave.waveName}");

            if (i > 0) yield return new WaitForSeconds(2f); // Delay antar wave

            if (wave.loopForever)
            {
                // Jalankan wave yang loop terus tanpa menghambat wave berikutnya
                StartCoroutine(SpawnLoopingWave(wave));
            }
            else
            {
                yield return StartCoroutine(SpawnSingleWave(wave));
            }
        }

        finishedSpawning = true;
        Debug.Log("🎯 Semua wave ObjectSpawner selesai!");
    }

    private IEnumerator SpawnSingleWave(Wave wave)
    {
        int spawned = 0;
        float startTime = Time.time;

        while (spawned < wave.spawnCount)
        {
            Spawn(wave);
            spawned++;
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        // Tunggu jika durasi wave belum cukup
        float remainingTime = wave.minDuration - (Time.time - startTime);
        if (remainingTime > 0f)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        Debug.Log("✅ Wave selesai: " + wave.waveName);
    }

    private IEnumerator SpawnLoopingWave(Wave wave)
    {
        while (true)
        {
            Spawn(wave);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private void Spawn(Wave wave)
    {
        Vector2 pos = RandomSpawnPoint();
        Instantiate(wave.prefab, pos, Quaternion.identity);
    }

    private Vector2 RandomSpawnPoint()
    {
        float randomY = Random.Range(spawn1.position.y, spawn2.position.y);
        return new Vector2(spawn1.position.x, randomY);
    }
}
