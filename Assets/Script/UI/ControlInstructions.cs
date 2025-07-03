using UnityEngine;
using UnityEngine.UI;

public class ControlInstructions : MonoBehaviour
{
    [Header("Instruction Texts")]
    public Text keyboardInstructions;
    public Text mouseInstructions;
    
    [Header("Control Panels")]
    public GameObject keyboardPanel;
    public GameObject mousePanel;
    
    private void Start()
    {
        UpdateInstructions();
    }
    
    private void Update()
    {
        // Update instructions if control scheme changes during gameplay
        if (PlayerController.instance != null)
        {
            UpdateInstructions();
        }
    }
    
    private void UpdateInstructions()
    {
        PlayerController.ControlScheme currentScheme = PlayerController.ControlScheme.Mouse;
        
        if (SettingsManager.instance != null)
        {
            currentScheme = SettingsManager.instance.GetControlScheme();
        }
        else if (PlayerController.instance != null)
        {
            currentScheme = PlayerController.instance.currentScheme;
        }
        
        // Show/hide appropriate instruction panels
        if (keyboardPanel != null && mousePanel != null)
        {
            keyboardPanel.SetActive(currentScheme == PlayerController.ControlScheme.Keyboard);
            mousePanel.SetActive(currentScheme == PlayerController.ControlScheme.Mouse);
        }
        
        // Update instruction texts
        if (keyboardInstructions != null)
        {
            keyboardInstructions.text = GetKeyboardInstructions();
        }
        
        if (mouseInstructions != null)
        {
            mouseInstructions.text = GetMouseInstructions();
        }
    }
    
    private string GetKeyboardInstructions()
    {
        return "KEYBOARD CONTROLS:\n" +
               "WASD - Move\n" +
               "Hold Right Click - Boost\n" +
               "Hold Left Click - Shoot";
    }
    
    private string GetMouseInstructions()
    {
        return "MOUSE CONTROLS:\n" +
               "Move Mouse - Move Ship\n" +
               "Hold Right Click - Boost\n" +
               "Hold Left Click - Shoot";
    }
}
