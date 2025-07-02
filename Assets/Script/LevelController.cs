using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

    [System.Serializable]
    public class Wave
    {
        public GameObject prefab;
        public float spawnInterval = 1f;
        public int spawnCount = 5;
        public float initialDelay = 0f;
    }

    [Header("Wave Settings")]
    [SerializeField] private List<Wave> waves;
    [SerializeField] private Transform spawn1;
    [SerializeField] private Transform spawn2;

    [Header("Dependency")]
    [SerializeField] private ObjectSpawner obstacleSpawner;
    [SerializeField] private float delayBeforeEnemyWave = 3f;

    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private BoxCollider2D bossBattleArea;
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private TextMeshProUGUI bossHealthText;
    private bool bossSpawned = false;

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
        if (bossHealthSlider != null)
            bossHealthSlider.gameObject.SetActive(false); // Optional extra safety

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

        Debug.Log($"⏱ Delay {delayBeforeEnemyWave} detik sebelum wave pertama dimulai...");
        yield return new WaitForSeconds(delayBeforeEnemyWave);

        StartCoroutine(StartWave(currentWaveIndex));
    }

    private void Update()
    {
        if (!waveActive || waitingForNextWave || currentWaveIndex >= waves.Count) return;

        Wave currentWave = waves[currentWaveIndex];
        spawnTimer += Time.deltaTime;

        if (spawnedInCurrentWave < currentWave.spawnCount && spawnTimer >= currentWave.spawnInterval)
        {
            spawnTimer = 0f;
            Spawn(currentWave);
            spawnedInCurrentWave++;
        }
        else if (spawnedInCurrentWave >= currentWave.spawnCount && activeEnemies.Count == 0)
        {
            StartCoroutine(WaitAndStartNextWave());
        }
    }

    private IEnumerator StartWave(int waveIndex)
    {
        if (waveIndex >= waves.Count) yield break;

        Wave wave = waves[waveIndex];
        yield return new WaitForSeconds(wave.initialDelay);

        Debug.Log($"🚀 Memulai wave {waveIndex}");
        waveActive = true;
        waitingForNextWave = false;
        spawnTimer = 0f;
        spawnedInCurrentWave = 0;
    }

    private IEnumerator WaitAndStartNextWave()
    {
        waitingForNextWave = true;
        waveActive = false;

        currentWaveIndex++;
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("✅ Semua wave selesai! Menunggu musuh terakhir dihancurkan...");

            yield return new WaitUntil(() => activeEnemies.Count == 0);

            UIWarningController warningController = FindObjectOfType<UIWarningController>();
            if (warningController != null)
            {
                float warningDuration = 2f;
                warningController.ShowWarning(warningDuration);
                Debug.Log("⚠️ Menampilkan Warning untuk Boss...");
                yield return new WaitForSeconds(warningDuration);
            }

            if (!bossSpawned && bossPrefab != null && bossSpawnPoint != null)
            {
                Debug.Log("👹 Secret Boss muncul!");
                GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

                Boss1Controller bossScript = boss.GetComponent<Boss1Controller>();
                if (bossScript != null)
                {
                    bossScript.battleArea = bossBattleArea;
                    bossScript.bossHealthSlider = bossHealthSlider;
                    bossScript.bossHealthText = bossHealthText;
                }

                bossSpawned = true;
            }

            yield break;
        }

        yield return StartCoroutine(StartWave(currentWaveIndex));
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
}

public class EnemyDespawn : MonoBehaviour
{
    public System.Action onDespawn;

    private void OnDestroy()
    {
        onDespawn?.Invoke();
    }
}
