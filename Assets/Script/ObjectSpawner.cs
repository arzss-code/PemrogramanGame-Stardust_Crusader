using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject prefab;
        public float spawnInterval = 1f;
        public int spawnCount = 5;
    }

    [Header("Wave Settings")]
    [SerializeField] private List<Wave> waves;
    [SerializeField] private Transform spawn1;
    [SerializeField] private Transform spawn2;

    private int currentWaveIndex = 0;
    private int spawnedInCurrentWave = 0;
    private float spawnTimer = 0f;
    private bool waveActive = false;

    private void Start()
    {
        if (waves.Count > 0)
        {
            waveActive = true;
        }
    }

    private void Update()
    {
        if (!waveActive || waves.Count == 0) return;

        Wave currentWave = waves[currentWaveIndex];

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentWave.spawnInterval)
        {
            spawnTimer = 0f;

            if (spawnedInCurrentWave < currentWave.spawnCount)
            {
                Spawn(currentWave);
                spawnedInCurrentWave++;
            }
            else
            {
                // Proceed to next wave
                currentWaveIndex++;
                spawnedInCurrentWave = 0;

                if (currentWaveIndex >= waves.Count)
                {
                    currentWaveIndex = 0; // Loop back to the first wave
                }
            }
        }
    }

    private void Spawn(Wave wave)
    {
        Vector2 spawnPosition = RandomSpawnPoint();
        Instantiate(wave.prefab, spawnPosition, Quaternion.identity);
    }

    private Vector2 RandomSpawnPoint()
    {
        float randomY = Random.Range(spawn1.position.y, spawn2.position.y);
        return new Vector2(spawn1.position.x, randomY);
    }
}
