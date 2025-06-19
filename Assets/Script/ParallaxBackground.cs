using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float moveSpeed; // Kecepatan pergerakan latar belakang
    float backgroundImageWidth; // Lebar gambar latar belakang dalam unit dunia

    private void Start()
    {
        // Ambil sprite dari komponen SpriteRenderer
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        // Hitung lebar gambar latar belakang berdasarkan ukuran tekstur dan skala unit
        backgroundImageWidth = sprite.texture.width / sprite.pixelsPerUnit;
    }

    private void Update()
    {
        // Hitung perpindahan pada sumbu X berdasarkan kecepatan dan waktu
        float moveX = moveSpeed * Time.deltaTime;
        // Geser posisi latar belakang pada sumbu X
        transform.position += new Vector3(moveX, 0);

        // Jika posisi X melebihi lebar gambar, reset posisi ke awal
        if (Mathf.Abs(transform.position.x) - backgroundImageWidth > 0)
        {
            transform.position = new Vector3(0f, transform.position.y);
        }
    }
}