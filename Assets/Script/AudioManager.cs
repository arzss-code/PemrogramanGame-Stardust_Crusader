using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public AudioSource sfxSource;
    public AudioSource bgmSource;

    public AudioClip buttonClickSFX;
    public AudioClip bgmClip;

    private void Awake()
    {
        // Bikin Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // jangan dihancurkan saat pindah scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (bgmSource != null && bgmClip != null)
        {
            if (bgmSource.clip == null) // hanya set jika belum ada
            {
                bgmSource.clip = bgmClip;
                bgmSource.loop = true;

                float savedVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
                bgmSource.volume = savedVolume;

                bgmSource.Play();
            }
        }
    }


    // Function untuk play sfx
    public void PlaySFX()
    {
        if (sfxSource != null && buttonClickSFX != null)
        {
            sfxSource.PlayOneShot(buttonClickSFX);
        }
    }

    // Function untuk atur volume BGM
    public void SetBGMVolume(float value)
    {
        if (bgmSource == null)
        {
            Debug.LogWarning("BGM Source hilang atau sudah dihancurkan.");
            return;
        }

        bgmSource.volume = value;
    }



    // Function untuk mute/unmute BGM
    public void SetBGMMute(bool mute)
    {
        if (bgmSource != null)
        {
            bgmSource.mute = mute;
        }
    }

    public void ChangeBGM(AudioClip newClip)
    {
        if (bgmSource == null || newClip == null) return;

        if (bgmSource.clip == newClip) return; // sudah dipakai

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();
    }


}
