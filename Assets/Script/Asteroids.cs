using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Asteroids : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // Komponen untuk menampilkan sprite asteroid
    private Rigidbody2D rb; // Komponen fisika untuk mengatur gerakan asteroid

    private Material defaultMaterial; // Material default sprite untuk efek visual
    [SerializeField] private Material whiteMaterial; // Material putih untuk efek saat terkena tabrakan
    [SerializeField] private Sprite[] sprites; // Array sprite asteroid yang bisa dipilih secara acak
    [SerializeField] private float naturalDriftY = 0.5f; // Kecepatan drift alami pada sumbu Y
    [SerializeField] private float rotationTorque = 30f; // Besar torsi rotasi acak
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private int lives;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // Ambil komponen SpriteRenderer
        rb = GetComponent<Rigidbody2D>(); // Ambil komponen Rigidbody2D
        defaultMaterial = spriteRenderer.material; // Simpan material default untuk nanti

        // Pilih sprite secara acak dari array sprites untuk variasi tampilan asteroid
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

        // Tentukan drift alami ke atas atau ke bawah secara acak
        float driftY = Random.Range(-naturalDriftY, naturalDriftY);
        rb.linearVelocity = new Vector2(0, driftY); // Set kecepatan linear pada sumbu Y

        // Berikan torsi rotasi acak agar asteroid berputar secara natural
        float torque = Random.Range(-rotationTorque, rotationTorque);
        rb.AddTorque(torque);

        // Gravity, drag, dan pengaturan fisika lain dinonaktifkan melalui Inspector Unity
    }

    void Update()
    {
        // Hitung perpindahan horizontal berdasarkan kecepatan dunia dan multiplier boost pemain
        float moveX = (GameManager.instance.worldSpeed * PlayerController.instance.BoostMultiplier) * Time.deltaTime;
        // Geser posisi asteroid ke kiri sesuai kecepatan yang dihitung
        transform.position += new Vector3(-moveX, 0);

        // Jika asteroid sudah melewati batas kiri layar (x < -60), hancurkan objek untuk menghemat resource
        if (transform.position.x < -60f)
        {
            Destroy(gameObject);
        }
    }

    private void TakeDamage()
    {
        lives--;

        // Flash putih
        if (whiteMaterial != null && spriteRenderer != null)
        {
            spriteRenderer.material = whiteMaterial;
            StartCoroutine(ResetMaterial());
        }

        // Jika habis nyawa, hancurkan asteroid
        if (lives <= 0)
        {
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Hanya kurangi nyawa jika ditabrak Player atau bullet
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("bullet"))
        {
            TakeDamage();
        }

        // Jika asteroid bertabrakan dengan pemain atau peluru
        // if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("bullet"))
        // {
        //     // Ganti material sprite menjadi putih sebagai efek visual terkena tabrakan
        //     spriteRenderer.material = whiteMaterial;
        //     // Mulai coroutine untuk mengembalikan material ke default setelah delay
        //     StartCoroutine("ResetMaterial");
        // }
    }

    private IEnumerator ResetMaterial()

    {
        // Tunggu selama 0.2 detik sebelum mengembalikan material ke default
        yield return new WaitForSeconds(0.2f);
        if (spriteRenderer != null && defaultMaterial != null)
        {
            spriteRenderer.material = defaultMaterial;
        }
    }
}