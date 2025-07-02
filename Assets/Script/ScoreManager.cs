using UnityEngine;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    
    [Header("Score Settings")]
    public int currentScore = 0;
    public int enemyKillScore = 100;
    public int powerUpScore = 50;
    public int obstacleDestroyScore = 35; // Score untuk menghancurkan obstacle
    public int survivalScorePerSecond = 10;
    public int levelCompleteBonus = 1000;
    public int comboMultiplier = 1;
    public int maxComboMultiplier = 5;
    
    [Header("Combo Settings")]
    public float comboTimeWindow = 3f; // Waktu untuk mempertahankan combo
    private float lastKillTime;
    private int currentCombo = 0;
    
    [Header("High Score")]
    private int highScore = 0;
    private const string HIGH_SCORE_KEY = "HighScore";
    
    [Header("Score Events")]
    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnHighScoreBeaten;
    
    private float gameStartTime;
    private Coroutine survivalScoreCoroutine;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadHighScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        gameStartTime = Time.time;
        StartSurvivalScore();
    }
    
    // Memulai skor survival (skor per detik)
    private void StartSurvivalScore()
    {
        if (survivalScoreCoroutine != null)
            StopCoroutine(survivalScoreCoroutine);
        
        survivalScoreCoroutine = StartCoroutine(SurvivalScoreCoroutine());
    }
    
    private IEnumerator SurvivalScoreCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            AddScore(survivalScorePerSecond, "Survival");
        }
    }
    
    // Method utama untuk menambah skor
    public void AddScore(int baseScore, string source = "")
    {
        int finalScore = baseScore * comboMultiplier;
        currentScore += finalScore;
        
        // Trigger event
        OnScoreChanged?.Invoke(currentScore);
        
        // Check for new high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
            OnHighScoreBeaten?.Invoke(highScore);
        }
        
        Debug.Log($"Score +{finalScore} from {source} (Base: {baseScore}, Multiplier: x{comboMultiplier})");
    }
    
    // Skor untuk membunuh musuh
    public void AddEnemyKillScore(string enemyType = "Enemy")
    {
        AddScore(enemyKillScore, $"{enemyType} Kill");
        AddCombo();
    }
    
    // Skor untuk mengambil power-up
    public void AddPowerUpScore()
    {
        AddScore(powerUpScore, "Power-Up");
    }
    
    // Skor untuk menghancurkan obstacle
    public void AddObstacleDestroyScore(string obstacleType = "Obstacle")
    {
        AddScore(obstacleDestroyScore, $"{obstacleType} Destroyed");
        AddCombo(); // Menghancurkan obstacle juga menambah combo
    }
    
    // Skor bonus level complete
    public void AddLevelCompleteScore()
    {
        int timeBonus = CalculateTimeBonus();
        int comboBonus = currentCombo * 50;
        int totalBonus = levelCompleteBonus + timeBonus + comboBonus;
        
        AddScore(totalBonus, "Level Complete");
        Debug.Log($"Level Complete! Bonus: {totalBonus} (Base: {levelCompleteBonus}, Time: {timeBonus}, Combo: {comboBonus})");
    }
    
    // Hitung bonus waktu berdasarkan seberapa cepat level diselesaikan
    private int CalculateTimeBonus()
    {
        float gameTime = Time.time - gameStartTime;
        int timeBonus = Mathf.Max(0, (int)(1000 - gameTime * 10)); // Bonus berkurang seiring waktu
        return timeBonus;
    }
    
    // Sistem combo
    private void AddCombo()
    {
        lastKillTime = Time.time;
        currentCombo++;
        
        // Update combo multiplier
        comboMultiplier = Mathf.Clamp(1 + (currentCombo / 5), 1, maxComboMultiplier);
        
        // Start combo decay timer
        StartCoroutine(ComboDecayTimer());
    }
    
    private IEnumerator ComboDecayTimer()
    {
        yield return new WaitForSeconds(comboTimeWindow);
        
        // Check if no new kills happened during the window
        if (Time.time - lastKillTime >= comboTimeWindow)
        {
            ResetCombo();
        }
    }
    
    private void ResetCombo()
    {
        currentCombo = 0;
        comboMultiplier = 1;
    }
    
    // High Score Management
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
    }
    
    private void SaveHighScore()
    {
        PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
        PlayerPrefs.Save();
    }
    
    public int GetHighScore()
    {
        return highScore;
    }
    
    public int GetCurrentScore()
    {
        return currentScore;
    }
    
    public int GetCurrentCombo()
    {
        return currentCombo;
    }
    
    public int GetComboMultiplier()
    {
        return comboMultiplier;
    }
    
    // Reset skor untuk game baru
    public void ResetScore()
    {
        currentScore = 0;
        currentCombo = 0;
        comboMultiplier = 1;
        gameStartTime = Time.time;
        
        OnScoreChanged?.Invoke(currentScore);
        
        StartSurvivalScore();
    }
    
    // Stop survival score saat game over
    public void StopScoring()
    {
        if (survivalScoreCoroutine != null)
        {
            StopCoroutine(survivalScoreCoroutine);
            survivalScoreCoroutine = null;
        }
    }
    
    // Format skor untuk display
    public string GetFormattedScore()
    {
        return currentScore.ToString("N0");
    }
    
    public string GetFormattedHighScore()
    {
        return highScore.ToString("N0");
    }
}
