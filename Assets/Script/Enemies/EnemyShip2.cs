using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyShip2 : MonoBehaviour
{
    public enum WaveType
    {
        FastAttack,      // Fast-moving enemies
        Formation,       // Formation movement
        HighIntensity    // High-intensity mixed wave
    }

    private enum State
    {
        Entering,
        InFormation,
        Attacking,
        Exiting
    }

    [Header("Wave Configuration")]
    [SerializeField] private WaveType waveType = WaveType.FastAttack;
    [SerializeField] private int formationIndex = 0; // Position in formation (0, 1, 2, etc.)

    [Header("Movement Settings")]
    [SerializeField] private float baseSpeed = 4f;
    [SerializeField] private float fastSpeed = 8f;
    [SerializeField] private float formationSpeed = 3f;
    [SerializeField] private float highIntensitySpeed = 6f;
    [SerializeField] private float destroyXThreshold = -20f;

    [Header("Formation Settings")]
    [SerializeField] private Vector2 formationOffset = new Vector2(-6f, 0f);
    [SerializeField] private float formationSpacing = 2f;
    [SerializeField] private float formationWaitTime = 2f;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 8f;

    [Header("High Intensity Settings")]
    [SerializeField] private float zigzagAmplitude = 3f;
    [SerializeField] private float zigzagFrequency = 2f;
    [SerializeField] private float burstFireRate = 0.3f;
    [SerializeField] private int burstCount = 3;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 2;
    private int currentHealth;

    [Header("Score Settings")]
    [SerializeField] private int scoreValue = 150;
    [SerializeField] private string enemyType = "Enemy Ship 2";

    [Header("Hit Effect")]
    [SerializeField] private Material whiteMaterial;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip destroySound;

    private Rigidbody2D rb;
    private Collider2D col;
    private Transform player;
    private State currentState = State.Entering;
    
    private Vector2 targetFormationPosition;
    private Vector2 startingPosition;
    private float attackTimer = 0f;
    private float stateTimer = 0f;
    private bool isAttacking = false;

    private void Start()
    {
        InitializeComponents();
        InitializeByWaveType();
        FindPlayer();
        
        currentHealth = maxHealth;
        startingPosition = transform.position;
        
        Debug.Log($"🚀 EnemyShip2 spawned with wave type: {waveType}, formation index: {formationIndex}");
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
            defaultMaterial = spriteRenderer.material;
        
        // Enable collider after a short delay to avoid spawn collisions
        StartCoroutine(EnableColliderAfterDelay(0.5f));
    }

    private void InitializeByWaveType()
    {
        switch (waveType)
        {
            case WaveType.FastAttack:
                InitializeFastAttack();
                break;
            case WaveType.Formation:
                InitializeFormation();
                break;
            case WaveType.HighIntensity:
                InitializeHighIntensity();
                break;
        }
    }

    private void InitializeFastAttack()
    {
        rb.linearVelocity = Vector2.left * fastSpeed;
        attackCooldown = 2f; // Less frequent attacks for fast enemies
    }

    private void InitializeFormation()
    {
        // Calculate formation position based on index
        float yOffset = (formationIndex - 1) * formationSpacing; // Center around index 1
        targetFormationPosition = startingPosition + formationOffset + Vector2.up * yOffset;
        rb.linearVelocity = Vector2.left * formationSpeed;
    }

    private void InitializeHighIntensity()
    {
        rb.linearVelocity = Vector2.left * highIntensitySpeed;
        attackCooldown = burstFireRate; // Rapid fire for high intensity
        maxHealth = 3; // Higher health for challenge
        currentHealth = maxHealth;
    }

    private void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (col != null) col.enabled = true;
    }

    private void Update()
    {
        UpdateStateTimer();
        UpdateMovement();
        UpdateAttacks();
        CheckBoundaries();
    }

    private void UpdateStateTimer()
    {
        stateTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;
    }

    private void UpdateMovement()
    {
        switch (waveType)
        {
            case WaveType.FastAttack:
                HandleFastAttackMovement();
                break;
            case WaveType.Formation:
                HandleFormationMovement();
                break;
            case WaveType.HighIntensity:
                HandleHighIntensityMovement();
                break;
        }
    }

    private void HandleFastAttackMovement()
    {
        // Simple fast movement toward player with slight tracking
        if (player != null && currentState == State.Attacking)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            Vector2 adjustedVelocity = Vector2.left * fastSpeed + directionToPlayer * (fastSpeed * 0.3f);
            rb.linearVelocity = adjustedVelocity;
        }
    }

    private void HandleFormationMovement()
    {
        switch (currentState)
        {
            case State.Entering:
                if (Vector2.Distance(transform.position, targetFormationPosition) < 1f)
                {
                    currentState = State.InFormation;
                    stateTimer = 0f;
                    rb.linearVelocity = Vector2.zero;
                }
                break;
                
            case State.InFormation:
                if (stateTimer >= formationWaitTime)
                {
                    currentState = State.Attacking;
                    rb.linearVelocity = Vector2.left * formationSpeed;
                }
                break;
                
            case State.Attacking:
                // Continue moving left in formation
                break;
        }
    }

    private void HandleHighIntensityMovement()
    {
        // Zigzag movement pattern
        float zigzagY = Mathf.Sin(Time.time * zigzagFrequency) * zigzagAmplitude;
        Vector2 baseVelocity = Vector2.left * highIntensitySpeed;
        Vector2 zigzagVelocity = Vector2.up * zigzagY * zigzagFrequency;
        
        rb.linearVelocity = baseVelocity + zigzagVelocity;
        
        // Change to attacking state quickly
        if (stateTimer >= 0.5f && currentState == State.Entering)
        {
            currentState = State.Attacking;
        }
    }

    private void UpdateAttacks()
    {
        if (currentState != State.Attacking || player == null) return;
        
        switch (waveType)
        {
            case WaveType.FastAttack:
                HandleFastAttackShooting();
                break;
            case WaveType.Formation:
                HandleFormationShooting();
                break;
            case WaveType.HighIntensity:
                HandleHighIntensityShooting();
                break;
        }
    }

    private void HandleFastAttackShooting()
    {
        if (attackTimer >= attackCooldown)
        {
            ShootAtPlayer();
            attackTimer = 0f;
        }
    }

    private void HandleFormationShooting()
    {
        if (attackTimer >= attackCooldown)
        {
            ShootStraight();
            attackTimer = 0f;
        }
    }

    private void HandleHighIntensityShooting()
    {
        if (attackTimer >= attackCooldown && !isAttacking)
        {
            StartCoroutine(BurstFire());
            attackTimer = 0f;
        }
    }

    private void ShootAtPlayer()
    {
        if (projectilePrefab == null || firePoint == null || player == null) return;
        
        Vector2 direction = (player.position - firePoint.position).normalized;
        FireProjectile(direction);
        
        Debug.Log($"🎯 EnemyShip2 ({waveType}) shooting at player");
    }

    private void ShootStraight()
    {
        if (projectilePrefab == null || firePoint == null) return;
        
        FireProjectile(Vector2.left);
        
        Debug.Log($"⬅️ EnemyShip2 ({waveType}) shooting straight");
    }

    private void FireProjectile(Vector2 direction)
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        
        if (projectileRb != null)
        {
            projectileRb.linearVelocity = direction * projectileSpeed;
        }
        
        // Play shoot sound
        if (AudioManager.instance != null && shootSound != null)
        {
            AudioManager.instance.sfxSource.PlayOneShot(shootSound);
        }
    }

    private IEnumerator BurstFire()
    {
        isAttacking = true;
        
        for (int i = 0; i < burstCount; i++)
        {
            if (player != null)
            {
                Vector2 direction = (player.position - firePoint.position).normalized;
                FireProjectile(direction);
            }
            
            yield return new WaitForSeconds(burstFireRate);
        }
        
        isAttacking = false;
    }

    private void CheckBoundaries()
    {
        if (transform.position.x < destroyXThreshold)
        {
            DestroyEnemy(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HandlePlayerCollision(other);
        }
        else if (other.CompareTag("bullet"))
        {
            HandleBulletHit(other);
        }
    }

    private void HandlePlayerCollision(Collider2D player)
    {
        PlayerController playerScript = player.GetComponent<PlayerController>();
        if (playerScript != null)
        {
            playerScript.TakeDamage(1);
        }
        
        DestroyEnemy(false);
    }

    private void HandleBulletHit(Collider2D bullet)
    {
        // Destroy the bullet
        Destroy(bullet.gameObject);
        
        // Apply damage
        TakeDamage();
        
        // Visual hit effect
        StartCoroutine(HitEffect());
        
        // Play hit sound
        if (AudioManager.instance != null && hitSound != null)
        {
            AudioManager.instance.sfxSource.PlayOneShot(hitSound);
        }
        
        Debug.Log($"💥 EnemyShip2 ({waveType}) hit! Health: {currentHealth}/{maxHealth}");
    }

    private void TakeDamage()
    {
        currentHealth--;
        
        if (currentHealth <= 0)
        {
            DestroyEnemy(true);
        }
    }

    private IEnumerator HitEffect()
    {
        if (spriteRenderer != null && whiteMaterial != null)
        {
            spriteRenderer.material = whiteMaterial;
            yield return new WaitForSeconds(0.1f);
            if (spriteRenderer != null) // Check if still exists
                spriteRenderer.material = defaultMaterial;
        }
    }

    private void DestroyEnemy(bool wasDestroyed)
    {
        if (wasDestroyed)
        {
            // Award score
            if (ScoreManager.instance != null)
            {
                ScoreManager.instance.AddScore(scoreValue, enemyType);
            }
            
            // Play destroy sound
            if (AudioManager.instance != null && destroySound != null)
            {
                AudioManager.instance.sfxSource.PlayOneShot(destroySound);
            }
            
            Debug.Log($"💀 EnemyShip2 ({waveType}) destroyed! Score awarded: {scoreValue}");
        }
        else
        {
            Debug.Log($"🏃 EnemyShip2 ({waveType}) exited screen or collided with player");
        }
        
        Destroy(gameObject);
    }

    // Public method to set wave configuration from spawner
    public void ConfigureWave(WaveType type, int index = 0)
    {
        waveType = type;
        formationIndex = index;
        
        // Reconfigure based on new type
        InitializeByWaveType();
        
        Debug.Log($"🔧 EnemyShip2 reconfigured: Wave Type = {waveType}, Formation Index = {formationIndex}");
    }

    // Public method for debugging and testing
    public WaveType GetWaveType()
    {
        return waveType;
    }

    public int GetFormationIndex()
    {
        return formationIndex;
    }
}
