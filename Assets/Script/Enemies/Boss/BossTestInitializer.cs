// ✅ BENAR: Direktif ini wajib ada agar Unity mengenali tipe data TextMeshProUGUI.
using TMPro;
using UnityEngine;
// ✅ BENAR: Direktif ini wajib ada agar Unity mengenali tipe data Slider.
using UnityEngine.UI;

/// <summary>
/// Skrip ini berfungsi untuk menginisialisasi Boss AI dalam sebuah scene pengetesan
/// tanpa memerlukan LevelController.
/// </summary>
public class BossTestInitializer : MonoBehaviour
{
    [Header("Referensi Wajib")]
    [Tooltip("Pilih jenis Boss yang akan diuji.")]
    [SerializeField] private BossType bossType;

    [Tooltip("Seret prefab atau objek Boss dari scene ke sini.")]
    [SerializeField] private GameObject bossPrefab;

    [Tooltip("Seret objek yang memiliki BoxCollider2D sebagai area pergerakan bos.")]
    [SerializeField] private BoxCollider2D battleArea;

    [Header("Referensi Opsional (UI)")]
    [SerializeField] private Slider healthSlider;

    // ✅ BENAR: Tipe data yang digunakan adalah 'TextMeshProUGUI'.
    // ❌ SALAH (yang umum terjadi): Mengetik 'TextMeshProUGT' atau hanya 'Text'.
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Slider shieldSlider; // Tambahan untuk Boss 3

    // ✅ BENAR: Tipe data di sini juga sudah tepat.
    [SerializeField] private TextMeshProUGUI shieldText; // Tambahan untuk Boss 3

    private enum BossType
    {
        Boss1,
        Boss2,
        Boss3
    }

    void Start()
    {
        if (bossPrefab == null || battleArea == null)
        {
            Debug.LogError("Boss Prefab atau Battle Area belum di-set di Inspector BossTestInitializer!", this);
            return;
        }

        GameObject bossInstance = Instantiate(bossPrefab, transform.position, Quaternion.identity);
        InitializeBoss(bossInstance);
    }

    private void InitializeBoss(GameObject bossInstance)
    {
        switch (bossType)
        {
            case BossType.Boss1:
                var boss1 = bossInstance.GetComponent<Boss1Controller>();
                if (boss1 != null)
                {
                    // ✅ BENAR: Memanggil Initialize dengan 3 parameter yang sesuai untuk Boss1.
                    boss1.Initialize(battleArea, healthSlider, healthText);
                    Debug.Log("Boss 1 AI Initialized for testing.");
                }
                break;
            case BossType.Boss2:
                var boss2 = bossInstance.GetComponent<Boss2Controller>();
                if (boss2 != null)
                {
                    // ✅ BENAR: Memanggil Initialize dengan 3 parameter yang sesuai untuk Boss2.
                    boss2.Initialize(battleArea, healthSlider, healthText);
                    Debug.Log("Boss 2 AI Initialized for testing.");
                }
                break;
            case BossType.Boss3:
                var boss3 = bossInstance.GetComponent<Boss3Controller>();
                if (boss3 != null)
                {
                    // ✅ BENAR: Memanggil Initialize dengan 5 parameter yang sesuai untuk Boss3.
                    boss3.Initialize(battleArea, healthSlider, healthText, shieldSlider, shieldText);
                    Debug.Log("Boss 3 AI Initialized for testing.");
                }
                break;
            default:
                Debug.LogError("Tipe Boss tidak valid!", this);
                break;
        }
    }
}
