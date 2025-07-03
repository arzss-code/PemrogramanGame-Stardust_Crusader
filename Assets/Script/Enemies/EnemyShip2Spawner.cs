using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip2Spawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveConfiguration
    {
        [Header("Wave Settings")]
        public EnemyShip2.WaveType waveType;
        public string waveName;
        
        [Header("Spawn Settings")]
        public GameObject enemyPrefab;
        public int enemyCount = 5;
        public float spawnInterval = 1f;
        public float initialDelay = 0f;
        
        [Header("Formation Settings (Formation Wave Only)")]
        public int formationSize = 3;
        public float formationSpawnDelay = 0.5f;
        
        [Header("Spawn Points")]
        public List<Transform> spawnPoints;
        
        [Header("High Intensity Settings")]
        public float burstSpawnInterval = 0.3f;
        public int burstSize = 2;
    }

    [Header("Wave Configurations")]
    [SerializeField] private List<WaveConfiguration> waveConfigs;
    [SerializeField] private Transform defaultSpawnPoint;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private int currentWaveIndex = 0;
    private int spawnedInCurrentWave = 0;
    private bool isSpawning = false;

    public bool IsSpawningComplete => currentWaveIndex >= waveConfigs.Count;
    public int TotalWaves => waveConfigs.Count;
    public int CurrentWaveIndex => currentWaveIndex;

    private void Start()
    {
        if (enableDebugLogs)
        {
            Debug.Log($"🏭 EnemyShip2Spawner initialized with {waveConfigs.Count} wave configurations");
        }
    }

    public void StartAllWaves()
    {
        if (waveConfigs.Count == 0)
        {
            Debug.LogWarning("⚠️ No wave configurations found in EnemyShip2Spawner!");
            return;
        }

        StartCoroutine(ExecuteAllWaves());
    }

    public void StartWave(int waveIndex)
    {
        if (waveIndex < 0 || waveIndex >= waveConfigs.Count)
        {
            Debug.LogWarning($"⚠️ Invalid wave index: {waveIndex}. Available waves: 0-{waveConfigs.Count - 1}");
            return;
        }

        currentWaveIndex = waveIndex;
        StartCoroutine(ExecuteWave(waveConfigs[waveIndex]));
    }

    private IEnumerator ExecuteAllWaves()
    {
        for (int i = 0; i < waveConfigs.Count; i++)
        {
            currentWaveIndex = i;
            yield return StartCoroutine(ExecuteWave(waveConfigs[i]));
            
            // Wait a bit between waves
            if (i < waveConfigs.Count - 1)
            {
                yield return new WaitForSeconds(2f);
            }
        }

        if (enableDebugLogs)
        {
            Debug.Log("✅ All EnemyShip2 waves completed!");
        }
    }

    private IEnumerator ExecuteWave(WaveConfiguration config)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"🌊 Starting EnemyShip2 wave: {config.waveName} ({config.waveType})");
        }

        isSpawning = true;
        spawnedInCurrentWave = 0;

        // Initial delay
        yield return new WaitForSeconds(config.initialDelay);

        switch (config.waveType)
        {
            case EnemyShip2.WaveType.FastAttack:
                yield return StartCoroutine(SpawnFastAttackWave(config));
                break;
            case EnemyShip2.WaveType.Formation:
                yield return StartCoroutine(SpawnFormationWave(config));
                break;
            case EnemyShip2.WaveType.HighIntensity:
                yield return StartCoroutine(SpawnHighIntensityWave(config));
                break;
        }

        isSpawning = false;

        if (enableDebugLogs)
        {
            Debug.Log($"✅ Completed EnemyShip2 wave: {config.waveName}");
        }
    }

    private IEnumerator SpawnFastAttackWave(WaveConfiguration config)
    {
        for (int i = 0; i < config.enemyCount; i++)
        {
            SpawnEnemy(config, EnemyShip2.WaveType.FastAttack, 0);
            spawnedInCurrentWave++;
            
            yield return new WaitForSeconds(config.spawnInterval);
        }
    }

    private IEnumerator SpawnFormationWave(WaveConfiguration config)
    {
        int formationsToSpawn = Mathf.CeilToInt((float)config.enemyCount / config.formationSize);
        
        for (int formation = 0; formation < formationsToSpawn; formation++)
        {
            int enemiesInThisFormation = Mathf.Min(config.formationSize, config.enemyCount - spawnedInCurrentWave);
            
            // Spawn formation simultaneously
            for (int i = 0; i < enemiesInThisFormation; i++)
            {
                SpawnEnemy(config, EnemyShip2.WaveType.Formation, i);
                spawnedInCurrentWave++;
                
                yield return new WaitForSeconds(config.formationSpawnDelay);
            }
            
            // Wait before next formation
            if (formation < formationsToSpawn - 1)
            {
                yield return new WaitForSeconds(config.spawnInterval);
            }
        }
    }

    private IEnumerator SpawnHighIntensityWave(WaveConfiguration config)
    {
        int burstsToSpawn = Mathf.CeilToInt((float)config.enemyCount / config.burstSize);
        
        for (int burst = 0; burst < burstsToSpawn; burst++)
        {
            int enemiesInThisBurst = Mathf.Min(config.burstSize, config.enemyCount - spawnedInCurrentWave);
            
            // Spawn burst rapidly
            for (int i = 0; i < enemiesInThisBurst; i++)
            {
                SpawnEnemy(config, EnemyShip2.WaveType.HighIntensity, i);
                spawnedInCurrentWave++;
                
                yield return new WaitForSeconds(config.burstSpawnInterval);
            }
            
            // Wait before next burst
            if (burst < burstsToSpawn - 1)
            {
                yield return new WaitForSeconds(config.spawnInterval);
            }
        }
    }

    private void SpawnEnemy(WaveConfiguration config, EnemyShip2.WaveType waveType, int formationIndex)
    {
        Transform spawnPoint = GetSpawnPoint(config);
        
        if (config.enemyPrefab == null)
        {
            Debug.LogWarning($"⚠️ Enemy prefab is null for wave: {config.waveName}");
            return;
        }

        GameObject enemy = Instantiate(config.enemyPrefab, spawnPoint.position, Quaternion.identity);
        EnemyShip2 enemyScript = enemy.GetComponent<EnemyShip2>();
        
        if (enemyScript != null)
        {
            enemyScript.ConfigureWave(waveType, formationIndex);
        }
        else
        {
            Debug.LogWarning($"⚠️ EnemyShip2 component not found on spawned enemy for wave: {config.waveName}");
        }

        if (enableDebugLogs)
        {
            Debug.Log($"🛸 Spawned EnemyShip2: {waveType} at {spawnPoint.position}, formation index: {formationIndex}");
        }
    }

    private Transform GetSpawnPoint(WaveConfiguration config)
    {
        if (config.spawnPoints != null && config.spawnPoints.Count > 0)
        {
            // For formation waves, use spawn points in order
            if (config.waveType == EnemyShip2.WaveType.Formation)
            {
                int index = spawnedInCurrentWave % config.spawnPoints.Count;
                return config.spawnPoints[index];
            }
            // For other waves, use random spawn points
            else
            {
                int randomIndex = Random.Range(0, config.spawnPoints.Count);
                return config.spawnPoints[randomIndex];
            }
        }

        return defaultSpawnPoint != null ? defaultSpawnPoint : transform;
    }

    // Public methods for external control
    public void SetWaveConfiguration(int index, WaveConfiguration newConfig)
    {
        if (index >= 0 && index < waveConfigs.Count)
        {
            waveConfigs[index] = newConfig;
        }
    }

    public WaveConfiguration GetWaveConfiguration(int index)
    {
        if (index >= 0 && index < waveConfigs.Count)
        {
            return waveConfigs[index];
        }
        return null;
    }

    public void AddWaveConfiguration(WaveConfiguration config)
    {
        waveConfigs.Add(config);
    }

    public void StopSpawning()
    {
        StopAllCoroutines();
        isSpawning = false;
        
        if (enableDebugLogs)
        {
            Debug.Log("🛑 EnemyShip2Spawner stopped");
        }
    }

    // Test method for debugging
    [ContextMenu("Test Fast Attack Wave")]
    public void TestFastAttackWave()
    {
        if (waveConfigs.Count > 0)
        {
            WaveConfiguration fastConfig = waveConfigs.Find(config => config.waveType == EnemyShip2.WaveType.FastAttack);
            if (fastConfig != null)
            {
                StartCoroutine(ExecuteWave(fastConfig));
            }
        }
    }

    [ContextMenu("Test Formation Wave")]
    public void TestFormationWave()
    {
        if (waveConfigs.Count > 0)
        {
            WaveConfiguration formationConfig = waveConfigs.Find(config => config.waveType == EnemyShip2.WaveType.Formation);
            if (formationConfig != null)
            {
                StartCoroutine(ExecuteWave(formationConfig));
            }
        }
    }

    [ContextMenu("Test High Intensity Wave")]
    public void TestHighIntensityWave()
    {
        if (waveConfigs.Count > 0)
        {
            WaveConfiguration highIntensityConfig = waveConfigs.Find(config => config.waveType == EnemyShip2.WaveType.HighIntensity);
            if (highIntensityConfig != null)
            {
                StartCoroutine(ExecuteWave(highIntensityConfig));
            }
        }
    }
}
