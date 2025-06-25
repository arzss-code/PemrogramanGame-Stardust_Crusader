using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Asteroids : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private Material defaultMaterial;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float naturalDriftY = 0.5f; // Gerakan Y alami
    [SerializeField] private float rotationTorque = 30f;
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private int lives;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        defaultMaterial = spriteRenderer.material;

        // Random sprite
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

        // Drift natural ke atas/bawah
        float driftY = Random.Range(-naturalDriftY, naturalDriftY);
        rb.linearVelocity = new Vector2(0, driftY);

        // Rotasi acak
        float torque = Random.Range(-rotationTorque, rotationTorque);
        rb.AddTorque(torque);

        // Gravity, drag, dll dinonaktifkan via Inspector
    }

    void Update()
    {
        // Efek worldSpeed dan BoostMultiplier tetap dipakai
        float moveX = (GameManager.instance.worldSpeed * PlayerController.instance.BoostMultiplier) * Time.deltaTime;
        transform.position += new Vector3(-moveX, 0);

        // Hancurkan jika terlalu kiri
        if (transform.position.x < -60f)
        {
            Destroy(gameObject);
        }
    }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     // Hanya kurangi nyawa jika ditabrak Player atau bullet
    //     if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("bullet"))
    //     {
    //         TakeDamage();
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // --- BLOK KODE BARU DIMULAI DI SINI ---

            // 1. Ambil komponen PlayerController dari objek yang kita sentuh.
            PlayerController player = other.GetComponent<PlayerController>();

            // 2. Jika komponennya berhasil ditemukan (untuk menghindari error),
            //    panggil fungsinya untuk mengurangi nyawa player.
            if (player != null)
            {
                player.TakeDamage(1); // Memberi 1 damage pada player
            }

            // --- BLOK KODE BARU SELESAI ---

            // 3. Setelah memberi damage, hancurkan asteroid seperti sebelumnya.
            DestroyAsteroid();
        }
        else if (other.CompareTag("bullet"))
        {
            // Logika untuk peluru tidak berubah
            TakeDamage(1);
            Destroy(other.gameObject); 
        }
    }
    
    // Saya pindahkan logika hancur ke method sendiri agar rapi
    private void DestroyAsteroid()
    {
        // Melapor ke LevelController bahwa satu musuh telah dikalahkan
        if (LevelController.main != null)
        {
            LevelController.main.EnemyDefeated();
        }
        if (destroyEffect != null)
        {
            GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(effect, 0.5f); // Hancurkan efek setelah durasinya
        }
        // Hancurkan game object asteroid ini
        Destroy(gameObject);
    }

    // Ubah menjadi public dan tambahkan parameter
    public void TakeDamage(int damageAmount)
    {
        lives -= damageAmount; // Kurangi nyawa sesuai damage yang diterima
        
        // Flash putih
        if (whiteMaterial != null && spriteRenderer != null)
        {
            spriteRenderer.material = whiteMaterial;
            StartCoroutine(ResetMaterial());
        }

        // Jika habis nyawa, hancurkan asteroid
        if (lives <= 0)
        {
            DestroyAsteroid();
        }
    }

    private IEnumerator ResetMaterial()
    {
        yield return new WaitForSeconds(0.2f);
        if (spriteRenderer != null && defaultMaterial != null)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }

}
