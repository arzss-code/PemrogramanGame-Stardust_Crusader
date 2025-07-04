using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class CreditManager : MonoBehaviour
{
    public RectTransform panel;
    public Vector2 hiddenPosition = new Vector2(0, 1000);
    public Vector2 visiblePosition = new Vector2(0, 0);
    public float slideDuration = 0.5f;

    private bool isShowing = false;
    private Coroutine slideCoroutine;

    private void Start()
    {
        panel.anchoredPosition = hiddenPosition;
        gameObject.SetActive(false); // tersembunyi saat mulai
    }

    private void Update()
    {
        // Tekan ESC untuk menutup saat sedang aktif
        if (isShowing && Input.GetKeyDown(KeyCode.Escape))
        {
            HidePanel();
        }
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlidePanel(visiblePosition));
        isShowing = true;
    }

    public void HidePanel()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlidePanel(hiddenPosition, () =>
        {
            gameObject.SetActive(false); // nonaktifkan setelah animasi
        }));
        isShowing = false;
    }

    private System.Collections.IEnumerator SlidePanel(Vector2 targetPosition, System.Action onComplete = null)
    {
        Vector2 startPos = panel.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            panel.anchoredPosition = Vector2.Lerp(startPos, targetPosition, t);
            yield return null;
        }

        panel.anchoredPosition = targetPosition;
        onComplete?.Invoke();
    }
}
