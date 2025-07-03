using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonClickSFX : MonoBehaviour, IPointerEnterHandler
{
    [Header("Audio Clips")]
    public AudioClip clickSound;
    public AudioClip hoverSound;

    public float destroyDelay = 1f; // Durasi hidup AudioSource sebelum dihancurkan

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(PlayClickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
        {
            PlayOneShot(hoverSound);
        }
    }

    void PlayClickSound()
    {
        if (clickSound != null)
        {
            GameObject tempAudio = new GameObject("ClickSFX");
            DontDestroyOnLoad(tempAudio);
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = clickSound;
            tempSource.Play();

            Destroy(tempAudio, clickSound.length + 0.1f); // Biarkan selesai
        }
        else
        {
            Debug.LogWarning("Click sound belum disetel.");
        }
    }

    void PlayOneShot(AudioClip clip)
    {
        GameObject tempAudio = new GameObject("HoverSFX");
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.Play();
        Destroy(tempAudio, clip.length + 0.1f);
    }
}
