using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    [Header("Timing Settings")]
    [SerializeField] private float delayBeforeEnemyWave = 3f;

    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private BoxCollider2D bossBattleArea;
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private TextMeshProUGUI bossHealthText;

    [Header("Audio")]
    [Tooltip("Musik latar yang diputar selama wave musuh biasa.")]
    [SerializeField] private AudioClip levelBGM;
    [Tooltip("Musik latar yang diputar saat bos muncul.")]
    [SerializeField] private AudioClip bossBGM;

    [Header("Level Flow")]
    [Tooltip("Nama scene yang akan dimuat setelah bos dikalahkan. Pastikan nama ini ada di Build Settings.")]
    [SerializeField] private string nextLevelSceneName;
    [SerializeField] private float delayBeforeNextLevel = 3f;

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

        // Sembunyikan BossHealthBar di awal
        if (bossHealthSlider != null)
            bossHealthSlider.gameObject.SetActive(false);
    }


    public void BeginLevel()
    {
        Debug.Log("🟢 BeginLevel dipanggil dari LevelIntroManager!");
        if (AudioManager.instance != null && levelBGM != null)
        {
            AudioManager.instance.ChangeBGM(levelBGM);
        }
        StartCoroutine(BeginWavesAfterDelay());
    }

    private IEnumerator BeginWavesAfterDelay()
    {
        Debug.Log($"⏱ Delay {delayBeforeEnemyWave} detik sebelum wave musuh dimulai...");
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
        Debug.Log($"⏳ Initial delay {wave.initialDelay} detik untuk wave {waveIndex}");
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

            yield return new WaitForSeconds(10f);
            activeEnemies.Clear(); // optional backup

            UIWarningController warningController = FindObjectOfType<UIWarningController>();
            if (warningController != null)
            {
                float warningDuration = 2f;
                warningController.ShowWarning(warningDuration);
                Debug.Log("⚠️ Menampilkan Warning untuk Boss...");
                yield return new WaitForSeconds(warningDuration);
            }
            // Ganti musik ke BGM bos
            if (AudioManager.instance != null && bossBGM != null)
            {
                AudioManager.instance.ChangeBGM(bossBGM);
            }

            if (!bossSpawned && bossPrefab != null && bossSpawnPoint != null)
            {
                Debug.Log("👹 Secret Boss muncul!");
                GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, Quaternion.identity);

                Boss1Controller bossScript = boss.GetComponent<Boss1Controller>();
                if (bossScript != null)
                {
                    bossScript.Initialize(bossBattleArea, bossHealthSlider, bossHealthText);
                }

                bossSpawned = true;
            }

            yield break;
        }

        yield return StartCoroutine(StartWave(currentWaveIndex));
    }

    public void OnBossDefeated()
    {
        Debug.Log("🏆 Boss dikalahkan! Mempersiapkan level selanjutnya...");
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            StartCoroutine(LoadNextLevelAfterDelay());
        }
        else
        {
            Debug.LogWarning("Nama scene level selanjutnya belum diatur di LevelController!");
        }
    }

    private IEnumerator LoadNextLevelAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextLevel);
        SceneManager.LoadScene(nextLevelSceneName);
    }

    private void Spawn(Wave wave)
    {
        if (wave.prefab == null)
        {
            Debug.LogError($"❌ Wave {currentWaveIndex} tidak memiliki prefab yang valid!");
            return;
        }

        Vector2 spawnPosition = RandomSpawnPoint();
        GameObject enemy = Instantiate(wave.prefab, spawnPosition, Quaternion.identity);
        activeEnemies.Add(enemy);

        DespawnTracker tracker = enemy.AddComponent<DespawnTracker>();
        tracker.Init(enemy, this);
    }

    private Vector2 RandomSpawnPoint()
    {
        float randomY = Random.Range(spawn1.position.y, spawn2.position.y);
        return new Vector2(spawn1.position.x, randomY);
    }

    public void OnEnemyDespawn(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log("☠️ Musuh dihapus dari activeEnemies");
        }
    }

    private class DespawnTracker : MonoBehaviour
    {
        private GameObject enemyObj;
        private LevelController levelController;

        public void Init(GameObject enemy, LevelController controller)
        {
            enemyObj = enemy;
            levelController = controller;
        }

        private void OnDestroy()
        {
            if (levelController != null)
                levelController.OnEnemyDespawn(enemyObj);
        }

        private void OnBecameInvisible()
        {
            Debug.Log($"👋 Enemy {gameObject.name} menjadi invisible, dihancurkan.");
            Destroy(gameObject);
        }
    }
}
