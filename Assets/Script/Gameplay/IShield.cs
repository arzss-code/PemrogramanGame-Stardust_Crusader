/// <summary>
/// Antarmuka untuk semua jenis shield yang bisa menyerap damage.
/// Dengan ini, semua shield dapat dikenali dan diperlakukan sama oleh sistem lain (seperti peluru).
/// </summary>
public interface IShield
{
    /// <summary> Mengurangi health shield dan mengembalikan sisa damage yang tidak terserap. </summary>
    int AbsorbDamage(int damage);
}