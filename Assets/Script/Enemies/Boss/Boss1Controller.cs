using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
public class Boss1Controller : MonoBehaviour, IDamageable
{
    [Header("Visual Effects")]
    [SerializeField] private GameObject dashEffectPrefab;
    [SerializeField] private Transform dashEffectPoint;
    [SerializeField] private GameObject destructionEffect;

    [Header("Movement")]
    [Tooltip("Area di mana bos akan bergerak.")]
    public BoxCollider2D battleArea;
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("Interval waktu (min, max) sebelum bos memilih target gerakan baru.")]
    [SerializeField] private Vector2 moveInterval = new Vector2(1f, 3f);

    [Header("Attacks")]
    [Header(" - Projectile Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [Tooltip("Jumlah peluru dalam satu rentetan tembakan (burst).")]
    [SerializeField] private int burstCount = 5;
    [Tooltip("Jeda waktu antar tembakan dalam satu rentetan (detik).")]
    [SerializeField] private float fireRate = 0.2f;
    [Tooltip("Waktu jeda (detik) antara rentetan tembakan.")]
    [SerializeField] private float shootCooldown = 2f;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private int projectileDamage = 1;

    [Header(" - Dash Attack")]
    [SerializeField] private float dashSpeed = 20f;
    [Tooltip("Waktu tunggu (cooldown) sebelum bisa dash lagi.")]
    [SerializeField] private float dashCooldown = 8f;
    [Tooltip("Seberapa sering bos mencoba melakukan dash (detik).")]
    [SerializeField] private float dashAttemptInterval = 4f;

    [Header("Health UI")]
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private TextMeshProUGUI bossHealthText;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("Hit Flash Effect")]
    [SerializeField] private Material mWhite;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;

    [Header("Collision")]
    [SerializeField] private int collisionDamage = 2; // Damage saat bos menabrak player

    [Header("Audio")]
    [SerializeField] private AudioClip shootSFX;
    private AudioSource audioSource;

    // Internal state
    private Transform player;
    private Rigidbody2D rb;
    private Vector2 minBounds, maxBounds;
    private bool canDash = true;
    private bool isDying = false;
    private Animator animator;
    private Coroutine movementCoroutine;
    private Coroutine shootingCoroutine;
    private Coroutine dashCoroutine;

    // Gunakan Awake untuk inisialisasi komponen internal.
    // Awake dipanggil sebelum method Initialize dari luar.
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (spriteRenderer != null)
        {
            defaultMaterial = spriteRenderer.material;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;
    }

    public void StartBehavior()
    {
        if (isDying) return;

        // Hentikan coroutine lama jika ada (untuk keamanan)
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        if (shootingCoroutine != null) StopCoroutine(shootingCoroutine);
        if (dashCoroutine != null) StopCoroutine(dashCoroutine);

        // Mulai coroutine untuk setiap aksi
        movementCoroutine = StartCoroutine(MoveRoutine());
        shootingCoroutine = StartCoroutine(ShootRoutine());
        dashCoroutine = StartCoroutine(AttemptDashRoutine());
    }

    // --- BEHAVIOR COROUTINES ---

    /// <summary>
    /// 1. Pergerakan lincah naik-turun dan maju-mundur.
    /// </summary>
    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            // Tentukan target posisi acak di dalam battleArea
            Vector2 targetPosition = new Vector2(
                Random.Range(minBounds.x, maxBounds.x),
                Random.Range(minBounds.y, maxBounds.y)
            );

            // Bergerak ke target
            while (Vector2.Distance(transform.position, targetPosition) > 0.5f)
            {
                if (isDying) yield break; // Hentikan jika bos mati
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Tunggu dengan interval waktu acak sebelum bergerak lagi
            float waitTime = Random.Range(moveInterval.x, moveInterval.y);
            yield return new WaitForSeconds(waitTime);
        }
    }

    /// <summary>
    /// 2. Menembakkan proyektil secara berkala.
    /// </summary>
    private IEnumerator ShootRoutine()
    {
        // Tunggu sebentar sebelum mulai menembak
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            if (isDying || player == null) yield break;

            // --- FASE TEMBAKAN (BURST) ---
            for (int i = 0; i < burstCount; i++)
            {
                // Cek lagi di dalam loop, kalau-kalau bos mati di tengah burst
                if (isDying || player == null) yield break;

                // Tembak ke arah player
                GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

                if (audioSource != null && shootSFX != null)
                {
                    audioSource.PlayOneShot(shootSFX);
                }

                BossProjectile projectileScript = projectileObj.GetComponent<BossProjectile>();
                if (projectileScript != null)
                {
                    Vector2 directionToPlayer = (player.position - firePoint.position).normalized;
                    projectileScript.Initialize(directionToPlayer, projectileSpeed, projectileDamage);
                }

                // Tunggu sesuai fireRate (jeda antar peluru dalam burst)
                yield return new WaitForSeconds(fireRate);
            }

            // --- FASE JEDA (COOLDOWN) ---
            // Tunggu sebelum memulai burst berikutnya
            yield return new WaitForSeconds(shootCooldown);
        }
    }

    /// <summary>
    /// 3. Serangan Dash (condong maju).
    /// </summary>
    private IEnumerator AttemptDashRoutine()
    {
        while (true)
        {
            // Tunggu interval sebelum mencoba dash
            yield return new WaitForSeconds(dashAttemptInterval);

            if (canDash && !isDying && player != null)
            {
                yield return StartCoroutine(DashRoutine());
            }
        }
    }

    private IEnumerator DashRoutine()
    {
        canDash = false;

        // Hentikan pergerakan normal sejenak
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);

        // Arahkan ke player sebelum dash
        Vector2 targetPos = new Vector2(transform.position.x, player.position.y);
        float timeToAlign = 0.5f;
        float elapsedTime = 0;
        while (elapsedTime < timeToAlign)
        {
            transform.position = Vector2.Lerp(transform.position, targetPos, (elapsedTime / timeToAlign));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Animasi dan efek dash
        if (animator != null) animator.SetTrigger("Dash");
        if (dashEffectPrefab != null && dashEffectPoint != null)
        {
            Instantiate(dashEffectPrefab, dashEffectPoint.position, Quaternion.identity);
        }

        // Lakukan Dash ke kiri
        rb.linearVelocity = Vector2.left * dashSpeed;

        // Tunggu sampai keluar layar
        yield return new WaitUntil(() => transform.position.x < minBounds.x - 5f);

        // Berhenti dan pindah posisi ke kanan layar secara acak
        rb.linearVelocity = Vector2.zero;
        Vector2 newPos = new Vector2(maxBounds.x + 5f, Random.Range(minBounds.y, maxBounds.y));
        transform.position = newPos;

        // Kembali masuk ke area pertempuran
        Vector2 entryPoint = new Vector2(maxBounds.x, transform.position.y);
        while (Vector2.Distance(transform.position, entryPoint) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, entryPoint, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // Mulai lagi pergerakan normal
        movementCoroutine = StartCoroutine(MoveRoutine());

        // Cooldown sebelum bisa dash lagi
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }


    // --- HEALTH & DAMAGE ---

    public void TakeDamage(int damageAmount)
    {
        if (isDying) return;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0); // Pastikan health tidak minus
        UpdateHealthUI();

        StartCoroutine(HitFlash());

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDying = true;

        // Hentikan semua aksi
        if (movementCoroutine != null) StopCoroutine(movementCoroutine);
        if (shootingCoroutine != null) StopCoroutine(shootingCoroutine);
        if (dashCoroutine != null) StopCoroutine(dashCoroutine);
        rb.linearVelocity = Vector2.zero;

        // Hancurkan semua proyektil bos yang ada
        foreach (var projectile in FindObjectsOfType<BossProjectile>())
        {
            Destroy(projectile.gameObject);
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Efek ledakan
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        // Sembunyikan health bar
        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(false);
        }

        // Beri tahu LevelController bahwa bos telah dikalahkan
        LevelController.Instance?.OnBossDefeated();

        // Hancurkan game object setelah beberapa saat
        Destroy(gameObject, 1.5f); // Beri waktu untuk animasi kematian
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer != null && mWhite != null)
        {
            spriteRenderer.material = mWhite;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.material = defaultMaterial;
        }
    }

    // --- INITIALIZATION ---
    // Method ini dipanggil oleh LevelController setelah bos di-spawn.
    public void Initialize(BoxCollider2D newBattleArea, Slider healthSlider, TextMeshProUGUI healthText)
    {
        // 1. Set referensi eksternal yang diberikan oleh LevelController
        this.battleArea = newBattleArea;
        this.bossHealthSlider = healthSlider;
        this.bossHealthText = healthText;

        // 2. Inisialisasi state internal yang bergantung pada referensi eksternal
        if (this.battleArea != null)
        {
            Bounds bounds = this.battleArea.bounds;
            minBounds = bounds.min;
            maxBounds = bounds.max;
        }
        else
        {
            Debug.LogError("Boss di-spawn tanpa Battle Area! AI tidak akan berjalan.", this.gameObject);
            return; // Hentikan eksekusi jika area tidak ada
        }

        InitHealthUI();
        StartBehavior(); // Mulai AI setelah semua setup selesai
    }

    // --- UI ---

    private void InitHealthUI()
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(true);
            bossHealthSlider.maxValue = maxHealth;
            bossHealthSlider.value = maxHealth;
        }

        if (bossHealthText != null)
        {
            bossHealthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    private void UpdateHealthUI()
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = currentHealth;
        }

        if (bossHealthText != null)
        {
            bossHealthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Tabrakan dengan player
        if (other.CompareTag("Player"))
        {
            // Berikan damage ke player jika player punya IDamageable
            other.GetComponent<IDamageable>()?.TakeDamage(collisionDamage);
        }
    }

}
