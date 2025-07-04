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

    [Header("UI Elements")]
    [Tooltip("Teks untuk menampilkan informasi wave (misal: 'Wave 1 / 3'). Opsional, bisa dikosongkan.")]
    [SerializeField] private TextMeshProUGUI waveInfoText;
    [Tooltip("Durasi (detik) wave popup ditampilkan sebelum menghilang.")]
    [SerializeField] private float wavePopupDuration = 3.0f;

    [Header("UI Messages")]
    [Tooltip("Nomor level saat ini untuk ditampilkan di popup.")]
    [SerializeField] private int currentLevel = 1;
    [Tooltip("Pesan deskriptif untuk setiap wave. Jika kosong, akan menggunakan pesan default.")]
    [SerializeField] private string[] waveMessages;
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private TextMeshProUGUI bossHealthText;
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
    [Tooltip("Apakah ini level terakhir? Jika ya, akan menampilkan Win Panel, bukan pindah scene.")]
    [SerializeField] private bool isFinalLevel = false;
    [Tooltip("Prefab Win Panel yang akan muncul jika ini adalah level terakhir.")]
    [SerializeField] private GameObject winPanelPrefab;
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

        if (waveInfoText != null)
            waveInfoText.gameObject.SetActive(false);
        else
        {
            // Jika referensi UI tidak ada, coba buat secara otomatis.
            Debug.Log("Wave Info Text tidak di-assign. Mencoba membuat secara otomatis...");
            CreateWaveInfoText();
        }
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

        if (waveInfoText != null) StartCoroutine(ShowWavePopup(waveIndex));

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

                // Coba inisialisasi sebagai Boss3Controller
                var boss3 = boss.GetComponent<Boss3Controller>();
                if (boss3 != null)
                {
                    boss3.Initialize(bossBattleArea, bossHealthSlider, bossHealthText, bossShieldSlider, bossShieldText);
                }

                // Jika tidak ada skrip bos yang cocok, beri peringatan.
                if (boss1 == null && boss2 == null && boss3 == null)
                {
                    Debug.LogWarning($"Prefab bos '{bossPrefab.name}' tidak memiliki skrip Boss1Controller, Boss2Controller, atau Boss3Controller yang bisa diinisialisasi.", boss);
                }

                bossSpawned = true;
            }

            yield break;
        }

        yield return StartCoroutine(StartWave(currentWaveIndex));
    }

    /// <summary>
    /// Menampilkan popup informasi wave untuk sementara dengan efek fade.
    /// </summary>
    private IEnumerator ShowWavePopup(int waveIndex)
    {
        // Set teks dan aktifkan objek
        waveInfoText.text = GetWaveMessage(waveIndex);
        waveInfoText.gameObject.SetActive(true);

        // Efek fade-in
        Color originalColor = waveInfoText.color;
        waveInfoText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        float fadeInTime = 0.5f;
        for (float t = 0; t < fadeInTime; t += Time.unscaledDeltaTime)
        {
            waveInfoText.color = new Color(originalColor.r, originalColor.g, originalColor.b, t / fadeInTime);
            yield return null;
        }
        waveInfoText.color = originalColor;

        // Tunggu selama durasi yang ditentukan
        yield return new WaitForSeconds(wavePopupDuration);

        // Efek fade-out
        float fadeOutTime = 0.5f;
        for (float t = 0; t < fadeOutTime; t += Time.unscaledDeltaTime)
        {
            waveInfoText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - (t / fadeOutTime));
            yield return null;
        }

        // Sembunyikan popup dan reset warna untuk penggunaan selanjutnya
        waveInfoText.gameObject.SetActive(false);
        waveInfoText.color = originalColor;
    }

    /// <summary>
    /// Membuat pesan yang akan ditampilkan pada wave popup.
    /// </summary>
    private string GetWaveMessage(int waveIndex)
    {
        string levelText = $"LEVEL {currentLevel}";
        string waveText = $"WAVE {waveIndex + 1}";
        string description = "";

        if (waveMessages != null && waveIndex < waveMessages.Length && !string.IsNullOrEmpty(waveMessages[waveIndex]))
        {
            description = waveMessages[waveIndex];
        }
        else
        {
            description = "ENEMY APPROACHING";
        }

        return $"{levelText}\n{waveText} {description}";
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

        if (isFinalLevel)
        {
            // Ini adalah level terakhir, berikan skor bonus dan tampilkan Win Panel
            Debug.Log("🏆 GAME TAMAT! Menampilkan Win Panel.");

            // Berikan skor bonus penyelesaian level
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddLevelCompleteScore();
            }

            if (winPanelPrefab != null)
            {
                Instantiate(winPanelPrefab);
                // Jeda game dan musik
                Time.timeScale = 0f;
                if (AudioManager.instance != null && AudioManager.instance.bgmSource.isPlaying)
                {
                    AudioManager.instance.bgmSource.Pause();
                }
            }
            else
            {
                Debug.LogError("isFinalLevel is true, tapi winPanelPrefab belum di-assign di LevelController!", this);
            }
        }
        else
        {
            // Ini bukan level terakhir, lanjutkan ke level berikutnya via GameManager
            if (GameManager.instance != null)
            {
                GameManager.instance.LevelCompleted();
            }
            else
            {
                Debug.LogError("GameManager.instance tidak ditemukan! Tidak bisa melanjutkan ke level berikutnya.");
            }
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

    /// <summary>
    /// Membuat objek TextMeshPro untuk wave info secara otomatis jika tidak di-assign di Inspector.
    /// </summary>
    private void CreateWaveInfoText()
    {
        // 1. Cari Canvas yang ada di scene.
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Tidak dapat membuat Wave Info Text secara otomatis karena tidak ada Canvas di scene.", this);
            return;
        }

        // 2. Buat GameObject baru untuk teks.
        GameObject textObject = new GameObject("WaveInfoText_AutoCreated");
        textObject.transform.SetParent(canvas.transform, false); // Set parent ke canvas.

        // 3. Tambahkan dan konfigurasikan komponen TextMeshProUGUI.
        waveInfoText = textObject.AddComponent<TextMeshProUGUI>();
        waveInfoText.text = ""; // Teks awal kosong.
        waveInfoText.fontSize = 48;
        waveInfoText.fontStyle = FontStyles.Bold;
        waveInfoText.alignment = TextAlignmentOptions.Center;
        waveInfoText.color = Color.white;

        // 4. Atur posisi dan ukuran menggunakan RectTransform.
        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f); // Anchor ke tengah
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);     // Pivot di tengah
        rectTransform.anchoredPosition = new Vector2(0, 0);   // Posisi di tengah anchor (center-middle)
        rectTransform.sizeDelta = new Vector2(600, 150); // Lebar 600px, tinggi 150px (untuk mengakomodasi 2 baris teks)

        // Sembunyikan di awal.
        waveInfoText.gameObject.SetActive(false);
    }
}
