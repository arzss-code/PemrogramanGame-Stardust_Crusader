using UnityEngine;

public class LaserWeapon : MonoBehaviour
{
    public static LaserWeapon Instance;

    [Header("Weapon Configuration")]
    [SerializeField] private GameObject prefab;
    [Tooltip("Titik di mana peluru akan muncul. Buat Empty Object sebagai child dari Player.")]
    [SerializeField] private Transform firePoint; // Menggantikan posisi hardcode

    [Header("Weapon Stats")]
    [Tooltip("Variabel ini sebaiknya dipindahkan ke script peluru (LaserBullets)")]
    public float speed;
    [Tooltip("Variabel ini sebaiknya dipindahkan ke script peluru (LaserBullets)")]
    public float damage;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Shoot()
    {
        // Selalu cek jika firePoint sudah di-assign untuk menghindari error
        if (firePoint == null)
        {
            Debug.LogError("Fire Point belum di-set di Inspector!");
            return;
        }

        // Cek status boost dari PlayerController
        if (PlayerController.instance.IsWeaponBoosted)
        {
            // --- LOGIKA TEMBAKAN BOOSTED (SPREAD SHOT 3 ARAH) ---

            // Hitung arah untuk setiap tembakan
            Vector2 mainDirection = Vector2.right; // Arah lurus
            Vector2 upDirection = Quaternion.Euler(0, 0, 15) * Vector2.right;   // Arah diputar 15 derajat ke atas
            Vector2 downDirection = Quaternion.Euler(0, 0, -15) * Vector2.right; // Arah diputar 15 derajat ke bawah

            // Tembakkan tiga peluru dengan arah yang berbeda
            FireOneShot(mainDirection);
            FireOneShot(upDirection);
            FireOneShot(downDirection);
        }
        else
        {
            // --- LOGIKA TEMBAKAN NORMAL ---
            FireOneShot(Vector2.right);
        }
    }

    /// <summary>
    /// Fungsi bantuan untuk membuat satu peluru dan menginisialisasinya dengan arah tertentu.
    /// </summary>
    /// <param name="direction">Arah peluru akan bergerak.</param>
    private void FireOneShot(Vector2 direction)
    {
        // Gunakan posisi dari firePoint, bukan posisi player + offset
        GameObject bullet = Instantiate(prefab, firePoint.position, Quaternion.identity);

        LaserBullets bulletScript = bullet.GetComponent<LaserBullets>();
        if (bulletScript != null)
        {
            // --- BARIS INI DIUBAH ---
            // Kirimkan juga speed dan damage yang ada di LaserWeapon ke setiap peluru
            bulletScript.Initialize(direction, this.speed, this.damage);
        }
    }
}
