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
    public TMP_Text levelReachedText; // DITAMBAHKAN

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
        // DITAMBAHKAN: Tampilkan level yang dicapai
        if (levelReachedText != null && GameManager.instance != null)
        {
            levelReachedText.text = "Level: " + GameManager.currentLevelIndex;
        }

        if (ScoreManager.instance != null)
        {
            int finalScore = ScoreManager.instance.GetCurrentScore();
            int highScore = ScoreManager.instance.GetHighScore();

            // Menampilkan skor dari game yang baru saja berakhir
            if (finalScoreText != null)
            {
                finalScoreText.text = "Score: " + finalScore.ToString("N0");
            }

            // Menampilkan skor tertinggi sepanjang masa
            if (highScoreText != null)
            {
                highScoreText.text = "High Score: " + highScore.ToString("N0");
            }

            // Check if new high score
            if (newHighScoreText != null)
            {
                if (finalScore >= highScore && finalScore > 0)
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
        // Reset game state (skor & level)
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetGameState();
        }

        // Load the first level (adjust scene name as needed)
        SceneManager.LoadScene("Level1");
    }

    public void GoToMainMenu()
    {
        // Reset game state
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetGameState();
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
