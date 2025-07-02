using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Boss1Controller : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private GameObject dashEffectPrefab;
    [SerializeField] private Transform dashEffectPoint;

    [Header("Movement Area")]
    public BoxCollider2D battleArea;
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashCooldown = 5f;

    [Header("Health UI")]
    public Slider bossHealthSlider;
    public TextMeshProUGUI bossHealthText;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;

    // Internal
    private Transform player;
    private Rigidbody2D rb;
    private Vector2 minBounds, maxBounds;
    private bool canDash = true;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); // ambil komponen Animator
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        currentHealth = maxHealth;

        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(false);
        }

        if (battleArea != null)
        {
            Bounds bounds = battleArea.bounds;
            minBounds = bounds.min;
            maxBounds = bounds.max;
        }

        InitHealthUI();
    }

    private void Update()
    {
        if (player == null) return;

        FollowPlayerWithinBounds();

        if (canDash)
        {
            StartCoroutine(DashRoutine());
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.magnitude));
        }
    }

    private void FollowPlayerWithinBounds()
    {
        Vector2 targetPos = new Vector2(transform.position.x, player.position.y);
        targetPos.y = Mathf.Clamp(targetPos.y, minBounds.y, maxBounds.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

    private IEnumerator DashRoutine()
    {
        canDash = false;

        yield return new WaitForSeconds(1f); // sebelum dash

        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }

        // Tampilkan efek dash jika tersedia
        if (dashEffectPrefab != null && dashEffectPoint != null)
        {
            GameObject dashEffect = Instantiate(dashEffectPrefab, dashEffectPoint.position, Quaternion.identity);
            Destroy(dashEffect, 1f);
        }

        rb.linearVelocity = Vector2.left * dashSpeed;

        yield return new WaitUntil(() => transform.position.x < minBounds.x - 2f);

        // Reposisi ke kanan
        Vector2 newPos = new Vector2(maxBounds.x + 2f, Random.Range(minBounds.y, maxBounds.y));
        transform.position = newPos;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        UpdateHealthUI();

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            if (animator != null)
            {
                animator.SetTrigger("Die");
            }

            Destroy(gameObject, 0.5f);

            if (bossHealthSlider != null)
            {
                bossHealthSlider.gameObject.SetActive(false);
            }
        }
    }

    private void InitHealthUI()
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.gameObject.SetActive(true);
            bossHealthSlider.maxValue = maxHealth;
            bossHealthSlider.value = maxHealth;
        }

        if (bossHealthText != null)
        {
            bossHealthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    private void UpdateHealthUI()
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = currentHealth;
        }

        if (bossHealthText != null)
        {
            bossHealthText.text = $"{currentHealth} / {maxHealth}";
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bullet"))
        {
            Destroy(other.gameObject);
            TakeDamage(1);
        }
    }
}
