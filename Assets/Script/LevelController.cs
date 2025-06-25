using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Wave
{
    public string waveName;
    public GameObject enemyPrefab; // DIPERBAIKI: Menambahkan variabel prefab musuh
    public int enemyCount;
    public float spawnInterval;
}

public class LevelController : MonoBehaviour
{
    public static LevelController main;

    [Header("Wave Settings")]
    public Wave[] waves;

    [Header("Enemy Spawn Settings")] // DITAMBAHKAN: Header baru
    [Tooltip("Area di mana musuh akan muncul. Buat Empty GameObject dengan BoxCollider2D.")]
    public BoxCollider2D enemySpawnArea; // DITAMBAHKAN: Variabel untuk area spawn

    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public Transform bossBattlePosition;

    [Header("Boss UI References")]
    public Slider bossHealthBarSlider;

    private int currentWaveIndex = 0;
    private int enemiesRemaining;
    private bool bossHasSpawned = false;

    private void Awake()
    {
        if (main == null) { main = this; }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        if (bossHealthBarSlider != null)
        {
            bossHealthBarSlider.gameObject.SetActive(false);
        }
        StartCoroutine(SpawnWave(waves[currentWaveIndex]));
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log("Memulai Gelombang: " + wave.waveName);
        enemiesRemaining = wave.enemyCount;

        // Cek apakah prefab musuh sudah diatur untuk menghindari error
        if (wave.enemyPrefab == null)
        {
            Debug.LogError("Enemy Prefab untuk gelombang '" + wave.waveName + "' belum diatur di LevelController!");
            yield break; // Hentikan coroutine jika prefab kosong
        }

        for (int i = 0; i < wave.enemyCount; i++)
        {
            // --- BAGIAN INI DIIMPLEMENTASIKAN ---
            // Tentukan posisi spawn acak di dalam area
            Bounds bounds = enemySpawnArea.bounds;
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            Vector2 spawnPosition = new Vector2(randomX, randomY);

            // Munculkan musuh
            Instantiate(wave.enemyPrefab, spawnPosition, Quaternion.identity);
            // ------------------------------------

            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    public void EnemyDefeated()
    {
        if (bossHasSpawned) return;

        enemiesRemaining--;
        Debug.Log("Musuh dikalahkan! Sisa: " + enemiesRemaining);

        if (enemiesRemaining <= 0)
        {
            currentWaveIndex++;
            if (currentWaveIndex < waves.Length)
            {
                StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            }
            else
            {
                SpawnBoss();
            }
        }
    }

    private void SpawnBoss()
    {
        Debug.Log("SEMUA GELOMBANG SELESAI! BOSS MUNCUL!");
        bossHasSpawned = true;

        if (bossPrefab == null)
        {
            Debug.LogError("Boss Prefab belum diatur di LevelController!");
            return;
        }

        GameObject bossObject = Instantiate(bossPrefab, bossSpawnPoint.position, bossPrefab.transform.rotation);
        BossController bossController = bossObject.GetComponent<BossController>();

        if (bossController != null)
        {
            bossController.Initialize(bossBattlePosition.position);
            bossController.healthBarSlider = this.bossHealthBarSlider;
            if (bossHealthBarSlider != null)
            {
                bossController.healthText = bossHealthBarSlider.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }
}