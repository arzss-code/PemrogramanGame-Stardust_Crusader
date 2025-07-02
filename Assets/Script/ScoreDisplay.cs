using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScoreDisplay : MonoBehaviour
{
    [Header("Score UI")]
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    
    [Header("Effects")]
    public Animator scoreAnimator;
    public ParticleSystem scoreParticles;
    public AudioSource scoreAudioSource;
    public AudioClip scoreSound;
    
    private int displayedScore = 0;
    private Coroutine scoreCountUpCoroutine;
    
    private void Start()
    {
        // Subscribe to score events
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.OnScoreChanged += OnScoreChanged;
            ScoreManager.instance.OnHighScoreBeaten += OnHighScoreBeaten;
            
            // Initialize display
            displayedScore = ScoreManager.instance.GetCurrentScore();
            UpdateScoreDisplay();
            UpdateHighScoreDisplay();
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.OnScoreChanged -= OnScoreChanged;
            ScoreManager.instance.OnHighScoreBeaten -= OnHighScoreBeaten;
        }
    }
    
    private void OnScoreChanged(int newScore)
    {
        // Stop current count up if running
        if (scoreCountUpCoroutine != null)
        {
            StopCoroutine(scoreCountUpCoroutine);
        }
        
        // Start count up animation
        scoreCountUpCoroutine = StartCoroutine(CountUpScore(displayedScore, newScore));
        
        // Play effects
        PlayScoreEffects();
    }
    
    private IEnumerator CountUpScore(int startScore, int targetScore)
    {
        float duration = 0.5f;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            
            displayedScore = Mathf.RoundToInt(Mathf.Lerp(startScore, targetScore, progress));
            UpdateScoreDisplay();
            
            yield return null;
        }
        
        displayedScore = targetScore;
        UpdateScoreDisplay();
    }
    
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = displayedScore.ToString("N0");
            
            // Animate score text
            if (scoreAnimator != null)
            {
                scoreAnimator.SetTrigger("ScoreUpdate");
            }
        }
    }
    
    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null && ScoreManager.instance != null)
        {
            int highScore = ScoreManager.instance.GetHighScore();
            highScoreText.text = "High Score: " + highScore.ToString("N0");
        }
    }
    
    private void OnHighScoreBeaten(int newHighScore)
    {
        UpdateHighScoreDisplay();
        
        // Add high score beaten effects here
        if (scoreAnimator != null)
        {
            scoreAnimator.SetTrigger("HighScoreBeat");
        }
        
        PlayScoreEffects();
    }
    
    private void PlayScoreEffects()
    {
        // Play particle effect
        if (scoreParticles != null)
        {
            scoreParticles.Play();
        }
        
        // Play sound effect
        if (scoreAudioSource != null && scoreSound != null)
        {
            scoreAudioSource.PlayOneShot(scoreSound);
        }
    }
}
