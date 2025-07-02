using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menus : MonoBehaviour
{
    [Header("Control Settings UI")]
    public GameObject settingsPanel;
    public Dropdown controlSchemeDropdown;
    public Button settingsButton;
    public Button backButton;
    
    private void Start()
    {
        // Initialize dropdown if it exists
        if (controlSchemeDropdown != null)
        {
            SetupControlDropdown();
        }
        
        // Hide settings panel initially
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    
    private void SetupControlDropdown()
    {
        // Clear existing options
        controlSchemeDropdown.ClearOptions();
        
        // Add control scheme options
        controlSchemeDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Keyboard",
            "Mouse"
        });
        
        // Set current value based on saved settings
        if (SettingsManager.instance != null)
        {
            controlSchemeDropdown.value = (int)SettingsManager.instance.GetControlScheme();
        }
        
        // Add listener for when value changes
        controlSchemeDropdown.onValueChanged.AddListener(OnControlSchemeChanged);
    }
    
    // Panggil fungsi ini saat tombol Play ditekan
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1"); // Ganti dengan nama scene utama kamu
    }

    // Panggil fungsi ini saat tombol Settings ditekan
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }
    
    // Panggil fungsi ini saat tombol Back ditekan di settings
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
    
    // Panggil fungsi ini saat control scheme dropdown berubah
    public void OnControlSchemeChanged(int value)
    {
        if (SettingsManager.instance != null)
        {
            SettingsManager.instance.SetControlScheme(value);
        }
    }

    // Panggil fungsi ini saat tombol Quit ditekan
    public void QuitGame()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();
    }
}
