using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Asteroids : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D col;

    private Material defaultMaterial;
    [SerializeField] private Material whiteMaterial;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float naturalDriftY = 0.5f;
    [SerializeField] private float rotationTorque = 30f;
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private int lives;

    [Header("Collider Delay")]
    [SerializeField] private float colliderDelay = 1f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        col.enabled = false; // Nonaktifkan collider dulu
        StartCoroutine(EnableColliderAfterDelay(colliderDelay)); // Aktifkan setelah delay

        defaultMaterial = spriteRenderer.material;
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];

        float driftY = Random.Range(-naturalDriftY, naturalDriftY);
        rb.linearVelocity = new Vector2(0, driftY);

        float torque = Random.Range(-rotationTorque, rotationTorque);
        rb.AddTorque(torque);
    }

    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (col != null) col.enabled = true;
    }

    void Update()
    {
        float moveX = (GameManager.instance.worldSpeed * PlayerController.instance.BoostMultiplier) * Time.deltaTime;
        transform.position += new Vector3(-moveX, 0);

        if (transform.position.x < -60f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1);
            }
            DestroyAsteroid();
        }
        else if (other.CompareTag("bullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }

    private void DestroyAsteroid()
    {
        if (destroyEffect != null)
        {
            GameObject effect = Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(effect, 0.5f);
        }
        Destroy(gameObject);
    }

    public void TakeDamage(int damageAmount)
    {
        lives -= damageAmount;

        if (whiteMaterial != null && spriteRenderer != null)
        {
            spriteRenderer.material = whiteMaterial;
            StartCoroutine(ResetMaterial());
        }

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
