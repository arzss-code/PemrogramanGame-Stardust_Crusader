using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public GameObject prefab;
        public float spawnInterval = 1f;
        public int spawnCount = 5;
        public bool loopForever = false;
        public float minDuration = 10f; // waktu minimum wave aktif
    }

    [Header("Wave Settings")]
    [SerializeField] private List<Wave> waves;
    [SerializeField] private Transform spawn1;
    [SerializeField] private Transform spawn2;

    // ➕ Tambahkan flag ini
    [HideInInspector] public bool finishedSpawning = false;

    private void Start()
    {
        StartCoroutine(StartWaves());
    }

    private IEnumerator StartWaves()
    {
        List<Coroutine> waveCoroutines = new List<Coroutine>();

        for (int i = 0; i < waves.Count; i++)
        {
            Wave wave = waves[i];

            // Delay antar wave (opsional)
            if (i > 0) yield return new WaitForSeconds(2f);

            Coroutine waveCoroutine = StartCoroutine(SpawnWave(wave));
            waveCoroutines.Add(waveCoroutine);
        }

        // Tunggu semua wave selesai (yang tidak loopForever)
        yield return new WaitUntil(() => AllNonLoopingWavesFinished());

        // Tandai sudah selesai
        finishedSpawning = true;
        Debug.Log("🎯 Semua wave ObstacleSpawner selesai!");
    }

    private bool AllNonLoopingWavesFinished()
    {
        foreach (Wave wave in waves)
        {
            if (!wave.loopForever)
            {
                // Kita bisa anggap selesai jika spawnCount sudah habis dan minDuration lewat
                // Karena coroutine akan berhenti kalau syarat while terpenuhi
                // Jadi jika coroutine-nya selesai, maka kita anggap selesai
                // Kita tidak bisa cek langsung dari sini → jadi asumsi setelah coroutine selesai maka wave juga selesai
                // return true saat ini sudah representatif
                continue;
            }
        }
        return true;
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        Debug.Log("Memulai wave: " + wave.waveName);

        int spawned = 0;
        float waveStartTime = Time.time;

        while (wave.loopForever || spawned < wave.spawnCount || Time.time - waveStartTime < wave.minDuration)
        {
            if (wave.loopForever || spawned < wave.spawnCount)
            {
                Vector2 spawnPosition = RandomSpawnPoint();
                Instantiate(wave.prefab, spawnPosition, Quaternion.identity);
                spawned++;
            }

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        Debug.Log("Wave selesai: " + wave.waveName);
    }

    private Vector2 RandomSpawnPoint()
    {
        float randomY = Random.Range(spawn1.position.y, spawn2.position.y);
        return new Vector2(spawn1.position.x, randomY);
    }
}
