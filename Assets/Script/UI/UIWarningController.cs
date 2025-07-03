using System.Collections;
using UnityEngine;

public class UIWarningController : MonoBehaviour
{
    [Header("Prefab Reference")]
    [SerializeField] private GameObject redWarningPrefab;

    [Header("Settings")]
    [SerializeField] private float flashInterval = 0.3f;
    [SerializeField] private float defaultWarningDuration = 3f;

    [Header("Canvas Parent")]
    [SerializeField] private Transform uiCanvas; // drag Canvas UI kamu di Inspector

    [Header("Spawner Reference")]
    [SerializeField] private MonoBehaviour objectSpawner; // drag ObjectSpawner (bisa script apapun yg inherit MonoBehaviour)

    private GameObject currentInstance;
    private Coroutine flashCoroutine;

    public void ShowWarning(float duration = -1f)
    {
        Debug.Log("🚨 ShowWarning Called!");

        if (redWarningPrefab == null)
        {
            Debug.LogWarning("❌ redWarningPrefab belum di-assign!");
            return;
        }

        if (uiCanvas == null)
        {
            Debug.LogWarning("❌ uiCanvas belum di-assign!");
            return;
        }

        // ✅ Nonaktifkan hanya ObjectSpawner
        if (objectSpawner != null)
        {
            objectSpawner.enabled = false;
            Debug.Log("🛑 ObjectSpawner dinonaktifkan karena Warning Boss aktif.");
        }

        float usedDuration = (duration > 0) ? duration : defaultWarningDuration;

        // Hapus warning sebelumnya jika masih aktif
        HideWarning();

        currentInstance = Instantiate(redWarningPrefab, uiCanvas);
        currentInstance.name = "RedWarningInstance";
        currentInstance.transform.SetAsLastSibling(); // tampil di atas UI lain

        CanvasGroup cg = currentInstance.GetComponent<CanvasGroup>();
        if (cg == null) cg = currentInstance.AddComponent<CanvasGroup>();
        cg.alpha = 0;

        flashCoroutine = StartCoroutine(FlashRoutine(currentInstance, cg, usedDuration));

        // 🔊 Ganti BGM ke Boss BGM
        if (AudioManager.instance != null)
        {
            StartCoroutine(SwitchToBossBGM());
        }
    }

    public void HideWarning()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        if (currentInstance != null)
        {
            Destroy(currentInstance);
            currentInstance = null;
        }
    }

    private IEnumerator FlashRoutine(GameObject instance, CanvasGroup cg, float duration)
    {
        float timer = 0f;
        bool isVisible = true;

        while (timer < duration)
        {
            cg.alpha = isVisible ? 1 : 0;
            isVisible = !isVisible;
            timer += flashInterval;
            yield return new WaitForSeconds(flashInterval);
        }

        HideWarning();
    }

    private IEnumerator SwitchToBossBGM()
    {
        AudioManager audio = AudioManager.instance;

        // Fade out BGM sekarang
        audio.FadeOutBGM(1.5f);

        // Tunggu sebelum ganti
        yield return new WaitForSeconds(1.5f);

        if (audio.bossBGM != null)
        {
            audio.ChangeBGM(audio.bossBGM);
            audio.FadeInBGM(1.5f, 0.7f); // Volume boss, bisa kamu sesuaikan
        }
    }
}
