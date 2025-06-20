using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum untuk menentukan arah scroll
public enum ScrollDirection { LeftToRight, RightToLeft, DownToUp, UpToDown };

public class SpaceManager : MonoBehaviour
{
    // Menentukan arah scroll layar atau kamera
    public ScrollDirection scrollDirection = ScrollDirection.LeftToRight;
    ScrollDirection direction; // Variabel untuk menyimpan arah scroll saat ini

    public static SpaceManager instance = null; // Instance singleton untuk akses global

    void Start()
    {
        direction = scrollDirection; // Set arah scroll awal
        instance = this; // Set instance singleton
    }

    void Update()
    {
        // Mencegah perubahan arah scroll saat mode eksekusi (menghapus ini dapat menyebabkan bug)
        if (direction != scrollDirection)
        {
            scrollDirection = direction; // Kembalikan arah scroll ke nilai awal
        }
    }
}