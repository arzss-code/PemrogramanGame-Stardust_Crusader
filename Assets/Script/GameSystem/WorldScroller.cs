using UnityEngine;

public class WorldScroller : MonoBehaviour
{
    public float scrollSpeed = 2f; // Kecepatan scroll objek dunia

    void Update()
    {
        // Hitung perpindahan ke kiri berdasarkan kecepatan scroll dan waktu
        Vector3 move = Vector3.left * scrollSpeed * Time.deltaTime;

        // Temukan semua objek dengan tag "WorldObject" dan geser posisinya
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("WorldObject"))
        {
            // Tambahkan perpindahan ke posisi setiap objek
            obj.transform.position += move;
        }
    }
}