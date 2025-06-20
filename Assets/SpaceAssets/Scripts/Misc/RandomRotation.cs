using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    // Kecepatan rotasi maksimum objek
    public float rotationSpeedMax = 35f;
    // Kecepatan rotasi aktual yang akan digunakan
    float rotationSpeed;
    // Menentukan apakah kecepatan rotasi harus diacak atau tidak
    public bool randomize = true;

    void Start()
    {
        // Memanggil fungsi Generate untuk menginisialisasi rotasi
        Generate();
    }

    // Fungsi untuk menghasilkan kecepatan rotasi baru
    public void Generate()
    {
        if (randomize)
        {
            // Jika randomize true, pilih arah rotasi secara acak (searah atau berlawanan jarum jam)
            // dan tentukan kecepatan rotasi acak antara 0 dan rotationSpeedMax
            rotationSpeed = (Random.Range(0, 100) < 50 ? -1f : 1f) * Random.Range(0f, rotationSpeedMax);
        }
        else
        {
            // Jika randomize false, gunakan kecepatan rotasi maksimum
            rotationSpeed = rotationSpeedMax;
        }
    }

    void Update()
    {
        // Merotasi objek setiap frame berdasarkan kecepatan rotasi dan waktu yang berlalu
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}