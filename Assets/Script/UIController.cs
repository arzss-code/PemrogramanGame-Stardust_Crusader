using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public Slider energySlider;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetMaxEnergy(float maxEnergy)
    {
        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = maxEnergy;
        }
    }

    public void SetEnergy(float energy)
    {
        if (energySlider != null)
        {
            energySlider.value = Mathf.Clamp(energy, 0f, energySlider.maxValue);
        }
    }
}
