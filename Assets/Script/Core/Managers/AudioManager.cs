using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource sfxSource;
    public AudioSource bgmSource;

    [Header("BGM Clips")]
    [Tooltip("Musik yang akan diputar di Main Menu.")]
    [SerializeField] private AudioClip mainMenuBGM;

    [Header("SFX")]
    [SerializeField] private AudioClip buttonClickSFX;

    private Coroutine m_MusicFadeCoroutine;
    private float m_TargetBGMVolume = 1.0f;

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
            return; // Hentikan eksekusi lebih lanjut jika ini adalah duplikat
        }

        // Inisialisasi volume BGM dari PlayerPrefs
        m_TargetBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 1.0f);
        if (bgmSource != null)
        {
            bgmSource.volume = m_TargetBGMVolume;
            bgmSource.loop = true;
        }
    }

    private void Start()
    {
        // Saat game pertama kali jalan, putar musik menu utama jika belum ada musik yang berputar.
        if (mainMenuBGM != null && !bgmSource.isPlaying)
        {
            bgmSource.clip = mainMenuBGM;
            bgmSource.Play();
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
        m_TargetBGMVolume = Mathf.Clamp01(value);

        // Jika tidak sedang dalam proses fade, langsung atur volume
        if (bgmSource != null && m_MusicFadeCoroutine == null)
        {
            bgmSource.volume = m_TargetBGMVolume;
        }

        PlayerPrefs.SetFloat("BGMVolume", m_TargetBGMVolume);
        PlayerPrefs.Save();
    }


    // Function untuk mute/unmute BGM
    public void SetBGMMute(bool mute)
    {
        if (bgmSource != null)
        {
            bgmSource.mute = mute;
        }
    }

    /// <summary>
    /// Mengganti BGM yang sedang diputar dengan klip baru menggunakan efek fade.
    /// </summary>
    /// <param name="newClip">Klip musik baru yang akan diputar.</param>
    /// <param name="fadeDuration">Durasi total untuk fade out dan fade in.</param>
    public void ChangeBGM(AudioClip newClip, float fadeDuration = 2.0f)
    {
        if (bgmSource == null || newClip == null) return;
        if (bgmSource.clip == newClip && bgmSource.isPlaying) return; // Jangan ganti jika klip sudah sama dan sedang diputar

        if (m_MusicFadeCoroutine != null)
            StopCoroutine(m_MusicFadeCoroutine);

        m_MusicFadeCoroutine = StartCoroutine(FadeSwitchBGM(newClip, fadeDuration));
    }

    private IEnumerator FadeSwitchBGM(AudioClip newClip, float duration)
    {
        float startVolume = bgmSource.volume;
        float fadeOutDuration = bgmSource.isPlaying ? duration / 2f : 0f;
        float fadeInDuration = duration / 2f;
        float timer = 0f;

        // --- FADE OUT ---
        if (fadeOutDuration > 0)
        {
            while (timer < fadeOutDuration)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeOutDuration);
                timer += Time.unscaledDeltaTime; // Gunakan unscaledDeltaTime agar fade tetap berjalan saat game di-pause
                yield return null;
            }
        }

        // --- GANTI KLIP ---
        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.Play();

        // --- FADE IN ---
        timer = 0f;
        while (timer < fadeInDuration)
        {
            bgmSource.volume = Mathf.Lerp(0f, m_TargetBGMVolume, timer / fadeInDuration);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        bgmSource.volume = m_TargetBGMVolume;
        m_MusicFadeCoroutine = null;
    }

    /// <summary>
    /// Memutar BGM untuk Main Menu. Panggil ini sebelum kembali ke Main Menu.
    /// </summary>
    public void PlayMainMenuBGM()
    {
        // Gunakan ChangeBGM untuk mendapatkan efek fade yang mulus
        if (mainMenuBGM != null)
        {
            ChangeBGM(mainMenuBGM, 1.0f);
        }
    }
}
