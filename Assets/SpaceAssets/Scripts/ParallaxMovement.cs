using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxMovement : MonoBehaviour
{
    // Menentukan arah pergerakan layar atau kamera
    ScrollDirection direction;
    // Nilai kecepatan ini menciptakan efek parallax
    // Catatan: Kecepatan ini mempengaruhi pergerakan objek berdasarkan kecepatan kamera
    public float minSpeed = 0.2f;
    public float maxSpeed = 0.6f;
    Vector3 speed;
    float scrollValue;
    float lastScrollValue;

    public enum BehaviourOnExit { Destroy, Regenerate };
    // Menentukan apakah objek dihancurkan atau diregenerasi ketika keluar dari layar
    public BehaviourOnExit behaviourOnExit = BehaviourOnExit.Regenerate;

    Transform cameraTransform;
    // Menentukan nilai offScreen yang harus dicapai objek untuk dianggap keluar dari layar
    // Juga digunakan untuk meregenerasi objek
    // Jika nilainya 1f, berarti lebar atau tinggi layar tergantung pada arah
    public float limitOffScreen = 1f;

    void Start()
    {
        // Mengambil arah scroll dari SpaceManager jika ada
        if (SpaceManager.instance != null)
            direction = SpaceManager.instance.scrollDirection;

        // Menetapkan kecepatan acak dalam rentang yang ditentukan
        float randomSpeed = Random.Range(minSpeed, maxSpeed);

        // Menentukan kecepatan berdasarkan arah scroll
        switch (direction)
        {
            case ScrollDirection.LeftToRight:
                speed = new Vector3(randomSpeed, 0f, 0f);
                break;
            case ScrollDirection.RightToLeft:
                speed = new Vector3(-randomSpeed, 0f, 0f);
                break;
            case ScrollDirection.DownToUp:
                speed = new Vector3(0f, randomSpeed, 0f);
                break;
            case ScrollDirection.UpToDown:
                speed = new Vector3(0f, -randomSpeed, 0f);
                break;
        }
    }

    void Regenerate()
    {
        // Mendapatkan posisi saat ini dari objek
        Vector3 currentPos = transform.position;

        // Misalnya arah RightToLeft
        switch (direction)
        {
            case ScrollDirection.RightToLeft:
                // Tetap di posisi Y yang sama dan pindahkan ke kanan layar
                float newY = currentPos.y;
                transform.position = Camera.main.ViewportToWorldPoint(new Vector3(1f + limitOffScreen, Camera.main.WorldToViewportPoint(transform.position).y, 10f));
                break;

                // Tambahkan logika lain jika menggunakan arah lain
        }

        // Hilangkan semua random jika tidak perlu
        // (Jika hanya ingin looping tanpa rotasi atau ubah ukuran)
    }

    void Update()
    {
        // Gerakkan objek berdasarkan waktu, bukan pergerakan kamera
        float multiplier = PlayerController.instance != null ? PlayerController.instance.BoostMultiplier : 1f;
        transform.position += speed * multiplier * Time.deltaTime;

        // Cek apakah objek keluar layar, dan regenerasi jika perlu
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        switch (direction)
        {
            case ScrollDirection.LeftToRight:
                if (viewportPos.x > 1f + limitOffScreen)
                {
                    if (behaviourOnExit == BehaviourOnExit.Regenerate) Regenerate();
                    else Destroy(gameObject);
                }
                break;

            case ScrollDirection.RightToLeft:
                if (viewportPos.x < -limitOffScreen)
                {
                    if (behaviourOnExit == BehaviourOnExit.Regenerate) Regenerate();
                    else Destroy(gameObject);
                }
                break;

            case ScrollDirection.DownToUp:
                if (viewportPos.y > 1f + limitOffScreen)
                {
                    if (behaviourOnExit == BehaviourOnExit.Regenerate) Regenerate();
                    else Destroy(gameObject);
                }
                break;

            case ScrollDirection.UpToDown:
                if (viewportPos.y < -limitOffScreen)
                {
                    if (behaviourOnExit == BehaviourOnExit.Regenerate) Regenerate();
                    else Destroy(gameObject);
                }
                break;
        }
    }
}