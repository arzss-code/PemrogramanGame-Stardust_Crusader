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
        // 🚫 Obstacle
        if (other.CompareTag("obstacle"))
        {
            Destroy(gameObject);
            return;
        }

        // 🎯 Kena Shield duluan
        if (other.CompareTag("EnemyShield"))
        {
            EnemyShield shield = other.GetComponent<EnemyShield>();
            if (shield != null)
            {
                int remainingDamage = shield.AbsorbDamage((int)bulletDamage);

                if (remainingDamage <= 0)
                {
                    Destroy(gameObject); // Damage sudah habis ke shield
                    return;
                }

                // Damage tersisa? Cari EnemyShip3 (parent dari shield)
                EnemyShip3 enemy = other.GetComponentInParent<EnemyShip3>();
                if (enemy != null)
                {
                    enemy.TakeDamage(remainingDamage);
                }

                Destroy(gameObject);
                return;
            }
        }

        // 🎯 Langsung ke musuh (jika shield sudah hancur)
        if (other.CompareTag("enemy"))
        {
            EnemyShip3 enemy = other.GetComponent<EnemyShip3>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)bulletDamage);
            }

            Destroy(gameObject);
            return;
        }
    }


}
