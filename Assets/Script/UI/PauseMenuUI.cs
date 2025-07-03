using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;

    private void Start()
    {
        // Tambahkan listener ke tombol
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitToMainMenu);
        }

        // Isi UI dengan informasi game saat ini
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        if (GameManager.instance != null && levelText != null)
        {
            levelText.text = "Level: " + GameManager.currentLevelIndex;
        }

        if (ScoreManager.instance != null)
        {
            if (scoreText != null)
            {
                scoreText.text = "Score: " + ScoreManager.instance.GetCurrentScore().ToString("N0");
            }
            if (highScoreText != null)
            {
                highScoreText.text = "High Score: " + ScoreManager.instance.GetHighScore().ToString("N0");
            }
        }
    }

    private void ResumeGame()
    {
        // Panggil PauseManager untuk melanjutkan game
        if (PauseManager.instance != null)
        {
            PauseManager.instance.TogglePause();
        }
    }

    private void ExitToMainMenu()
    {
        // DITAMBAHKAN: Putar musik menu utama sebelum pindah scene
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayMainMenuBGM();
        }

        // Reset state game (level dan skor)
        if (GameManager.instance != null)
        {
            GameManager.instance.ResetGameState();
        }

        // Kembali ke Main Menu (diasumsikan scene index 0)
        SceneManager.LoadScene(0);
    }
}
