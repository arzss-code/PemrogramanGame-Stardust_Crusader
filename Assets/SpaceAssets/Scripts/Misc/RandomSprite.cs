using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomSprite : MonoBehaviour
{
	// Referensi ke komponen SpriteRenderer
	SpriteRenderer spriteRenderer;

	// Array untuk menyimpan sprite-sprite yang akan dipilih secara acak
	public Sprite[] sprites;

	void Start()
	{
		// Mendapatkan komponen SpriteRenderer yang terpasang pada objek ini
		spriteRenderer = GetComponent<SpriteRenderer>();
		// Memanggil fungsi Generate untuk memilih sprite secara acak
		Generate();
	}

	// Fungsi untuk memilih dan menerapkan sprite secara acak
	public void Generate()
	{
		// Memeriksa apakah ada sprite dalam array
		if (sprites.Length > 0)
		{
			// Memilih sprite secara acak dari array dan menerapkannya ke SpriteRenderer
			spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
		}
	}

	// Update dipanggil sekali per frame
	void Update()
	{
		// Fungsi Update dibiarkan kosong karena tidak ada yang perlu diupdate setiap frame
	}
}