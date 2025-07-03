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
        [Tooltip("Time between each spawn in seconds")]
        public float spawnInterval = 1f;
        [Tooltip("Number of objects to spawn in this wave")]
        public int spawnCount = 5;
        public bool loopForever = false;
        [Tooltip("Minimum duration of the wave in seconds")]
        public float minDuration = 10f;
        [Tooltip("Maximum number of active objects at once")]
        public int maxActiveObjects = 10;
    }

    [Header("Wave Settings")]
    [SerializeField] private List<Wave> waves;
    [SerializeField] private Transform spawn1;
    [SerializeField] private Transform spawn2;

    [HideInInspector] public bool finishedSpawning = false;
    private Dictionary<Wave, List<GameObject>> activeObjects = new Dictionary<Wave, List<GameObject>>();
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
            // Check if we can spawn more objects
            if (GetActiveObjectCount(wave) < wave.maxActiveObjects)
            {
                Spawn(wave);
                spawned++;
            }
            yield return new WaitForSeconds(wave.spawnInterval);
        }

        // Wait if the wave duration hasn't been reached
        float remainingTime = wave.minDuration - (Time.time - startTime);
        if (remainingTime > 0f)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        Debug.Log("✅ Wave completed: " + wave.waveName);
    }

    private IEnumerator SpawnLoopingWave(Wave wave)
    {
        while (true)
        {
            if (GetActiveObjectCount(wave) < wave.maxActiveObjects)
            {
                Spawn(wave);
            }
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private void Spawn(Wave wave)
    {
        Vector2 pos = RandomSpawnPoint();
        GameObject spawnedObject = Instantiate(wave.prefab, pos, Quaternion.identity);
        AddActiveObject(wave, spawnedObject);
    }

    private void AddActiveObject(Wave wave, GameObject obj)
    {
        if (!activeObjects.ContainsKey(wave))
        {
            activeObjects[wave] = new List<GameObject>();
        }
        activeObjects[wave].Add(obj);
    }

    private int GetActiveObjectCount(Wave wave)
    {
        if (!activeObjects.ContainsKey(wave))
        {
            return 0;
        }
        activeObjects[wave].RemoveAll(obj => obj == null);
        return activeObjects[wave].Count;
    }

    private Vector2 RandomSpawnPoint()
    {
        float randomY = Random.Range(spawn1.position.y, spawn2.position.y);
        return new Vector2(spawn1.position.x, randomY);
    }
}
