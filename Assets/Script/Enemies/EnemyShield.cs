using UnityEngine;

public class EnemyShield : MonoBehaviour
{
    public int shieldHealth = 2;

    /// <summary>
    /// Mengurangi HP shield dan mengembalikan true jika damage tersisa.
    /// </summary>
    public int AbsorbDamage(int damage)
    {
        int absorbed = Mathf.Min(shieldHealth, damage);
        shieldHealth -= absorbed;

        if (shieldHealth <= 0)
        {
            Destroy(gameObject); // Hancurkan Shield (Prefab)
            Debug.Log("🛡️ Shield Hancur!");
        }

        return damage - absorbed; // Damage sisa (jika ada) akan ke badan musuh
    }
}
