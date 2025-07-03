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

        if (transform.position.x > 55) // batas luar layar kanan
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Jika mengenai obstacle, hancurkan peluru dan hentikan proses.
        if (other.CompareTag("obstacle"))
        {
            Destroy(gameObject);
            return;
        }

        // Coba cari komponen perisai terlebih dahulu.
        EnemyShield shield = other.GetComponent<EnemyShield>();
        if (shield != null)
        {
            // Biarkan perisai menyerap damage dan beri tahu sisa damage-nya.
            int remainingDamage = shield.AbsorbDamage((int)this.bulletDamage);

            // Jika ada damage yang menembus perisai, berikan ke musuh di baliknya.
            if (remainingDamage > 0)
            {
                // Kita asumsikan perisai adalah komponen anak dari entitas yang bisa rusak.
                IDamageable underlyingEnemy = other.GetComponentInParent<IDamageable>();
                if (underlyingEnemy != null)
                {
                    underlyingEnemy.TakeDamage(remainingDamage);
                }
            }

            // Peluru selalu hancur saat mengenai perisai.
            Destroy(gameObject);
            return; // Hentikan proses lebih lanjut.
        }

        // Jika bukan perisai, cek apakah objek lain bisa menerima damage.
        // Ini akan menangani musuh tanpa perisai atau objek lain yang mengimplementasikan IDamageable.
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            // Berikan damage penuh.
            damageable.TakeDamage((int)this.bulletDamage);

            // Hancurkan peluru setelah mengenai target yang bisa rusak.
            Destroy(gameObject);
            // Tidak perlu 'return' di sini karena ini adalah kondisi terakhir sebelum tidak terjadi apa-apa.
        }
    }
}
