using UnityEngine;

public class LaserBullets : MonoBehaviour
{
    // Variabel untuk menyimpan data milik peluru ini sendiri
    private Vector2 moveDirection;
    private float bulletSpeed;
    private float bulletDamage; // Variabel damage sudah disimpan, siap digunakan

    /// <summary>
    /// Inisialisasi peluru dengan arah, kecepatan, dan kerusakan.
    /// Metode ini dipanggil oleh LaserWeapon saat peluru dibuat.
    /// </summary>
    public void Initialize(Vector2 shootDirection, float speed, float damage)
    {
        this.moveDirection = shootDirection.normalized;
        this.bulletSpeed = speed;
        this.bulletDamage = damage;
    }

    void Update()
    {
        // Gerakkan peluru menggunakan speed-nya sendiri, bukan dari LaserWeapon.Instance
        transform.position += (Vector3)(moveDirection * bulletSpeed * Time.deltaTime);

        // Hancurkan peluru jika keluar layar
        if (transform.position.x > 55)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Jika menabrak obstacle, hancurkan diri sendiri
        if (collision.gameObject.CompareTag("obstacle"))
        {
            Destroy(gameObject);
        }

        // CONTOH PENGGUNAAN DAMAGE: Jika menabrak musuh
        // if (collision.gameObject.CompareTag("Enemy"))
        // {
        //     // Ambil komponen health dari musuh dan berikan damage
        //     EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        //     if (enemy != null)
        //     {
        //         enemy.TakeDamage(this.bulletDamage);
        //     }
            
        //     // Hancurkan peluru setelah mengenai musuh
        //     Destroy(gameObject);
        // }
    }
}