using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    [Header("Energy UI")]
    public Slider energySlider;
    public TMP_Text energyText;
    private float maxEnergy = 100f;

    [Header("Health UI")]
    public Slider healthSlider;
    public TMP_Text healthText;
    private float maxHealth = 100f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // ================= ENERGY =================

    /// <summary>
    /// Set maksimum dan isi penuh energi UI.
    /// </summary>
    public void SetMaxEnergy(float maxEnergy)
    {
        this.maxEnergy = maxEnergy;

        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = maxEnergy;
        }

        UpdateEnergyText(maxEnergy);
    }

    /// <summary>
    /// Perbarui nilai energi di slider dan teks.
    /// </summary>
    public void SetEnergy(float energy)
    {
        if (energySlider != null)
        {
            energySlider.value = Mathf.Clamp(energy, 0f, maxEnergy);
        }

        UpdateEnergyText(energy);
    }

    /// <summary>
    /// Perbarui teks energi.
    /// </summary>
    private void UpdateEnergyText(float current)
    {
        if (energyText != null)
        {
            energyText.text = $"{Mathf.FloorToInt(current)} / {Mathf.FloorToInt(maxEnergy)}";
        }
    }

    // ================= HEALTH =================

    /// <summary>
    /// Set maksimum dan isi penuh health UI.
    /// </summary>
    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }

        UpdateHealthText(maxHealth);
    }

    /// <summary>
    /// Perbarui nilai health di slider dan teks.
    /// </summary>
    public void SetHealth(float health)
    {
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Clamp(health, 0f, maxHealth);
        }

        UpdateHealthText(health);
    }

    /// <summary>
    /// Perbarui teks health.
    /// </summary>
    private void UpdateHealthText(float current)
    {
        if (healthText != null)
        {
            healthText.text = $"{Mathf.FloorToInt(current)} / {Mathf.FloorToInt(maxHealth)}";
        }
    }
}
