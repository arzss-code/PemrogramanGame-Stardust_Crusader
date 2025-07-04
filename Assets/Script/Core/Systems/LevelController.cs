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
    [Tooltip("Jeda waktu (detik) setelah wave terakhir selesai sebelum bos muncul.")]
    [SerializeField] private float delayBeforeBoss = 10f;

    [Header("Boss Settings")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPoint;
    [SerializeField] private BoxCollider2D bossBattleArea;
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private TextMeshProUGUI bossHealthText;

    // 🔽 Tambahan baru untuk Shield
    [SerializeField] private Slider bossShieldSlider;
    [SerializeField] private TextMeshProUGUI bossShieldText;

    public Slider BossHealthSlider => bossHealthSlider;
    public TextMeshProUGUI BossHealthText => bossHealthText;
    public Slider BossShieldSlider => bossShieldSlider;
    public TextMeshProUGUI BossShieldText => bossShieldText;


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
    private bool levelBegun = false; // DITAMBAHKAN: Flag untuk mencegah pemanggilan ganda

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Sembunyikan BossHealthBar di awal
        if (bossHealthSlider != null)
            bossHealthSlider.gameObject.SetActive(false);

        if (bossShieldSlider != null)
            bossShieldSlider.gameObject.SetActive(false);

        if (bossShieldText != null)
            bossShieldText.gameObject.SetActive(false);
    }

    private void Start()
    {
        // DITAMBAHKAN: Jika tidak ada LevelIntroManager di scene, level akan mulai secara otomatis.
        // Ini membuat level bisa berjalan sendiri tanpa memerlukan cutscene intro.
        if (FindObjectOfType<LevelIntroManager>() == null)
        {
            BeginLevel();
        }
    }

    public void BeginLevel()
    {
        if (levelBegun) return; // Mencegah pemanggilan ganda
        levelBegun = true;

        Debug.Log("🟢 BeginLevel dipanggil! Mengganti BGM ke musik level.");
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
        // DITAMBAHKAN: Jangan proses wave jika game sedang di-pause
        if (PauseManager.instance != null && PauseManager.instance.IsPaused)
        {
            return;
        }

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

            yield return new WaitForSeconds(delayBeforeBoss);
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

                // Coba inisialisasi sebagai Boss1Controller
                var boss1 = boss.GetComponent<Boss1Controller>();
                if (boss1 != null)
                {
                    boss1.Initialize(bossBattleArea, bossHealthSlider, bossHealthText);
                }

                // Coba inisialisasi sebagai Boss2Controller
                var boss2 = boss.GetComponent<Boss2Controller>();
                if (boss2 != null)
                {
                    boss2.Initialize(bossBattleArea, bossHealthSlider, bossHealthText);
                }

                // Jika tidak ada skrip bos yang cocok, beri peringatan.
                if (boss1 == null && boss2 == null)
                {
                    Debug.LogWarning($"Prefab bos '{bossPrefab.name}' tidak memiliki skrip Boss1Controller atau Boss2Controller yang bisa diinisialisasi.", boss);
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
        // Panggil GameManager untuk menangani logika penyelesaian level setelah jeda.
        // Ini memastikan semua logika (skor, kenaikan level) dijalankan secara terpusat.
        StartCoroutine(CompleteLevelAfterDelay());
    }

    /// <summary>
    /// Menunggu beberapa detik sebelum memanggil GameManager untuk menyelesaikan level.
    /// </summary>
    private IEnumerator CompleteLevelAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextLevel);
        if (GameManager.instance != null)
        {
            GameManager.instance.LevelCompleted();
        }
        else
        {
            Debug.LogError("GameManager.instance tidak ditemukan! Tidak bisa melanjutkan ke level berikutnya.");
        }
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
