using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomColor : MonoBehaviour
{
	// Referensi ke komponen SpriteRenderer
	SpriteRenderer spriteRenderer;

	// Array untuk menyimpan warna-warna yang akan dipilih secara acak
	public Color[] colors;

	// Probabilitas objek menjadi tidak terlihat (0-100)
	// Jika 0, objek akan selalu terlihat
	[Range(0.0f, 100.0f)]
	public int invisibleProbability = 30;

	void Start()
	{
		// Mendapatkan komponen SpriteRenderer yang terpasang pada objek ini
		spriteRenderer = GetComponent<SpriteRenderer>();
		// Memanggil fungsi Generate untuk memilih warna secara acak
		Generate();
	}

	// Fungsi untuk menghasilkan warna baru atau membuat objek tidak terlihat
	public void Generate()
	{
		// Jika probabilitas tidak terlihat > 0 dan angka acak < probabilitas tidak terlihat
		if (invisibleProbability > 0 && Random.Range(0, 100) < invisibleProbability)
		{
			// Membuat objek tidak terlihat dengan mengatur warna menjadi transparan
			spriteRenderer.color = Color.clear;
			return;
		}

		// Jika array warna tidak kosong
		if (colors.Length > 0)
		{
			// Memilih warna secara acak dari array dan menerapkannya ke SpriteRenderer
			int colorSelected = Random.Range(0, colors.Length);
			spriteRenderer.color = colors[colorSelected];
		}
	}

	void Update()
	{
		// Fungsi Update dibiarkan kosong karena tidak ada yang perlu diupdate setiap frame
	}
}