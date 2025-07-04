using UnityEngine;

public class Boss3LaserSpiral : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.1f;
    public float bulletSpeed = 6f;
    public float angleIncrement = 10f;

    private float currentAngle = 0f;
    private float fireTimer = 0f;
    private bool isFiring = false;

    void Update()
    {
        if (!isFiring) return;

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            ShootSpiral();
        }
    }

    void ShootSpiral()
    {
        float radian = currentAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Boss3Bullets>().Initialize(direction.normalized, bulletSpeed);

        currentAngle += angleIncrement;
        if (currentAngle >= 360f) currentAngle -= 360f;
    }

    // 🟢 Dipanggil dari Boss3Controller
    public void StartFiring()
    {
        isFiring = true;
        fireTimer = 0f;
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
