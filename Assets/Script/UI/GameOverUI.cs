using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Score Display")]
    public TMP_Text finalScoreText;
    public TMP_Text highScoreText;
    public TMP_Text newHighScoreText;
    
    [Header("Buttons")]
    public Button restartButton;
    public Button mainMenuButton;
    public Button quitButton;
    
    [Header("Effects")]
    public Animator uiAnimator;
    
    private void Start()
    {
        DisplayFinalScore();
        SetupButtons();
    }
    
    private void DisplayFinalScore()
    {
        if (ScoreManager.instance != null)
        {
            int finalScore = ScoreManager.instance.GetCurrentScore();
            int highScore = ScoreManager.instance.GetHighScore();
            
            // Display final score
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + finalScore.ToString("N0");
            }
            
            // Display high score
            if (highScoreText != null)
            {
                highScoreText.text = "High Score: " + highScore.ToString("N0");
            }
            
            // Check if new high score
            if (newHighScoreText != null)
            {
                if (finalScore >= highScore)
                {
                    newHighScoreText.gameObject.SetActive(true);
                    newHighScoreText.text = "NEW HIGH SCORE!";
                    
                    // Trigger special animation if available
                    if (uiAnimator != null)
                    {
                        uiAnimator.SetTrigger("NewHighScore");
                    }
                }
                else
                {
                    newHighScoreText.gameObject.SetActive(false);
                }
            }
        }
    }
    
    private void SetupButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }
    
    public void RestartGame()
    {
        // Reset score before restarting
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }
        
        // Load the first level (adjust scene name as needed)
        SceneManager.LoadScene("Level1");
    }
    
    public void GoToMainMenu()
    {
        // Reset score
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }
        
        // Load main menu (adjust scene name/index as needed)
        SceneManager.LoadScene(0);
    }
    
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
