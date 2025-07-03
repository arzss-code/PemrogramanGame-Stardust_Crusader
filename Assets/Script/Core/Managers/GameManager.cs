using UnityEngine;
using UnityEngine.SceneManagement; // DITAMBAHKAN: Untuk bisa mengontrol perpindahan scene

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float worldSpeed; // Variabel Anda yang sudah ada tetap dipertahankan

    // DITAMBAHKAN: Untuk melacak level saat ini
    public static int currentLevelIndex = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // DITAMBAHKAN: Perintah ini membuat GameManager 'abadi' dan tidak ikut hancur
            // saat scene baru dimuat. Ini sangat PENTING!
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Jika sudah ada GameManager dari scene sebelumnya, hancurkan duplikat yang baru ini.
            Destroy(gameObject);
        }
    }

    // --- FUNGSI-FUNGSI BARU UNTUK MANAJEMEN LEVEL ---

    /// <summary>
    /// Panggil fungsi ini ketika pemain berhasil menyelesaikan sebuah level.
    /// </summary>
    public void LevelCompleted()
    {
        Debug.Log("Level Selesai! Memuat level berikutnya...");

        // Add level complete bonus score
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddLevelCompleteScore();
        }

        // DITAMBAHKAN: Naikkan level index sebelum memuat level baru
        currentLevelIndex++;

        // Mendapatkan nomor index dari scene yang sedang berjalan saat ini.
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Cek apakah ada scene setelah scene ini di dalam Build Settings.
        if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            // Jika ada, muat scene selanjutnya.
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            // Jika tidak ada, berarti ini adalah level terakhir. Game tamat!
            Debug.Log("SELURUH LEVEL TELAH DISELESAIKAN! ANDA MENANG!");
            // Anda bisa memuat scene "Kemenangan" atau kembali ke Menu Utama.
            // Ganti "MainMenu" dengan nama scene yang Anda inginkan.
            // Contoh: SceneManager.LoadScene("WinScene");
            SceneManager.LoadScene(0); // Memuat scene pertama (index 0) di Build Settings
        }
    }

    /// <summary>
    /// Panggil fungsi ini ketika nyawa pemain habis.
    /// </summary>
    public void GameOver()
    {
        Debug.Log("GAME OVER");

        // Stop scoring when game is over
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.StopScoring();
        }

        // Ganti "GameOverScene" dengan nama scene "Game Over" yang sudah Anda buat.
        // Pastikan scene tersebut juga sudah didaftarkan di Build Settings.
        SceneManager.LoadScene("GameOverScene");
    }

    /// <summary>
    /// Panggil fungsi ini untuk mereset state game (level dan skor) saat memulai game baru.
    /// </summary>
    public void ResetGameState()
    {
        currentLevelIndex = 1;
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }
    }
}