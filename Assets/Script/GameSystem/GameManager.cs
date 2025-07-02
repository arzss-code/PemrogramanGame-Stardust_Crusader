using UnityEngine;
using UnityEngine.SceneManagement; // DITAMBAHKAN: Untuk bisa mengontrol perpindahan scene

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float worldSpeed; // Variabel Anda yang sudah ada tetap dipertahankan

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
        // Ganti "GameOverScene" dengan nama scene "Game Over" yang sudah Anda buat.
        // Pastikan scene tersebut juga sudah didaftarkan di Build Settings.
        SceneManager.LoadScene("GameOverScene");
    }
}