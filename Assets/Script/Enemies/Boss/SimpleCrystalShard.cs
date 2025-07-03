using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class SimpleCrystalShard : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float rotationSpeed = 360f; // Rotation for visual effect
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private TrailRenderer trail;
    
    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    
    private bool hasHit = false;
    
    private void Start()
    {
        // Set up collider as trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
        
        // Set up rigidbody
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0; // No gravity for space projectile
        }
        
        // Auto-destroy after 10 seconds
        Destroy(gameObject, 10f);
        
        Debug.Log("🔮 Crystal Shard Bullet ready to fire!");
    }
    
    private void Update()
    {
        // Rotate for visual effect
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
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
}
