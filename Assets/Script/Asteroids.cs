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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Hanya kurangi nyawa jika ditabrak Player atau bullet
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("bullet"))
        {
            TakeDamage();
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
        if(lives <= 0)
{
            if (destroyEffect != null)
            {
                // Instantiate dan simpan referensinya
                GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);

                // Hancurkan efek setelah durasi tertentu (misal 2 detik)
                Destroy(effect, 0.5f);
            }

            // Hancurkan asteroid
            Destroy(gameObject);
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
