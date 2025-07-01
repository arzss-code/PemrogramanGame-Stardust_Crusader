using UnityEngine;

public class PowerUpItem : MonoBehaviour
{
    // Enum untuk mendefinisikan semua jenis power-up yang ada di game Anda
    public enum PowerUpType { WeaponBoost, Health, Energy }

    [Header("Item Settings")]
    [Tooltip("Pilih jenis power-up untuk item ini dari dropdown")]
    public PowerUpType type;

    [Tooltip("Nilai dari power-up. Misal: 10 untuk durasi boost, 1 untuk jumlah health, 50 untuk jumlah energy")]
    public float value = 10f;

    [Header("Effects")]
    [Tooltip("Efek partikel yang muncul saat item diambil (opsional)")]
    public GameObject pickupEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Hanya proses jika yang menyentuh adalah pemain
        if (other.CompareTag("Player"))
        {
            // Ambil instance PlayerController
            PlayerController player = PlayerController.instance;
            if (player == null) return; // Keluar jika player tidak ditemukan

            // Gunakan switch untuk menentukan apa yang harus dilakukan berdasarkan jenis item
            switch (type)
            {
                case PowerUpType.WeaponBoost:
                    player.ActivateWeaponBoost(value);
                    break;
                case PowerUpType.Health:
                    // Kita cast 'value' ke int karena health adalah bilangan bulat
                    player.RestoreHealth((int)value); 
                    break;
                case PowerUpType.Energy:
                    player.RestoreEnergy(value);
                    break;
            }

            // Tampilkan efek visual/suara jika ada
            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            // Hancurkan objek item setelah diambil
            Destroy(gameObject);
        }
    }
}