using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    // Kecepatan pergerakan kamera
    public float cameraSpeed = 8f;
    // Posisi scroll saat ini
    float currentScrollPosition = 0f;

    void Start()
    {
        // Inisialisasi posisi scroll berdasarkan arah scroll yang ditentukan di SpaceManager
        switch (SpaceManager.instance.scrollDirection)
        {
            case ScrollDirection.LeftToRight:
            case ScrollDirection.RightToLeft:
                // Untuk pergerakan horizontal, gunakan posisi X
                currentScrollPosition = transform.position.x / cameraSpeed;
                break;
            case ScrollDirection.DownToUp:
            case ScrollDirection.UpToDown:
                // Untuk pergerakan vertikal, gunakan posisi Y
                currentScrollPosition = transform.position.y / cameraSpeed;
                break;
        }
    }

    void Update()
    {
        // Perbarui posisi scroll berdasarkan waktu
        currentScrollPosition += Time.deltaTime;
        Vector3 newPosition = Vector3.zero;

        // Hitung posisi baru kamera berdasarkan arah scroll
        switch (SpaceManager.instance.scrollDirection)
        {
            case ScrollDirection.LeftToRight:
                // Geser kamera ke kanan
                newPosition = new Vector3(
                    Mathf.Lerp(transform.position.x, cameraSpeed * currentScrollPosition, 1f * Time.deltaTime),
                    transform.position.y,
                    transform.position.z
                );
                break;
            case ScrollDirection.RightToLeft:
                // Geser kamera ke kiri
                newPosition = new Vector3(
                    Mathf.Lerp(transform.position.x, -cameraSpeed * currentScrollPosition, 1f * Time.deltaTime),
                    transform.position.y,
                    transform.position.z
                );
                break;
            case ScrollDirection.DownToUp:
                // Geser kamera ke atas
                newPosition = new Vector3(
                    transform.position.x,
                    Mathf.Lerp(transform.position.y, cameraSpeed * currentScrollPosition, 1f * Time.deltaTime),
                    transform.position.z
                );
                break;
            case ScrollDirection.UpToDown:
                // Geser kamera ke bawah
                newPosition = new Vector3(
                    transform.position.x,
                    Mathf.Lerp(transform.position.y, -cameraSpeed * currentScrollPosition, 1f * Time.deltaTime),
                    transform.position.z
                );
                break;
        }

        // Terapkan posisi baru ke kamera
        transform.position = newPosition;
    }
}