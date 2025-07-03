using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [Header("Score Display")]
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text newHighScoreText;
    [SerializeField] private TMP_Text levelReachedText;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Effects")]
    [SerializeField] private Animator uiAnimator;

    private void Start()
    {
        DisplayFinalScore();
        SetupButtons();
    }

    private void DisplayFinalScore()
    {
        // Tampilkan level yang dicapai
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

            // Cek apakah skor baru adalah rekor
            if (newHighScoreText != null)
            {
                if (finalScore >= highScore && finalScore > 0)
                {
                    newHighScoreText.gameObject.SetActive(true);
                    newHighScoreText.text = "NEW HIGH SCORE!";

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
    }

    public void RestartGame()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetGameState();
        }

        SceneManager.LoadScene("Level1");
    }

    public void GoToMainMenu()
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetGameState();
        }

        // Putar BGM menu utama
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMainMenuBGM();
        }

        SceneManager.LoadScene(0);
    }
}
