using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance; // Instance singleton untuk akses global

    [Header("Energy UI")]
    public Slider energySlider; // Slider untuk menampilkan energi
    public TMP_Text energyText; // Teks untuk menampilkan nilai energi
    private float maxEnergy = 100f; // Nilai maksimum energi

    [Header("Health UI")]
    public Slider healthSlider; // Slider untuk menampilkan kesehatan
    public TMP_Text healthText; // Teks untuk menampilkan nilai kesehatan
    private float maxHealth = 100f; // Nilai maksimum kesehatan

    private void Awake()
    {
        // Implementasi singleton
        if (instance == null)
            instance = this; // Set instance jika belum ada
        else
            Destroy(gameObject); // Hancurkan objek jika instance sudah ada
    }

    // ================= ENERGY =================

    /// <summary>
    /// Set maksimum dan isi penuh energi UI.
    /// </summary>
    public void SetMaxEnergy(float maxEnergy)
    {
        this.maxEnergy = maxEnergy; // Set nilai maksimum energi

        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy; // Set nilai maksimum slider
            energySlider.value = maxEnergy; // Isi slider dengan nilai maksimum
        }

        UpdateEnergyText(maxEnergy); // Perbarui teks energi
    }

    /// <summary>
    /// Perbarui nilai energi di slider dan teks.
    /// </summary>
    public void SetEnergy(float energy)
    {
        if (energySlider != null)
        {
            // Set nilai slider dengan energi saat ini, dibatasi antara 0 dan maxEnergy
            energySlider.value = Mathf.Clamp(energy, 0f, maxEnergy);
        }

        UpdateEnergyText(energy); // Perbarui teks energi
    }

    /// <summary>
    /// Perbarui teks energi.
    /// </summary>
    private void UpdateEnergyText(float current)
    {
        if (energyText != null)
        {
            // Set teks energi dengan format "current / maxEnergy"
            energyText.text = $"{Mathf.FloorToInt(current)} / {Mathf.FloorToInt(maxEnergy)}";
        }
    }

    // ================= HEALTH =================

    /// <summary>
    /// Set maksimum dan isi penuh health UI.
    /// </summary>
    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth; // Set nilai maksimum kesehatan

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth; // Set nilai maksimum slider
            healthSlider.value = maxHealth; // Isi slider dengan nilai maksimum
        }

        UpdateHealthText(maxHealth); // Perbarui teks kesehatan
    }

    /// <summary>
    /// Perbarui nilai health di slider dan teks.
    /// </summary>
    public void SetHealth(float health)
    {
        if (healthSlider != null)
        {
            // Set nilai slider dengan kesehatan saat ini, dibatasi antara 0 dan maxHealth
            healthSlider.value = Mathf.Clamp(health, 0f, maxHealth);
        }

        UpdateHealthText(health); // Perbarui teks kesehatan
    }

    /// <summary>
    /// Perbarui teks health.
    /// </summary>
    private void UpdateHealthText(float current)
    {
        if (healthText != null)
        {
            // Set teks kesehatan dengan format "current / maxHealth"
            healthText.text = $"{Mathf.FloorToInt(current)} / {Mathf.FloorToInt(maxHealth)}";
        }
    }
}