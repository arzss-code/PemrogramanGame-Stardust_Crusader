using UnityEngine;

public class CrystalShard : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 1; // Hanya 1 damage untuk balance
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private Material whiteMaterial;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;
    
    [Header("Audio")]
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;
    
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        
        if (spriteRenderer != null)
        {
            defaultMaterial = spriteRenderer.material;
        }
        
        // Auto-destroy setelah 8 detik jika tidak mengenai apa-apa
        Destroy(gameObject, 8f);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Hanya 1 damage agar tidak terlalu overpowered
                player.TakeDamage(damage);
            }
            
            // Play hit effect dan sound
            PlayHitEffects();
            
            // Destroy crystal shard
            Destroy(gameObject);
        }
        else if (other.CompareTag("Boundary") || other.CompareTag("Wall"))
        {
            // Hancurkan jika mengenai boundary/wall
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
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
    
    // Method untuk visual feedback jika terkena tembakan player
    public void OnHitByBullet()
    {
        if (spriteRenderer != null && whiteMaterial != null)
        {
            spriteRenderer.material = whiteMaterial;
            Invoke(nameof(ResetMaterial), 0.1f);
        }
    }
    
    private void ResetMaterial()
    {
        if (spriteRenderer != null && defaultMaterial != null)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }
}
