using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Skrip ini berfungsi untuk menginisialisasi Boss AI dalam sebuah scene pengetesan
/// tanpa memerlukan LevelController.
/// </summary>
public class BossTestInitializer : MonoBehaviour
{
    [Header("Referensi Wajib")]
    [Tooltip("Seret prefab atau objek Boss1 dari scene ke sini.")]
    [SerializeField] private Boss1Controller bossToInitialize;

    [Tooltip("Seret objek yang memiliki BoxCollider2D sebagai area pergerakan bos.")]
    [SerializeField] private BoxCollider2D battleArea;

    [Header("Referensi Opsional (UI)")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    void Start()
    {
        if (bossToInitialize == null || battleArea == null)
        {
            Debug.LogError("Boss atau Battle Area belum di-set di Inspector BossTestInitializer!", this);
            return;
        }

        // Panggil method Initialize pada bos untuk mengaktifkan AI-nya.
        bossToInitialize.Initialize(battleArea, healthSlider, healthText);
        Debug.Log("Boss AI Initialized for testing.");
    }
}