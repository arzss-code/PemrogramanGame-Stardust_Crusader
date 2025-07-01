using System.Collections;
using UnityEngine;

public class UIWarningController : MonoBehaviour
{
    public GameObject warningPanel;        // Drag RedWarning prefab atau panel aktif di sini
    public float flashInterval = 0.3f;

    private static bool hasShownWarning = false;
    private Coroutine flashRoutine;

    private void Awake()
    {
        // Saat scene dimulai, pastikan tidak aktif
        if (warningPanel != null)
        {
            warningPanel.SetActive(false);
        }
        hasShownWarning = false; // Reset agar bisa muncul lagi jika scene di-reload
    }

    public void ShowWarning(float duration)
    {
        if (hasShownWarning || warningPanel == null) return;

        hasShownWarning = true;
        warningPanel.SetActive(true);
        flashRoutine = StartCoroutine(FlashCoroutine(duration));
    }

    private IEnumerator FlashCoroutine(float duration)
    {
        float timer = 0f;
        bool visible = true;

        CanvasGroup cg = warningPanel.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = warningPanel.AddComponent<CanvasGroup>();
        }

        while (timer < duration)
        {
            cg.alpha = visible ? 1 : 0;
            visible = !visible;
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }

        cg.alpha = 0;
        warningPanel.SetActive(false);
    }
}
