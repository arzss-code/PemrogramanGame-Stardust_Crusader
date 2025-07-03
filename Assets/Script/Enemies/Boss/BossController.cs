using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class BossController : MonoBehaviour
{
    public enum BossState { Entering, Fighting, Dying }
    public BossState currentState;

    private enum FightingSubState { Patrolling, Charging, Returning }
    private FightingSubState fightingState;

    [Header("Boss Stats")]
    public int health = 100;
    private int maxHealth;

    [Header("Boss Movement")]
    public float entranceSpeed = 5f;
    public float moveSpeed = 2f;
    public float moveDistance = 5f;
    private Vector2 startPosition;
    private Vector2 targetBattlePosition;

    [Header("Charge Attack")]
    public float chargeSpeed = 20f;
    public float patrolDuration = 4f;
    private float patrolTimer;
    private Vector2 chargeTargetPosition;
    private Transform playerTransform;

    [Header("UI References")]
    public Slider healthBarSlider;
    public TextMeshProUGUI healthText;

    [Header("Effects")]
    public GameObject destructionEffect;
    public Material whiteFlashMaterial;
    private Material defaultMaterial;
    private SpriteRenderer spriteRenderer;

    public void Initialize(Vector2 battlePosition)
    {
        this.targetBattlePosition = battlePosition;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultMaterial = spriteRenderer.material;
        maxHealth = health;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    private void Start()
    {
        currentState = BossState.Entering;
    }

    private void Update()
    {
        switch (currentState)
        {
            case BossState.Entering:
                HandleEntrance();
                break;
            case BossState.Fighting:
                HandleFightingBehavior();
                break;
            case BossState.Dying:
                break;
        }
    }

    private void HandleEntrance()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetBattlePosition, entranceSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, targetBattlePosition) < 0.01f)
        {
            startPosition = transform.position;
            SetupHealthBar();
            currentState = BossState.Fighting;
            fightingState = FightingSubState.Patrolling;
            patrolTimer = 0f;
        }
    }

    private void HandleFightingBehavior()
    {
        switch (fightingState)
        {
            case FightingSubState.Patrolling:
                float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
                transform.position = new Vector2(startPosition.x, newY);

                patrolTimer += Time.deltaTime;
                if (patrolTimer >= patrolDuration)
                {
                    if (playerTransform != null)
                        chargeTargetPosition = new Vector2(-60f, playerTransform.position.y);
                    else
                        chargeTargetPosition = new Vector2(-60f, transform.position.y);

                    fightingState = FightingSubState.Charging;
                }
                break;

            case FightingSubState.Charging:
                transform.position = Vector2.MoveTowards(transform.position, chargeTargetPosition, chargeSpeed * Time.deltaTime);
                if (Vector2.Distance(transform.position, chargeTargetPosition) < 0.01f)
                {
                    fightingState = FightingSubState.Returning;
                }
                break;

            case FightingSubState.Returning:
                Vector2 returnPosition = new Vector2(startPosition.x, transform.position.y);
                transform.position = Vector2.MoveTowards(transform.position, returnPosition, chargeSpeed * Time.deltaTime);
                if (Mathf.Abs(transform.position.x - startPosition.x) < 0.01f)
                {
                    patrolTimer = 0f;
                    fightingState = FightingSubState.Patrolling;
                }
                break;
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (currentState == BossState.Dying) return;

        health -= damageAmount;
        health = Mathf.Max(health, 0);
        UpdateHealthBar();
        StartCoroutine(FlashWhite());

        if (health <= 0)
        {
            currentState = BossState.Dying;
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.Kill(); // 💀 langsung mati
            }
        }
    }


    private void Die()
    {
        Debug.Log("BOSS DIKALAHKAN!");

        if (healthBarSlider != null)
            healthBarSlider.gameObject.SetActive(false);

        if (destructionEffect != null)
            Instantiate(destructionEffect, transform.position, Quaternion.identity);

        GameManager.instance.LevelCompleted();
        Destroy(gameObject);
    }

    private void SetupHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.gameObject.SetActive(true);
            healthBarSlider.maxValue = maxHealth;
            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = health;
            if (healthText != null)
            {
                healthText.text = $"{health} / {maxHealth}";
            }
        }
    }

    private IEnumerator FlashWhite()
    {
        if (spriteRenderer != null && whiteFlashMaterial != null)
        {
            spriteRenderer.material = whiteFlashMaterial;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.material = defaultMaterial;
        }
    }
}
