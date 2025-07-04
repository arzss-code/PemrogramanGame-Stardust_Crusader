using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class Boss3Controller : MonoBehaviour, IDamageable
{
    private enum State { Entering, Attacking, Dying }
    private State currentState = State.Entering;
    [SerializeField] private BossRegenShield regenShield;


    [Header("Movement")]
    public float entrySpeed = 3f;
    public float entryTargetX = 12f;
    public float patrolSpeed = 2f;
    public float patrolDistance = 3f;

    [Header("Battle Area")]
    public BoxCollider2D battleArea;

    [Header("Attack Pattern")]
    [SerializeField] private Boss3LaserSpread laserSpread;
    [SerializeField] private Boss3LaserSpiral laserSpiral;
    [SerializeField] private float attackInterval = 3f;
    [SerializeField] private float spiralAttackDuration = 4f;

    [Header("Health")]
    [SerializeField] private int maxHP = 20;
    private int currentHP;

    [Header("Effects")]
    [SerializeField] private GameObject destructionEffect;

    [Header("Flash Effect")]
    [SerializeField] private Material whiteMaterial;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashDuration = 0.1f;

    // UI References (set by Initializer)
    private Slider bossHealthSlider;
    private TextMeshProUGUI bossHealthText;
    private Slider bossShieldSlider;
    private TextMeshProUGUI bossShieldText;

    private float attackTimer = 0f;
    private int currentPattern = 0;
    private Coroutine spiralCoroutine;
    private bool isDying = false;
    private Vector2 initialPosition;
    private Vector2 minBounds;
    private Vector2 maxBounds;

    private void Awake()
    {
        currentHP = maxHP;
        // Get internal components
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultMaterial = spriteRenderer.material;
        }

        // Otomatis cari komponen penting untuk mengurangi risiko error jika lupa di-assign di Inspector.
        if (regenShield == null)
        {
            regenShield = GetComponentInChildren<BossRegenShield>();
            Debug.Log("Mencari BossRegenShield secara otomatis...");
        }

        if (battleArea != null)
        {
            entryTargetX = battleArea.bounds.max.x - 1f; // masuk sedikit ke dalam BattleArea
        }

    }

    public void Initialize(BoxCollider2D newBattleArea, Slider healthSlider, TextMeshProUGUI healthText, Slider shieldSlider, TextMeshProUGUI shieldText)
    {
        this.battleArea = newBattleArea;
        this.bossHealthSlider = healthSlider;
        this.bossHealthText = healthText;
        this.bossShieldSlider = shieldSlider;
        this.bossShieldText = shieldText;

        if (this.battleArea != null)
        {
            entryTargetX = battleArea.bounds.max.x - 1f;
            minBounds = battleArea.bounds.min;
            maxBounds = battleArea.bounds.max;
        }
        else
        {
            Debug.LogError("Boss3 di-spawn tanpa Battle Area! AI tidak akan berjalan.", this.gameObject);
            return;
        }

        // Pass UI references to the shield component
        if (regenShield != null)
        {
            regenShield.bossShieldSlider = this.bossShieldSlider;
            regenShield.bossShieldText = this.bossShieldText;
        }
        else
        {
            Debug.LogWarning("Referensi 'regenShield' pada Boss3Controller hilang! UI Shield tidak akan ter-update dengan benar.", this);
        }

        InitUI();
        StartBehavior();
    }

    private void StartBehavior()
    {
        // State machine starts automatically, this is just for consistency
        Debug.Log("Boss 3 behavior started.");
    }
    private void Update()
    {
        if (isDying) return;
        switch (currentState)
        {
            case State.Entering:
                MoveIntoScreen();
                break;
            case State.Attacking:
                HandlePatrolMovement();
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackInterval)
                {
                    attackTimer = 0f;
                    PerformAttack();
                }
                break;
            case State.Dying:
                break;
        }
    }

    private void MoveIntoScreen()
    {
        transform.position += Vector3.left * entrySpeed * Time.deltaTime;

        if (transform.position.x <= entryTargetX)
        {
            transform.position = new Vector3(entryTargetX, transform.position.y, transform.position.z);
            currentState = State.Attacking;
            initialPosition = transform.position;
            Debug.Log("🎯 Boss3 memasuki layar dan mulai menyerang!");
        }
    }

    private void PerformAttack()
    {
        switch (currentPattern)
        {
            case 0:
                laserSpread?.Shoot();
                break;

            case 1:
                if (spiralCoroutine == null)
                {
                    spiralCoroutine = StartCoroutine(FireSpiralTemporarily());
                }
                break;
        }

        currentPattern = (currentPattern + 1) % 2;
    }

    private IEnumerator FireSpiralTemporarily()
    {
        if (laserSpiral != null)
        {
            laserSpiral.StartFiring();
            yield return new WaitForSeconds(spiralAttackDuration);
            laserSpiral.StopFiring();
        }

        spiralCoroutine = null;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDying) return;

        // Logika shield sekarang ditangani oleh LaserBullets.cs.
        // Metode ini hanya akan dipanggil dengan sisa damage jika shield sudah ditembus,
        // atau damage penuh jika shield tidak aktif.
        currentHP -= damageAmount;
        if (currentHP < 0) currentHP = 0;

        UpdateHealthUI();
        StartCoroutine(FlashEffect());

        if (currentHP <= 0)
        {
            Die();
        }
    }


    private IEnumerator FlashEffect()
    {
        if (spriteRenderer != null && whiteMaterial != null)
        {
            spriteRenderer.material = whiteMaterial;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.material = defaultMaterial;
        }
    }

    private void Die()
    {
        isDying = true;
        currentState = State.Dying;
        Debug.Log("💀 Boss3 telah dikalahkan!");

        // Stop all attacks
        if (spiralCoroutine != null)
        {
            StopCoroutine(spiralCoroutine);
            laserSpiral?.StopFiring();
        }

        // Destruction effect
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        // Hide UI
        if (bossHealthSlider != null) bossHealthSlider.gameObject.SetActive(false);
        if (bossHealthText != null) bossHealthText.gameObject.SetActive(false);
        if (bossShieldSlider != null) bossShieldSlider.gameObject.SetActive(false);
        if (bossShieldText != null) bossShieldText.gameObject.SetActive(false);

        // Notify LevelController
        LevelController.Instance?.OnBossDefeated();

        // Destroy the boss object
        Destroy(gameObject, 0.2f);
    }

    private void InitUI()
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(true);
            bossHealthSlider.maxValue = maxHP;
            bossHealthSlider.value = currentHP;
        }
        if (bossHealthText != null)
        {
            bossHealthText.gameObject.SetActive(true);
            bossHealthText.text = $"{currentHP} / {maxHP}";
        }

        // Shield UI is handled by BossRegenShield, but we ensure it's active
        if (bossShieldSlider != null) bossShieldSlider.gameObject.SetActive(true);
        if (bossShieldText != null) bossShieldText.gameObject.SetActive(true);
    }

    private void UpdateHealthUI()
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = currentHP;
        }
        if (bossHealthText != null)
        {
            bossHealthText.text = $"{currentHP} / {maxHP}";
        }
    }

    // This is no longer needed as LaserBullets will call TakeDamage via IDamageable
    // private void OnTriggerEnter2D(Collider2D other) { ... }

    private void HandlePatrolMovement()
    {
        // Gerakan sinusoidal (naik-turun) pada sumbu Y
        float newY = initialPosition.y + Mathf.Sin(Time.time * patrolSpeed) * patrolDistance;

        // Batasi pergerakan agar tetap di dalam battleArea
        if (battleArea != null)
        {
            newY = Mathf.Clamp(newY, minBounds.y, maxBounds.y);
        }

        transform.position = new Vector2(initialPosition.x, newY);
    }
}
