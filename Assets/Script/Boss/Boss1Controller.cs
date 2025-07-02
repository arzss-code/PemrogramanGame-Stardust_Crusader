// Boss1Controller.cs
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Boss1Controller : MonoBehaviour
{
    [Header("Movement Area")]
    [SerializeField] private BoxCollider2D battleArea;
    [SerializeField] private float followSpeed = 3f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashCooldown = 5f;

    [Header("Health UI")]
    [SerializeField] private Slider bossHealthSlider;
    [SerializeField] private Text bossNameText;
    [SerializeField] private GameObject uiGroup; // Untuk menampilkan/hide UI boss

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    private int currentHealth;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 minBounds, maxBounds;
    private bool canDash = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        currentHealth = maxHealth;
        InitHealthUI();

        if (battleArea != null)
        {
            Bounds bounds = battleArea.bounds;
            minBounds = bounds.min;
            maxBounds = bounds.max;
        }
    }

    private void Update()
    {
        if (player == null) return;

        FollowPlayerWithinBounds();

        if (canDash)
        {
            StartCoroutine(DashRoutine());
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
        yield return new WaitForSeconds(1f); // Antisipasi sebelum dash

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
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            uiGroup?.SetActive(false);
        }
    }

    private void InitHealthUI()
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.maxValue = maxHealth;
            bossHealthSlider.value = maxHealth;
        }
        if (bossNameText != null)
        {
            bossNameText.text = name.ToUpper();
        }
        if (uiGroup != null)
        {
            uiGroup.SetActive(true);
        }
    }

    private void UpdateHealthUI()
    {
        if (bossHealthSlider != null)
        {
            bossHealthSlider.value = currentHealth;
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
