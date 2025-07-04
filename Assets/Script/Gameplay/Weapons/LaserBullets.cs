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
        if (other.CompareTag("obstacle"))
        {
            Destroy(gameObject);
            return;
        }

        // REFAKTOR: Cek apakah objek yang ditabrak memiliki komponen shield (apapun jenisnya)
        // dengan mencari antarmuka IShield. Ini akan bekerja untuk EnemyShield dan BossRegenShield.
        IShield shield = other.GetComponent<IShield>();
        if (shield != null)
        {
            // Shield akan menyerap damage. Sisa damage (jika ada) akan diteruskan.
            int remainingDamage = shield.AbsorbDamage((int)this.bulletDamage);
            if (remainingDamage > 0)
            {
                IDamageable underlyingEnemy = other.GetComponentInParent<IDamageable>();
                if (underlyingEnemy != null)
                {
                    underlyingEnemy.TakeDamage(remainingDamage);
                }
            }

            Destroy(gameObject);
            return;
        }

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage((int)this.bulletDamage);
            Destroy(gameObject);
            return;
        }

        // ✅ Tambahan fallback untuk EnemyShip3
        EnemyShip3 enemyShip3 = other.GetComponent<EnemyShip3>();
        if (enemyShip3 != null)
        {
            enemyShip3.TakeDamage((int)this.bulletDamage);
            Destroy(gameObject);
        }
    }

}
