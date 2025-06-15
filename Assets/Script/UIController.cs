using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public Slider energySlider;
    public TMP_Text energyText;

    private float maxEnergy = 100f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

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

    public void SetEnergy(float energy)
    {
        if (energySlider != null)
        {
            energySlider.value = Mathf.Clamp(energy, 0f, energySlider.maxValue);
        }

        UpdateEnergyText(energy);
    }

    private void UpdateEnergyText(float current)
    {
        if (energyText != null)
        {
            energyText.text = $"{Mathf.FloorToInt(current)} / {Mathf.FloorToInt(maxEnergy)}";
        }
    }
}
