using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSize : MonoBehaviour
{
    // Nilai maksimum pengali ukuran, dapat diatur di Inspector
    [Range(1.0f, 10.0f)]
    public float multiplierMax = 3f;

    // Menyimpan skala awal objek
    Vector3 initialScale;

    void Start()
    {
        // Menyimpan skala awal objek saat pertama kali dibuat
        initialScale = transform.localScale;
        // Memanggil fungsi Generate untuk mengacak ukuran objek
        Generate();
    }

    public void Generate()
    {
        // Memilih pengali acak antara 1 dan multiplierMax
        float randomMultiplier = Random.Range(1f, multiplierMax);

        // Mengubah skala objek dengan mengalikan skala awal dengan pengali acak
        transform.localScale = initialScale * randomMultiplier;
    }

    void Update()
    {
        // Fungsi Update dibiarkan kosong karena tidak ada yang perlu diupdate setiap frame
    }
}