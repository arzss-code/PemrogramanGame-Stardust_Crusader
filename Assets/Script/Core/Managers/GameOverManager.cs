using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager instance;

    [Header("Game Over UI")]
    [Tooltip("Seret prefab Canvas Game Over ke sini")]
    [SerializeField] private GameObject gameOverMenuPrefab;

    private GameObject gameOverMenuInstance;
    private bool isGameOver = false;

    public bool IsGameOver => isGameOver;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowGameOver()
    {
        if (isGameOver) return; // Jangan tampilkan lagi jika sudah game over

        isGameOver = true;
        Time.timeScale = 0f; // Jeda semua aksi di game

        // Tampilkan UI Game Over dari prefab
        if (gameOverMenuPrefab != null && gameOverMenuInstance == null)
        {
            gameOverMenuInstance = Instantiate(gameOverMenuPrefab);
        }

        // Jeda musik latar
        if (AudioManager.instance != null && AudioManager.instance.bgmSource.isPlaying)
        {
            AudioManager.instance.bgmSource.Pause();
        }
    }

    /// <summary>
    /// Metode untuk mereset status saat game baru dimulai dari awal.
    /// </summary>
    public void ResetState()
    {
        isGameOver = false;
    }
}