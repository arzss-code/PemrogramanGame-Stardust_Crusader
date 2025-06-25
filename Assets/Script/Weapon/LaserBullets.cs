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
        // Jika mengenai obstacle
        if (other.CompareTag("obstacle"))
        {
            // Hancurkan peluru
            Destroy(gameObject);
            return; // Keluar dari fungsi agar tidak menjalankan kode di bawahnya
        }

        // // Jika mengenai Asteroid
        // if (other.CompareTag("Asteroid"))
        // {
        //     // Ambil komponen script Asteroids dari objek yang ditabrak
        //     Asteroids asteroid = other.GetComponent<Asteroids>();
        //     if (asteroid != null)
        //     {
        //         // Panggil fungsi TakeDamage pada asteroid
        //         // (Kita akan upgrade sedikit script Asteroid agar bisa menerima damage)
        //         asteroid.TakeDamage((int)this.bulletDamage);
        //     }
            
        //     // Hancurkan peluru setelah mengenai asteroid
        //     Destroy(gameObject);
        //     return;
        // }

        // Jika mengenai Boss
        if (other.CompareTag("Boss"))
        {
            // Ambil komponen script BossController
            BossController boss = other.GetComponent<BossController>();
            if (boss != null)
            {
                // Panggil fungsi TakeDamage pada boss dengan damage dari peluru ini
                boss.TakeDamage((int)this.bulletDamage);
            }
            
            // Hancurkan peluru setelah mengenai boss
            Destroy(gameObject);
            return;
        }
    }
}