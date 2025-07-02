using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 0.1f; // Kecepatan scroll latar belakang

    private Material material; // Material dari sprite renderer
    private Vector2 offset; // Offset untuk menggeser tekstur

    void Start()
    {
        // Ambil material dari komponen SpriteRenderer
        material = GetComponent<SpriteRenderer>().material;
        // Simpan offset awal dari tekstur
        offset = material.mainTextureOffset;
    }

    void Update()
    {
        // Tambahkan kecepatan scroll ke offset x berdasarkan waktu
        offset.x += scrollSpeed * Time.deltaTime;
        // Terapkan offset baru ke material untuk menggeser tekstur
        material.mainTextureOffset = offset;
    }
}