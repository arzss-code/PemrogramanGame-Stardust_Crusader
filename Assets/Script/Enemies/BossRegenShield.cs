using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class BossRegenShield : MonoBehaviour
{
    [Header("Shield Settings")]
    public int maxShieldHealth = 5;
    private int currentShieldHealth;

    [Header("Regeneration Settings")]
    public float regenDelay = 5f;
    public float blinkDuration = 1f;
    public float blinkInterval = 0.15f;

    [Header("Shield Visual")]
    public GameObject shieldVisual; // child GameObject berisi SpriteRenderer
    private Collider2D shieldCollider;
    private SpriteRenderer shieldRenderer;

    [Header("UI")]
    public Slider bossShieldSlider;
    public TextMeshProUGUI bossShieldText;

    private bool isRegenerating = false;

    private void Start()
    {
        currentShieldHealth = maxShieldHealth;

        if (shieldVisual == null) shieldVisual = gameObject;

        shieldCollider = GetComponent<Collider2D>();
        shieldRenderer = shieldVisual.GetComponent<SpriteRenderer>();

        UpdateShieldUI();
    }

    public int AbsorbDamage(int damage)
    {
        int absorbed = Mathf.Min(currentShieldHealth, damage);
        currentShieldHealth -= absorbed;
        UpdateShieldUI();

        if (currentShieldHealth <= 0 && !isRegenerating)
        {
            StartCoroutine(RechargeShield());
        }

        return damage - absorbed; // Damage sisa
    }

    private IEnumerator RechargeShield()
    {
        isRegenerating = true;

        yield return StartCoroutine(BlinkEffect());

        // Nonaktifkan visual & collider
        shieldVisual.SetActive(false);
        if (shieldCollider != null) shieldCollider.enabled = false;

        UpdateShieldUI(); // Update UI = 0 / max

        yield return new WaitForSeconds(regenDelay);

        // Aktifkan lagi
        currentShieldHealth = maxShieldHealth;
        shieldVisual.SetActive(true);
        if (shieldCollider != null) shieldCollider.enabled = true;

        yield return StartCoroutine(BlinkEffect());

        UpdateShieldUI();

        isRegenerating = false;
        Debug.Log("🛡️ Shield Boss kembali aktif!");
    }

    private IEnumerator BlinkEffect()
    {
        if (shieldRenderer == null)
            yield break;

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < blinkDuration)
        {
            visible = !visible;
            shieldRenderer.enabled = visible;
            elapsed += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        shieldRenderer.enabled = true; // pastikan terlihat
    }

    private void UpdateShieldUI()
    {
        if (bossShieldSlider != null)
        {
            bossShieldSlider.maxValue = maxShieldHealth;
            bossShieldSlider.value = currentShieldHealth;
        }

        if (bossShieldText != null)
        {
            bossShieldText.text = $"{currentShieldHealth} / {maxShieldHealth}";
        }
    }

    public bool IsActive() => currentShieldHealth > 0 && !isRegenerating;
}
