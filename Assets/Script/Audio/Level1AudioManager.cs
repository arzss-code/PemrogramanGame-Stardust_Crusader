using UnityEngine;

public class Level1AudioManager : MonoBehaviour
{
    [Header("Level 1 BGM")]
    [SerializeField] private AudioClip level1BGM;
    [SerializeField] private float level1Volume = 0.3f;

    [Header("Boss BGM")]
    [SerializeField] private AudioClip bossBGM;
    [SerializeField] private float bossVolume = 0.6f;

    [Header("Volume Transition")]
    [SerializeField] private float volumeLerpSpeed = 1f; // semakin tinggi = lebih cepat
    private float targetVolume;

    private AudioSource bgmSource;

    private void Start()
    {
        if (AudioManager.instance != null)
        {
            Destroy(AudioManager.instance.gameObject);
        }

        // Buat AudioSource untuk BGM level ini
        GameObject bgmObj = new GameObject("Level1BGM");
        bgmSource = bgmObj.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        if (level1BGM != null)
        {
            bgmSource.clip = level1BGM;
            bgmSource.volume = 0f;
            targetVolume = level1Volume;
            bgmSource.Play();
        }
    }

    private void Update()
    {
        if (bgmSource != null)
        {
            bgmSource.volume = Mathf.Lerp(bgmSource.volume, targetVolume, volumeLerpSpeed * Time.deltaTime);
        }
    }

    // Panggil ini saat Boss muncul (misalnya dari UIWarningController)
    public void SwitchToBossBGM()
    {
        if (bossBGM != null && bgmSource != null)
        {
            bgmSource.clip = bossBGM;
            bgmSource.Play();
            bgmSource.volume = 0f; // mulai dari 0
            targetVolume = bossVolume;
        }
    }
}
