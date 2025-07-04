using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class SimpleCrystalShard : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float rotationSpeed = 360f; // Rotation for visual effect
    [SerializeField] private bool useManualMovement = false; // Fallback if Rigidbody fails
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private TrailRenderer trail;
    
    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    
    private bool hasHit = false;
    private Vector2 moveDirection;
    private float moveSpeed;
    
    private void Start()
    {
        // Set up rigidbody first
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            Debug.Log("🔧 Adding missing Rigidbody2D to Crystal Shard");
        }
        
        // Important physics settings for projectile
        rb.gravityScale = 0; // No gravity for space projectile
        rb.linearDamping = 0; // No air resistance
        rb.angularDamping = 0; // No rotational drag
        rb.freezeRotation = false; // Allow visual rotation
        
        // Set up collider as trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>();
            Debug.Log("🔧 Adding missing Collider2D to Crystal Shard");
        }
        col.isTrigger = true;
        
        // Check if we need manual movement fallback
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            Debug.LogWarning("⚠️ Rigidbody velocity is too low, enabling manual movement fallback");
            useManualMovement = true;
            
            // Find player and set manual direction
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                moveDirection = (playerObj.transform.position - transform.position).normalized;
                moveSpeed = 8f; // Default speed
                Debug.Log($"🎯 Manual movement - Direction: {moveDirection}, Speed: {moveSpeed}");
            }
        }
        
        // Debug projectile status
        Debug.Log($"🔮 Crystal Shard initialized:");
        Debug.Log($"   Position: {transform.position}");
        Debug.Log($"   Velocity: {rb.linearVelocity}");
        Debug.Log($"   Manual Movement: {useManualMovement}");
        Debug.Log($"   Gravity Scale: {rb.gravityScale}");
        Debug.Log($"   Is Trigger: {col.isTrigger}");
        
        // Auto-destroy after 10 seconds
        Destroy(gameObject, 10f);
    }
    
    private void Update()
    {
        // Rotate for visual effect
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        
        // Manual movement fallback
        if (useManualMovement && !hasHit)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = moveDirection.normalized * moveSpeed;
            }
        }
        
        // Debug velocity setiap 2 detik
        if (Time.time % 2 < 0.1f)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Debug.Log($"🔮 Crystal Shard - Position: {transform.position}, Velocity: {rb.linearVelocity}, Manual: {useManualMovement}");
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return; // Prevent multiple hits
        
        Debug.Log($"🔮 Crystal Shard hit: {other.gameObject.name} with tag: {other.tag}");
        
        if (other.CompareTag("Player"))
        {
            hasHit = true;
            
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log($"💥 Crystal Shard mengenai Player! Damage: {damage}");
                player.TakeDamage(damage);
            }
            
            PlayHitEffects();
            Destroy(gameObject);
        }
        else if (other.CompareTag("Boundary") || other.CompareTag("Wall"))
        {
            Debug.Log("🔮 Crystal Shard hit boundary/wall - destroyed");
            hasHit = true;
            PlayHitEffects();
            Destroy(gameObject);
        }
        // Player can destroy crystal shards with bullets
        else if (other.CompareTag("bullet"))
        {
            Debug.Log("🔮 Crystal Shard dihancurkan oleh peluru Player!");
            hasHit = true;
            Destroy(other.gameObject); // Destroy player bullet
            PlayHitEffects();
            Destroy(gameObject);
        }
    }
    
    private void PlayHitEffects()
    {
        // Spawn hit effect
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        // Play hit sound
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }
    }
    
    // Method untuk set velocity dari Boss2Controller
    public void SetVelocity(Vector2 direction, float speed)
    {
        Debug.Log($"🔮 SetVelocity called - Direction: {direction}, Speed: {speed}");
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
            Debug.Log($"🔮 Velocity set via Rigidbody: {rb.linearVelocity}");
            
            // Check if velocity was set successfully
            if (rb.linearVelocity.magnitude < 0.1f)
            {
                Debug.LogWarning("⚠️ Rigidbody velocity failed, enabling manual movement");
                useManualMovement = true;
                moveDirection = direction.normalized;
                moveSpeed = speed;
            }
            else
            {
                useManualMovement = false;
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No Rigidbody2D found, using manual movement only");
            useManualMovement = true;
            moveDirection = direction.normalized;
            moveSpeed = speed;
        }
    }
    
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
        Debug.Log($"💥 Crystal Shard damage set to: {damage}");
    }
}
