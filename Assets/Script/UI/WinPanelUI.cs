using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Mengelola fungsionalitas untuk Win Panel yang muncul saat game tamat.
/// </summary>
public class WinPanelUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text highScoreText;

    void Start()
    {
        // LevelController sudah menjeda game, tapi ini untuk memastikan.
        // Cursor juga ditampilkan agar pemain bisa klik tombol.
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SetupButtons();
        DisplayScores();
    }

    private void SetupButtons()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(QuitToMainMenu);
        }
    }

    private void DisplayScores()
    {
        if (ScoreManager.instance != null)
        {
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + ScoreManager.instance.GetFormattedScore();
            }
            if (highScoreText != null)
            {
                highScoreText.text = "High Score: " + ScoreManager.instance.GetFormattedHighScore();
            }
        }
    }

    public void RestartGame()
    {
        // PENTING: Kembalikan waktu ke normal sebelum pindah scene
        Time.timeScale = 1f;

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetGameState();
        }

        // Ganti "Level1" dengan nama scene level pertama Anda jika berbeda
        SceneManager.LoadScene("Level1");
    }

    public void QuitToMainMenu()
    {
        // PENTING: Kembalikan waktu ke normal sebelum pindah scene
        Time.timeScale = 1f;

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetGameState();
        }

        // Kembali ke menu utama (diasumsikan scene index 0)
        SceneManager.LoadScene(0);
    }
}