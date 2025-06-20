using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingStar : MonoBehaviour
{
    // Waktu yang diperlukan untuk mengaktifkan bintang jatuh setelah dibuat
    [Range(0f, 30.0f)]
    public float spawnTime = 4f;
    float currentSpawnTime;

    // Nilai kecepatan untuk menggerakkan bintang jatuh
    [Range(0.3f, 10.0f)]
    public float speed = 1f;
    float currentSpeed;

    // Jika tidak diaktifkan, bintang jatuh tidak akan bergerak
    bool activated = false;

    // Menentukan apakah spawnTime dan speed harus diacak saat pembuatan
    public bool randomize = true;

    // Faktor modifikasi jika kamera tidak ortogonal
    float modificator = 1f;
    // Jarak dari kamera, digunakan jika kamera tidak ortogonal
    float cameraDistance = 10f;

    // Arah pergerakan bintang jatuh
    ScrollDirection direction = ScrollDirection.LeftToRight;

    void Start()
    {
        // Mengambil arah scroll dari SpaceManager jika tersedia
        if (SpaceManager.instance != null)
        {
            direction = SpaceManager.instance.scrollDirection;
        }

        // Menyesuaikan pengaturan jika kamera tidak ortogonal
        if (!Camera.main.orthographic)
        {
            modificator = Mathf.Max(Screen.width, Screen.height);
            cameraDistance = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
        }
        // Menggunakan nilai tetap jika tidak diacak
        if (!randomize)
        {
            currentSpawnTime = spawnTime;
            currentSpeed = speed;
        }
        // Memulai proses pembuatan bintang jatuh
        Generate();
    }

    public void Generate()
    {
        // Menonaktifkan bintang jatuh
        Activate(false);
        // Mengacak waktu spawn dan kecepatan jika diizinkan
        if (randomize)
        {
            currentSpawnTime = Random.Range(0.3f, spawnTime);
            currentSpeed = Random.Range(0.3f, speed);
        }
        // Menunggu selama currentSpawnTime sebelum mengaktifkan kembali bintang jatuh
        StartCoroutine(waitToActivate(currentSpawnTime));
    }

    IEnumerator waitToActivate(float waitTime)
    {
        // Menunggu selama waktu yang ditentukan
        yield return new WaitForSeconds(waitTime);
        // Setelah menunggu, aktifkan bintang jatuh
        Activate(true);
    }

    // Mengaktifkan atau menonaktifkan pergerakan bintang jatuh
    public void Activate(bool activate)
    {
        activated = activate;
        if (activated)
        {
            // Setelah diaktifkan, beri bintang jatuh posisi baru
            Vector3 newPosition = Vector3.zero;

            // Menentukan posisi baru berdasarkan arah scroll
            switch (direction)
            {
                case ScrollDirection.LeftToRight:
                    if (Camera.main.orthographic)
                        newPosition = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0f, 1.5f), (Random.Range(0, 100) < 50 ? -0.5f : 1.5f), 0f));
                    else
                        newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(-0.5f, 1f) * modificator, (Random.Range(0, 100) < 50 ? -0.5f : 1.5f) * modificator, cameraDistance));
                    break;
                case ScrollDirection.RightToLeft:
                    if (Camera.main.orthographic)
                        newPosition = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(-0.5f, 1f), (Random.Range(0, 100) < 50 ? -0.5f : 1.5f), 0f));
                    else
                        newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(-0.5f, 1f) * modificator, (Random.Range(0, 100) < 50 ? -0.5f : 1.5f) * modificator, cameraDistance));
                    break;
                case ScrollDirection.DownToUp:
                    if (Camera.main.orthographic)
                        newPosition = Camera.main.ViewportToWorldPoint(new Vector3((Random.Range(0, 100) < 50 ? -0.5f : 1.5f), Random.Range(0f, 1.5f), 0f));
                    else
                        newPosition = Camera.main.ScreenToWorldPoint(new Vector3((Random.Range(0, 100) < 50 ? -0.5f : 1.5f) * modificator, Random.Range(0f, 1.5f) * modificator, cameraDistance));
                    break;
                case ScrollDirection.UpToDown:
                    if (Camera.main.orthographic)
                        newPosition = Camera.main.ViewportToWorldPoint(new Vector3((Random.Range(0, 100) < 50 ? -0.5f : 1.5f), Random.Range(-0.5f, 1f), 0f));
                    else
                        newPosition = Camera.main.ScreenToWorldPoint(new Vector3((Random.Range(0, 100) < 50 ? -0.5f : 1.5f) * modificator, Random.Range(-0.5f, 1f) * modificator, cameraDistance));
                    break;
            }
            // Menetapkan posisi baru untuk bintang jatuh
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
            // Menentukan arah yang akan dituju oleh bintang jatuh
            Vector3 forwardDirection = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0f) - transform.position;
            // Memastikan arah depan tidak mengubah posisi pada sumbu z
            transform.forward = new Vector3(forwardDirection.x, forwardDirection.y, 0f);
        }
    }

    void Update()
    {
        // Jika tidak diaktifkan, jangan lakukan update
        if (!activated) return;
        // Menggerakkan bintang jatuh sesuai kecepatannya
        transform.position += transform.forward * currentSpeed;
        // Memeriksa apakah bintang jatuh telah mencapai batas untuk diregenerasi
        Rect rect = new Rect(-1f, -1f, 3f, 3f);
        if (!Camera.main.orthographic)
        {
            // Untuk kamera perspektif, hitung area deteksi keluar layar berdasarkan ukuran layar dan jarak kamera
            Vector3 rectPosition = Camera.main.ScreenToWorldPoint(new Vector3(-1f * modificator, -1f * modificator, 10f));
            Vector3 rectSize = Camera.main.ScreenToWorldPoint(new Vector3(3f * modificator, 3f * modificator, 10f));
            rect = new Rect(rectPosition.x, rectPosition.y, rectSize.x, rectSize.y);

            // Jika posisi bintang jatuh keluar dari area, mulai proses regenerasi
            if (!rect.Contains(transform.position))
            {
                Generate();
            }
        }
        else
        {
            // Untuk kamera ortogonal, cek apakah posisi bintang jatuh berada di luar viewport
            if (!rect.Contains(Camera.main.WorldToViewportPoint(transform.position)))
            {
                Generate();
            }
        }
    }
}
