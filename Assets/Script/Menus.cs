using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    // Panggil fungsi ini saat tombol Play ditekan
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1"); // Ganti dengan nama scene utama kamu
    }

    // Panggil fungsi ini saat tombol Quit ditekan
    public void QuitGame()
    {
        Debug.Log("Keluar dari game...");
        Application.Quit();
    }
}
