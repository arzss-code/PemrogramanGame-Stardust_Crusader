using UnityEngine;

public class EnemyLaserWeapon : MonoBehaviour
{

    [SerializeField] private EnemyLaserWeapon weapon;

    [Header("Weapon Configuration")]
    [SerializeField] private GameObject bulletPrefab;
    [Tooltip("Titik tembak, sebaiknya child dari Enemy (seperti moncong).")]
    [SerializeField] private Transform firePoint;

    [Header("Bullet Stats")]
    [Tooltip("Kecepatan peluru.")]
    public float bulletSpeed = 10f;
    [Tooltip("Damage dari peluru.")]
    public float bulletDamage = 1f;

    [Header("Auto Fire Settings")]
    [Tooltip("Interval antar tembakan (detik).")]
    public float fireInterval = 2f;
    [Tooltip("Delay sebelum tembakan pertama.")]
    public float initialDelay = 1f;

    private float fireTimer = 0f;
    private bool canShoot = false;

    private void Start()
    {
        // Mulai delay awal sebelum bisa menembak
        Invoke(nameof(EnableShooting), initialDelay);
    }

    private void Update()
    {
        if (!canShoot) return;

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            Shoot();
        }
    }

    private void EnableShooting()
    {
        canShoot = true;
    }

    /// <summary>
    /// Menembakkan satu peluru ke arah kiri.
    /// </summary>
    public void Shoot()
    {
        if (firePoint == null)
        {
            Debug.LogError("❌ FirePoint belum diset pada EnemyWeapon!");
            return;
        }

        FireOneShot(Vector2.left); // Arah peluru ke kiri
    }

    /// <summary>
    /// Fungsi bantuan untuk instantiate dan inisialisasi peluru.
    /// </summary>
    private void FireOneShot(Vector2 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Enemy3Bullets bulletScript = bullet.GetComponent<Enemy3Bullets>();

        if (bulletScript != null)
        {
            bulletScript.Initialize(direction, bulletSpeed, bulletDamage);
        }
        else
        {
            Debug.LogWarning("Peluru tidak memiliki script Enemy3Bullets!");
        }
    }
}
