using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    private Vector2 moveDirection;
    private float bulletSpeed;
    private int bulletDamage;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    public void Initialize(Vector2 shootDirection, float speed, int damage)
    {
        this.moveDirection = shootDirection.normalized;
        this.bulletSpeed = speed;
        this.bulletDamage = damage;

        // Arahkan proyektil ke arah gerakannya
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void Update()
    {
        transform.position += (Vector3)moveDirection * bulletSpeed * Time.deltaTime;

        // Hancurkan jika keluar dari pandangan kamera (lebih fleksibel)
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        if (viewportPos.x < -0.1f || viewportPos.x > 1.1f || viewportPos.y < -0.1f || viewportPos.y > 1.1f)
        {
            // Kita gunakan buffer 0.1 agar peluru benar-benar hilang dari pandangan sebelum dihancurkan.
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<IDamageable>()?.TakeDamage(bulletDamage);
            Destroy(gameObject);
        }
    }
}