using UnityEngine;

public class Boss3LaserSpread : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private int bulletCount = 5;
    [SerializeField] private float spreadAngle = 40f;
    [SerializeField] private int bulletDamage = 1;

    public void Shoot()
    {
        // Sudut awal: spreadAngle / 2 ke kiri
        float startAngle = -spreadAngle / 2f;
        float angleStep = spreadAngle / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.left;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Boss3Bullets bulletScript = bullet.GetComponent<Boss3Bullets>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(direction, bulletSpeed, bulletDamage);
            }
        }
    }
}
