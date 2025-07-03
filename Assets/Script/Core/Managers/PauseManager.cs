using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    [Header("Pause Menu")]
    [Tooltip("Seret prefab Canvas Pause Menu ke sini")]
    [SerializeField] private GameObject pauseMenuPrefab;

    private GameObject pauseMenuInstance;
    private bool isPaused = false;

    // Properti publik untuk memeriksa status pause dari skrip lain
    public bool IsPaused => isPaused;

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

    private void Update()
    {
        // Dengarkan input tombol Escape untuk membuka/menutup menu pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        // Hentikan waktu game
        Time.timeScale = 0f;

        // Tampilkan pause menu
        if (pauseMenuPrefab != null && pauseMenuInstance == null)
        {
            pauseMenuInstance = Instantiate(pauseMenuPrefab);
        }

        // Jeda BGM secara eksplisit
        if (AudioManager.instance != null && AudioManager.instance.bgmSource.isPlaying)
        {
            AudioManager.instance.bgmSource.Pause();
        }
    }

    public void ResumeGame()
    {
        // Lanjutkan waktu game
        Time.timeScale = 1f;

        // Hancurkan instance pause menu
        if (pauseMenuInstance != null)
        {
            Destroy(pauseMenuInstance);
            pauseMenuInstance = null; // Hapus referensi agar bisa dibuat lagi nanti
        }

        // Lanjutkan BGM secara eksplisit
        if (AudioManager.instance != null)
        {
            AudioManager.instance.bgmSource.UnPause();
        }
    }
}
