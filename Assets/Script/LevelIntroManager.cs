using System.Collections;
using UnityEngine;

public class LevelIntroManager : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public Transform playerTransform;
    public Transform startPoint; // Posisi awal cutscene
    public Transform endPoint;   // Posisi akhir cutscene
    public float introDuration = 2f;

    [Header("Spawner Reference")]
    public ObjectSpawner objectSpawner;
    public ItemSpawner itemSpawner;

    private void Start()
    {
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("❌ StartPoint atau EndPoint belum di-assign di Inspector.");
            yield break;
        }

        Vector3 startPos = startPoint.position;
        Vector3 endPos = endPoint.position;

        // Pindahkan player ke posisi awal cutscene
        playerTransform.position = startPos;

        // Nonaktifkan kontrol player
        if (PlayerController.instance != null)
            PlayerController.instance.isInCutscene = true;

        float elapsed = 0f;
        while (elapsed < introDuration)
        {
            playerTransform.position = Vector3.Lerp(startPos, endPos, elapsed / introDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Pastikan posisi akhir tercapai
        playerTransform.position = endPos;

        // Hentikan semua gerakan rigidbody
        Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // Aktifkan kembali kontrol player
        if (PlayerController.instance != null)
            PlayerController.instance.isInCutscene = false;

        // Mulai obstacle & item spawner jika ada
        objectSpawner?.BeginSpawning();
        itemSpawner?.StartSpawning();

        // Mulai musuh (wave level)
        if (LevelController.Instance != null)
        {
            LevelController.Instance.BeginLevel();
            Debug.Log("✅ Level dimulai setelah cutscene dan spawner.");
        }
        else
        {
            Debug.LogWarning("⚠️ LevelController.Instance tidak ditemukan!");
        }

        Debug.Log("🎬 Cutscene selesai. Player siap dikontrol.");
    }
}
