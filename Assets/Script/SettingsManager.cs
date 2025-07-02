using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;
    
    [Header("Control Settings")]
    public PlayerController.ControlScheme selectedControlScheme = PlayerController.ControlScheme.Mouse;
    
    private const string CONTROL_SCHEME_KEY = "ControlScheme";
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetControlScheme(PlayerController.ControlScheme scheme)
    {
        selectedControlScheme = scheme;
        SaveSettings();
        
        // Update player control scheme if player exists
        if (PlayerController.instance != null)
        {
            PlayerController.instance.currentScheme = selectedControlScheme;
        }
        
        Debug.Log("Control scheme changed to: " + scheme);
    }
    
    public void SetControlScheme(int schemeIndex)
    {
        PlayerController.ControlScheme scheme = (PlayerController.ControlScheme)schemeIndex;
        SetControlScheme(scheme);
    }
    
    public PlayerController.ControlScheme GetControlScheme()
    {
        return selectedControlScheme;
    }
    
    private void SaveSettings()
    {
        PlayerPrefs.SetInt(CONTROL_SCHEME_KEY, (int)selectedControlScheme);
        PlayerPrefs.Save();
    }
    
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey(CONTROL_SCHEME_KEY))
        {
            selectedControlScheme = (PlayerController.ControlScheme)PlayerPrefs.GetInt(CONTROL_SCHEME_KEY);
        }
    }
    
    // Method to apply settings to player when game starts
    public void ApplySettingsToPlayer()
    {
        if (PlayerController.instance != null)
        {
            PlayerController.instance.currentScheme = selectedControlScheme;
        }
    }
}
