using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

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

    [Header("Dependency")]
    [SerializeField] private ObjectSpawner obstacleSpawner;
    [SerializeField] private float delayBeforeEnemyWave = 3f;

    private int currentWaveIndex = 0;
    private int spawnedInCurrentWave = 0;
    private float spawnTimer = 0f;
    private bool waveActive = false;
    private bool waitingForNextWave = false;
    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(StartAfterObstacleSpawner());
    }

    private IEnumerator StartAfterObstacleSpawner()
    {
        if (obstacleSpawner != null)
        {
            Debug.Log("⏳ Menunggu ObjectSpawner selesai...");
            yield return new WaitUntil(() => obstacleSpawner.finishedSpawning);
            Debug.Log("✅ ObjectSpawner selesai!");
        }

        Debug.Log($"⏱ Delay {delayBeforeEnemyWave} detik sebelum musuh wave dimulai...");
        yield return new WaitForSeconds(delayBeforeEnemyWave);

        waveActive = true;
    }

    private void Update()
    {
        if (!waveActive || waves.Count == 0 || waitingForNextWave) return;

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
            else if (activeEnemies.Count == 0)
            {
                StartCoroutine(WaitAndStartNextWave());
            }
        }
    }

    private void Spawn(Wave wave)
    {
        Vector2 spawnPosition = RandomSpawnPoint();
        GameObject enemy = Instantiate(wave.prefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(enemy);
        EnemyDespawn despawn = enemy.AddComponent<EnemyDespawn>();
        despawn.onDespawn += () => activeEnemies.Remove(enemy);
    }

    private Vector2 RandomSpawnPoint()
    {
        float randomY = Random.Range(spawn1.position.y, spawn2.position.y);
        return new Vector2(spawn1.position.x, randomY);
    }

    private IEnumerator WaitAndStartNextWave()
    {
        waitingForNextWave = true;
        yield return new WaitForSeconds(1f);

        currentWaveIndex++;
        spawnedInCurrentWave = 0;

        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("🚀 Semua wave musuh selesai!");
            waveActive = false;
        }

        waitingForNextWave = false;
    }
}

public class EnemyDespawn : MonoBehaviour
{
    public System.Action onDespawn;

    private void OnDestroy()
    {
        onDespawn?.Invoke();
    }
}
