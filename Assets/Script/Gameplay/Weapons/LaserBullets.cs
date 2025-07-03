using UnityEngine;

public class LaserBullets : MonoBehaviour
{
    private Vector2 moveDirection;
    private float bulletSpeed;
    private float bulletDamage;

    public void Initialize(Vector2 shootDirection, float speed, float damage)
    {
        this.moveDirection = shootDirection.normalized;
        this.bulletSpeed = speed;
        this.bulletDamage = damage;
    }

    void Update()
    {
        transform.position += (Vector3)(moveDirection * bulletSpeed * Time.deltaTime);

        if (transform.position.x > 55)
        {
            Destroy(gameObject);
        }
    }

    // Mengganti OnCollisionEnter2D menjadi OnTriggerEnter2D
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah objek yang terkena bisa menerima damage
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            // Berikan damage sesuai damage peluru
            damageable.TakeDamage((int)this.bulletDamage);
            // Hancurkan peluru setelah mengenai target
            Destroy(gameObject);
            return;
        }

        // Jika mengenai obstacle, hancurkan peluru saja
        if (other.CompareTag("obstacle"))
        {
            Destroy(gameObject);
        }
    }
}