using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Boss2Controller : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private GameObject stealthEffectPrefab;
    [SerializeField] private GameObject appearEffectPrefab;
    [SerializeField] private GameObject crystalShardPrefab;
    [SerializeField] private Transform stealthEffectPoint;
    [SerializeField] private Transform[] crystalShardSpawnPoints;

    [Header("Movement Area")]
    public BoxCollider2D battleArea;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stealthMoveSpeed = 4f;

    [Header("Stealth System")]
    [SerializeField] private float stealthDuration = 3f;
    [SerializeField] private float stealthCooldown = 8f;
    [SerializeField] private float stealthAlpha = 0.2f; // Transparansi saat stealth
    [SerializeField] private bool canBeHitWhileStealth = false;

    [Header("Crystal Shard Attack")]
    [SerializeField] private int crystalShardCount = 5;
    [SerializeField] private float crystalShardSpeed = 8f;
    [SerializeField] private float crystalAttackCooldown = 6f;

    [Header("Health UI")]
    public Slider bossHealthSlider;
    public TextMeshProUGUI bossHealthText;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 75;
    private int currentHealth;

    [Header("Score Settings")]
    [SerializeField] private int scoreValue = 500;
    [SerializeField] private string bossType = "Crystal Stealth Ship";

    [Header("Hit Flash Effect")]
    [SerializeField] private Material mWhite;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;
    // Add AudioSource components
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip stealthSound;
    [SerializeField] private AudioClip crystalAttackSound;
    [SerializeField] private AudioClip hitSound;

    // Internal Systems
    private Transform player;
    private Rigidbody2D rb;
    private Collider2D bossCollider;
    private Vector2 minBounds, maxBounds;
    private Animator animator;

    // Stealth System
    private bool isInStealth = false;
    private bool canUseStealth = true;
    private Color originalColor;

    // Attack System
    private bool canUseCrystalAttack = true;
    
    // Damage Cooldown System
    private bool canDamagePlayer = true;
    private float damageCooldown = 1f; // 1 second cooldown between damages

    // Movement Pattern
    private Vector2 targetPosition;
    private bool isMovingToTarget = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossCollider = GetComponent<Collider2D>();

        if (spriteRenderer != null)
        {
            defaultMaterial = spriteRenderer.material;
            originalColor = spriteRenderer.color;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        currentHealth = maxHealth;

        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(false);
        }

        if (battleArea != null)
        {
            Bounds bounds = battleArea.bounds;
            minBounds = bounds.min;
            maxBounds = bounds.max;
        }

        InitHealthUI();
        SetRandomTargetPosition();
    }

    public void Initialize(BoxCollider2D area, Slider healthSlider, TextMeshProUGUI healthText)
    {
        battleArea = area;
        bossHealthSlider = healthSlider;
        bossHealthText = healthText;
        
        if (battleArea != null)
        {
            Bounds bounds = battleArea.bounds;
            minBounds = bounds.min;
            maxBounds = bounds.max;
        }
        
        InitHealthUI();
        
        Debug.Log("✅ Boss2Controller initialized with external UI components");
    }

    private void Update()
    {
        if (player == null) 
        {
            Debug.LogWarning("Player reference lost!");
            return;
        }

        HandleMovement();
        HandleStealthSystem();
        HandleCrystalAttack();

        // Update animator
        if (animator != null)
        {
            animator.SetFloat("Speed", rb.linearVelocity.magnitude);
            animator.SetBool("IsInStealth", isInStealth);
        }
        
        // Debug info setiap 5 detik
        if (Time.time % 5 < 0.1f)
        {
            Debug.Log($"Boss Status - Stealth: {canUseStealth}, Crystal Attack: {canUseCrystalAttack}, In Stealth: {isInStealth}");
        }
    }

    private void HandleMovement()
    {
        float currentMoveSpeed = isInStealth ? stealthMoveSpeed : moveSpeed;

        if (isMovingToTarget)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, currentMoveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
            {
                SetRandomTargetPosition();
            }
        }
        else
        {
            // Gentle floating movement
            Vector2 playerDirection = (player.position - transform.position).normalized;
            Vector2 moveDirection = playerDirection + Random.insideUnitCircle * 0.3f;
            
            Vector2 newPosition = (Vector2)transform.position + moveDirection * currentMoveSpeed * Time.deltaTime;
            newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);
            
            transform.position = newPosition;
        }
    }

    private void SetRandomTargetPosition()
    {
        targetPosition = new Vector2(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y)
        );
        isMovingToTarget = true;
    }

    private void HandleStealthSystem()
    {
        if (canUseStealth && !isInStealth)
        {
            StartCoroutine(StealthSequence());
        }
    }

    private void HandleCrystalAttack()
    {
        if (canUseCrystalAttack)
        {
            StartCoroutine(CrystalShardAttack());
        }
    }

    private IEnumerator StealthSequence()
    {
        canUseStealth = false;
        Debug.Log("👻 Stealth Sequence dimulai!");

        // Pre-stealth effect
        if (stealthEffectPrefab != null && stealthEffectPoint != null)
        {
            GameObject stealthEffect = Instantiate(stealthEffectPrefab, stealthEffectPoint.position, Quaternion.identity);
            Destroy(stealthEffect, 2f);
        }

        // Play stealth sound
        if (audioSource != null && stealthSound != null)
        {
            audioSource.PlayOneShot(stealthSound);
        }

        yield return new WaitForSeconds(0.5f);

        // Enter stealth
        EnterStealth();
        Debug.Log("👻 Boss masuk mode stealth!");

        if (animator != null)
        {
            animator.SetTrigger("EnterStealth");
        }

        // Move to surprise position during stealth
        Vector2 playerPosition = player.position;
        float stealthTimer = 0f;

        Debug.Log($"👻 Boss bergerak menuju Player di posisi: {playerPosition}");

        // Bergerak langsung ke arah player selama stealth
        while (stealthTimer < stealthDuration)
        {
            // Update player position setiap frame untuk tracking real-time
            if (player != null)
            {
                playerPosition = player.position;
            }
            
            // Bergerak langsung ke arah player dengan kecepatan stealth
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, stealthMoveSpeed * Time.deltaTime);
            
            stealthTimer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("👻 Boss keluar dari stealth!");

        // Exit stealth with attack
        ExitStealth();

        if (animator != null)
        {
            animator.SetTrigger("ExitStealth");
        }

        // Appear effect
        if (appearEffectPrefab != null)
        {
            GameObject appearEffect = Instantiate(appearEffectPrefab, transform.position, Quaternion.identity);
            Destroy(appearEffect, 1f);
        }

        // Surprise attack
        yield return StartCoroutine(SurpriseAttack());

        yield return new WaitForSeconds(stealthCooldown);
        canUseStealth = true;
    }

    private void EnterStealth()
    {
        isInStealth = true;
        
        if (spriteRenderer != null)
        {
            Color stealthColor = originalColor;
            stealthColor.a = stealthAlpha;
            spriteRenderer.color = stealthColor;
        }

        // Make harder to hit while in stealth
        if (!canBeHitWhileStealth && bossCollider != null)
        {
            bossCollider.enabled = false;
        }
    }

    private void ExitStealth()
    {
        isInStealth = false;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // Re-enable collision
        if (bossCollider != null)
        {
            bossCollider.enabled = true;
        }
    }

    private Vector2 GetSurpriseAttackPosition()
    {
        if (player == null) return transform.position;

        // Position behind or to the side of player for surprise attack
        Vector2 playerPos = player.position;
        Vector2 offsetDirection = Random.insideUnitCircle.normalized;
        
        Vector2 surprisePos = playerPos + offsetDirection * Random.Range(3f, 6f);
        
        // Clamp to battle area
        surprisePos.x = Mathf.Clamp(surprisePos.x, minBounds.x, maxBounds.x);
        surprisePos.y = Mathf.Clamp(surprisePos.y, minBounds.y, maxBounds.y);
        
        return surprisePos;
    }

    private IEnumerator SurpriseAttack()
    {
        if (player == null) yield break;

        Debug.Log("💥 Surprise Attack - Boss dash ke Player!");
        
        // Quick dash towards player's current position
        Vector2 dashDirection = (player.position - transform.position).normalized;
        float dashSpeed = stealthMoveSpeed * 2f; // Even faster dash
        
        rb.linearVelocity = dashDirection * dashSpeed;
        
        yield return new WaitForSeconds(0.8f); // Longer dash duration
        
        rb.linearVelocity = Vector2.zero;
        Debug.Log("💥 Surprise Attack selesai!");
    }

    private IEnumerator CrystalShardAttack()
    {
        canUseCrystalAttack = false;
        Debug.Log("🔮 Boss menembak Crystal Shard ke Player!");

        if (animator != null)
        {
            animator.SetTrigger("CrystalAttack");
        }

        // Play crystal attack sound
        if (audioSource != null && crystalAttackSound != null)
        {
            audioSource.PlayOneShot(crystalAttackSound);
        }

        yield return new WaitForSeconds(0.3f);

        // Tembak crystal shards langsung ke player
        for (int i = 0; i < crystalShardCount; i++)
        {
            ShootCrystalShardAtPlayer();
            yield return new WaitForSeconds(0.15f); // Slight delay between shots
        }

        Debug.Log($"Crystal Attack selesai, cooldown {crystalAttackCooldown} detik");
        yield return new WaitForSeconds(crystalAttackCooldown);
        canUseCrystalAttack = true;
    }

    private void ShootCrystalShardAtPlayer()
    {
        if (player == null) 
        {
            Debug.LogWarning("Player null! Tidak bisa menembak!");
            return;
        }

        Vector3 shootPosition = transform.position;
        
        // Use spawn points if available, otherwise shoot from boss position
        if (crystalShardSpawnPoints != null && crystalShardSpawnPoints.Length > 0)
        {
            Transform randomSpawnPoint = crystalShardSpawnPoints[Random.Range(0, crystalShardSpawnPoints.Length)];
            shootPosition = randomSpawnPoint.position;
        }

        // Calculate direction to player with debug info
        Vector2 directionToPlayer = (player.position - shootPosition).normalized;
        Vector2 velocity = directionToPlayer * crystalShardSpeed;
        
        Debug.Log($"🔫 Boss Position: {transform.position}");
        Debug.Log($"🎯 Player Position: {player.position}");
        Debug.Log($"📍 Shoot Position: {shootPosition}");
        Debug.Log($"➡️ Direction: {directionToPlayer}");
        Debug.Log($"⚡ Velocity: {velocity} (Speed: {crystalShardSpeed})");
        
        if (crystalShardPrefab != null)
        {
            // Shoot crystal shard prefab
            GameObject crystalShard = Instantiate(crystalShardPrefab, shootPosition, Quaternion.identity);
            
            // Try to use SimpleCrystalShard.SetVelocity method first
            SimpleCrystalShard shardScript = crystalShard.GetComponent<SimpleCrystalShard>();
            if (shardScript != null)
            {
                shardScript.SetVelocity(directionToPlayer, crystalShardSpeed);
                Debug.Log($"✅ Using SimpleCrystalShard.SetVelocity method");
            }
            else
            {
                // Fallback to direct Rigidbody2D control
                Rigidbody2D shardRb = crystalShard.GetComponent<Rigidbody2D>();
                if (shardRb != null)
                {
                    shardRb.linearVelocity = velocity;
                    Debug.Log($"✅ Crystal Shard Rigidbody2D found, velocity set to: {velocity}");
                }
                else
                {
                    Debug.LogError("❌ Crystal Shard TIDAK memiliki Rigidbody2D! Menambahkan secara otomatis...");
                    shardRb = crystalShard.AddComponent<Rigidbody2D>();
                    shardRb.gravityScale = 0;
                    shardRb.linearVelocity = velocity;
                }
            }
            
            // Ensure collider is trigger
            Collider2D shardCol = crystalShard.GetComponent<Collider2D>();
            if (shardCol != null)
            {
                shardCol.isTrigger = true;
            }
            
            // Auto-destroy after 8 seconds
            Destroy(crystalShard, 8f);
        }
        else
        {
            // Create temporary projectile if no prefab assigned
            Debug.LogWarning("Crystal Shard Prefab tidak ada! Membuat peluru sementara...");
            CreateAndShootTemporaryCrystal(shootPosition, directionToPlayer);
        }
    }
    
    private void CreateAndShootTemporaryCrystal(Vector3 shootPosition, Vector2 direction)
    {
        // Create temporary crystal projectile
        GameObject tempCrystal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tempCrystal.name = "Temporary Crystal Bullet";
        tempCrystal.transform.position = shootPosition;
        tempCrystal.transform.localScale = Vector3.one * 0.4f;
        
        // Setup physics - PENTING: gravity scale = 0!
        Rigidbody2D rb = tempCrystal.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0; // No gravity!
        rb.linearDamping = 0; // No air resistance
        rb.angularDamping = 0;
        
        // Calculate velocity
        Vector2 velocity = direction * crystalShardSpeed;
        rb.linearVelocity = velocity;
        
        // Setup collision
        Collider2D col = tempCrystal.GetComponent<Collider2D>();
        col.isTrigger = true;
        
        // Add damage script
        tempCrystal.AddComponent<SimpleCrystalShard>();
        
        // Make it look like crystal (cyan color)
        Renderer renderer = tempCrystal.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.cyan;
        }
        
        Debug.Log($"🔫 Temporary Crystal Bullet created at {shootPosition}");
        Debug.Log($"📈 Direction: {direction}, Speed: {crystalShardSpeed}");
        Debug.Log($"⚡ Final Velocity: {velocity}");
        Debug.Log($"🚀 Rigidbody settings - Gravity: {rb.gravityScale}, Drag: {rb.linearDamping}");
        
        // Auto-destroy
        Destroy(tempCrystal, 8f);
    }

    public void TakeDamage(int damageAmount)
    {
        // Can't take damage while in stealth (if set)
        if (isInStealth && !canBeHitWhileStealth) return;

        currentHealth -= damageAmount;
        UpdateHealthUI();

        // Play hit sound
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

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
        // Award score for defeating the boss
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddEnemyKillScore(bossType);
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Exit stealth if dying while stealthed
        if (isInStealth)
        {
            ExitStealth();
        }

        Destroy(gameObject, 1f);

        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(false);
        }
    }

    private IEnumerator HitFlash()
    {
        if (spriteRenderer != null && mWhite != null && !isInStealth)
        {
            spriteRenderer.material = mWhite;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.material = defaultMaterial;
        }
    }

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
        if (other.CompareTag("bullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(1);
        }
        else if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && canDamagePlayer)
            {
                // Deal only 1 damage to player, not instant kill
                player.TakeDamage(1);

                // Start damage cooldown
                StartCoroutine(DamageCooldown());
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamagePlayer = false;
        yield return new WaitForSeconds(damageCooldown);
        canDamagePlayer = true;
    }
}
